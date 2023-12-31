using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjDoor : InteractableObject
{
    [SerializeField] Transform graphicRoot;

    protected override void Use(BaseChar who)
    {
        LeanTween.cancel(graphicRoot.gameObject);
        graphicRoot.transform.localScale = Vector3.one;
        LeanTween.scale(graphicRoot.gameObject, new Vector2(1.125f,1.125f), 0.5f).setEase(LeanTweenType.punch);
        animCon.Play("Open");
        col.transform.localScale = Vector3.zero;
    }

    public void AfterOpen()
    {
        animCon.Play("Close");
    }

    public void AfterClose()
    {
        col.transform.localScale = Vector3.one;
    }
}
