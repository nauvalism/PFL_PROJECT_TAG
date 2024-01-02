using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : SoundManager
{
    [SerializeField] AudioLowPassFilter alpf;

    public void LowPassing(float duration = 1.0f)
    {
        LeanTween.cancel(alpf.gameObject, true);
        LeanTween.value(alpf.gameObject, alpf.cutoffFrequency, 200.0f, duration).setOnUpdate((float f)=>{
            alpf.cutoffFrequency = f;
        }).setOnComplete(()=>{
            alpf.cutoffFrequency = 200.0f;
        });
    }

    public void UnlowPassing(float duration = 0.125f)
    {
        LeanTween.cancel(alpf.gameObject, true);
        LeanTween.value(alpf.gameObject, alpf.cutoffFrequency, 22000.0f, duration).setOnUpdate((float f)=>{
            alpf.cutoffFrequency = f;
        }).setOnComplete(()=>{
            alpf.cutoffFrequency = 22000.0f;
        });
    }

    
}
