using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseMap : MonoBehaviour
{
    [SerializeField] string mapCodeName;
    [SerializeField] List<BaseObject> uninterractableObjects;


    protected virtual void OnValidate()
    {

    }

    protected virtual void Awake()
    {

    }

    protected virtual void Start()
    {

    }

    protected virtual void Update()
    {
        
    }

    public string GetCodeName()
    {
        return mapCodeName;
    }
}
