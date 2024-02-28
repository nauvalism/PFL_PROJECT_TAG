using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoreAI : MonoBehaviour
{
    [SerializeField] TopChar topChar;
    [SerializeField] BaseChar coreChar;
    
    [Header("STATES")]
    [SerializeField] bool overrideControl = false;

    private void OnValidate() {
        Debug.Log("Called");
        if(overrideControl)
        {
            OverrideControl();
        }
    }


    public void OverrideControl()
    {
        
    }
}
