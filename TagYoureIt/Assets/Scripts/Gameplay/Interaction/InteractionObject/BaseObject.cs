using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class BaseObject : MonoBehaviour
{
   [SerializeField] BaseMap belongMap;
   [SerializeField] string objectIdentity;
   [SerializeField] int objID;
   [SerializeField] protected bool interactable = false;
   [SerializeField] protected SortingGroup sort;
   [SerializeField] protected Animator animCon;
   public virtual void OnValidate()
   {
        if(belongMap != null)
        {
            objectIdentity = belongMap.GetCodeName()+"_"+objID;
        }
   }

   public void SetObjectID(int to)
   {
      this.objID = to;
   }

   public virtual void Awake()
   {

   }
   
   public virtual void Start()
   {

   }

   public virtual void Update()
   {

   }

   
}
