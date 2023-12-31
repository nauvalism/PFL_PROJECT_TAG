using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EffectList
{
    camShake = 0,
    slowMotion = 0
}

public class Effects : MonoBehaviour
{
    public static Effects instance {get;set;}
    [SerializeField] EffectList activeEffect;
    [SerializeField] List<BaseEffect> effects;


    [SerializeField] BaseEffect shakeEffect;

    private void Awake() {
        instance = this;
    }

    public void TriggerEffect(EffectList eff)
    {
        this.activeEffect = eff;
        effects[(int)eff].Trigger();
    }
}
