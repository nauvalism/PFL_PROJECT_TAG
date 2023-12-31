using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [SerializeField] CanvasGroup tagSensor;
    [SerializeField] CanvasGroup dangerSensor;
    [SerializeField] List<TextMeshProUGUI> lives;


    public void FadeTag(System.Action during, System.Action after)
    {
        LeanTween.cancel(tagSensor.gameObject);
        if(during != null)
        {
            during();
        }
        LeanTween.value(tagSensor.gameObject, .0f, 1.0f, 0.25f).setEase(LeanTweenType.easeOutQuad).setOnUpdate((float f)=>{
            tagSensor.alpha = f;
        }).setOnComplete(()=>{
            if(after != null)
            {
                after();
            } 
        });
    }

    public void UnFadeTag(System.Action during, System.Action after)
    {
        LeanTween.cancel(tagSensor.gameObject);
        if(during != null)
        {
            during();
        }
        LeanTween.value(tagSensor.gameObject, tagSensor.alpha, .0f, 0.125f).setEase(LeanTweenType.easeOutQuad).setOnUpdate((float f)=>{
            tagSensor.alpha = f;
        }).setOnComplete(()=>{
            if(after != null)
            {
                after();
            } 
        });
    }

    public void UpdateLive(PlayerIdentity who, int live)
    {
        lives[(int)who].text = live.ToString();
    }
}
