using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionSensor : MonoBehaviour
{
    
    [SerializeField] BaseChar owner;
    [SerializeField] InteractableObject interactedObject;
    [SerializeField] List<InteractableObject> interactedObjects;
    [SerializeField] BaseChar interactedChar;
    [SerializeField] Collider2D sensorCollider;

    private void Awake() {
        interactedObjects = new List<InteractableObject>();
    }

    public void Trigger()
    {
        if(interactedObject != null)
        {
            interactedObject.Trigger(owner);
            OnTriggerExit2D(interactedObject.GetCollider());
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if(other.CompareTag("InteractableObject"))
        {
            interactedObject = null;
            interactedObject = other.transform.parent.gameObject.GetComponent<InteractableObject>();
            owner.ShowBubble((int)interactedObject.GetIType());
        
            // int e = Exists(other.GetComponent<InteractableObject>());
            // if(e == -1)
            // {
            //     interactedObjects.Add(interactedObject);
            // }
        }

        if(other.CompareTag("Character"))
        {
            if(owner.IsYou())
            {
                if(owner.Bombed())
                {
                    GameplayController.instance.FadeTaggable(null,null);
                }
                
                
            }

            if(owner.Bombed())
            {
                interactedChar = other.GetComponent<BaseChar>();
                
            }
            
        }
            
    }

    private void OnTriggerExit2D(Collider2D other) {
        if(other.CompareTag("InteractableObject"))
        {
            if(other.transform.parent.GetComponent<InteractableObject>() == interactedObject)
                interactedObject = null;

            // int e = Exists(other.transform.parent.GetComponent<InteractableObject>());
            // if(e != -1)
            // {
            //     RemoveInteractable(e);
            // }

            owner.HideBubble();
        }

        if(other.CompareTag("Character"))
        {
            NullifyInteractedChar();
            GameplayController.instance.FadeUnTaggable(null,null);
        }
    }

    public void NullifyInteractedChar()
    {
        interactedChar = null;
    }

    public void DisableCol()
    {
        this.sensorCollider.transform.localScale = Vector3.zero;
    }

    public void EnableCol()
    {
        this.sensorCollider.transform.localScale = Vector3.one;
    }

    void RemoveInteractable(InteractableObject obj)
    {
        List<InteractableObject> _copy = new List<InteractableObject>(interactedObjects);
        _copy.Remove(obj);
        interactedObjects = new List<InteractableObject>(_copy);
    }

    void RemoveInteractable(int where)
    {
        List<InteractableObject> _copy = new List<InteractableObject>(interactedObjects);
        _copy.RemoveAt(where);
        interactedObjects = new List<InteractableObject>(_copy);
    }

    private int Exists(InteractableObject obj)
    {
        int result = -1;

        if(interactedObjects.Count > 0)
        {
            for(int i = 0 ; i < interactedObjects.Count ; i++)
            {
                if(interactedObjects[i].GetID() == obj.GetID())
                {
                    return i;
                }
            }
        }


        return result;
    }

    public void IgnoreCol(Collider2D from)
    {
        Physics2D.IgnoreCollision(from, sensorCollider);
    }

    public BaseChar GetInteractedChar()
    {
        return interactedChar;
    }
}
