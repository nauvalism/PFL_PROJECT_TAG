using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum InteractableType
{
    takeable = 0,
    usable = 1,
    askable = 2
}

public class InteractableObject : BaseObject
{
    [SerializeField] protected int interactableID;
    [SerializeField] protected InteractableType type;
    [SerializeField] protected Collider2D col;
    
    public void SetInteractableID(int to)
    {
        this.interactableID = to;
    }

    public void Trigger(BaseChar whoTriggersIt)
    {
        if(type == InteractableType.takeable)
        {
            Take(whoTriggersIt);
        }
        else if(type == InteractableType.usable)
        {
            Use(whoTriggersIt);
        }
        else
        {
            Ask(whoTriggersIt);
        }
    }

    protected virtual void Take(BaseChar who)
    {

    }

    protected virtual void Use(BaseChar who)
    {

    }

    protected virtual void Ask(BaseChar who)
    {

    }

    public InteractableType GetIType()
    {
        return type;
    }

    public int GetID()
    {
        return interactableID;
    }

    public Collider2D GetCollider()
    {
        return col;
    }
}
