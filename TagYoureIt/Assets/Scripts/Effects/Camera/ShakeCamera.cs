using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class ShakeCamera : BaseEffect
{
    [SerializeField] CinemachineVirtualCamera cmv;
    [SerializeField] CinemachineBasicMultiChannelPerlin shake;

    private void OnValidate() {
        if(cmv != null)
        {
            shake = cmv.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        }
    }

    public override void Trigger()
    {
        LeanTween.cancel(cmv.gameObject);
        shake.m_AmplitudeGain = 3.0f;
        LeanTween.value(cmv.gameObject, shake.m_AmplitudeGain, .0f, 2.0f).setEase(LeanTweenType.easeOutQuad).setOnUpdate((float f)=>{
            shake.m_AmplitudeGain = f;
        });
    }

    
}
