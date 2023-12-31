using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OpeningUI : MonoBehaviour
{
    [SerializeField] float readyDelay = 2.0f;
    [SerializeField] float readySpeed = 1.0f;
    [SerializeField] float goSpeed = 1.0f;
    [SerializeField] CanvasGroup openingFader;
    [SerializeField] GameObject readyTxt;
    [SerializeField] GameObject goTxt;
    [SerializeField] TextMeshProUGUI goTxtTxt;
    [SerializeField] Image flashGO;



    public void OpeningUISequence(System.Action after)
    {
        LeanTween.cancel(readyTxt.gameObject);
        LeanTween.cancel(goTxt.gameObject);
        LeanTween.cancel(flashGO.GetComponent<RectTransform>());
        
        readyTxt.transform.localPosition = new Vector3(-1500.0f, .0f, .0f);
        goTxt.transform.localScale = new Vector3(3.0f,3.0f,3.0f);
        goTxtTxt.alpha = .0f;
        flashGO.color = Color.clear;
        

        openingFader.interactable = true;
        openingFader.blocksRaycasts = true;
        LeanTween.value(openingFader.gameObject, .0f, 1.0f, 0.5f).setEase(LeanTweenType.easeOutQuad).setOnUpdate((float f)=>{
            openingFader.alpha = f;
        }).setOnComplete(()=>{
            readyTxt.GetComponent<AudioSource>().Play();
            LeanTween.moveLocalX(readyTxt, .0f, readySpeed).setEase(LeanTweenType.easeOutQuad).setOnComplete(()=>{
                
                LeanTween.moveLocalX(readyTxt, 1500.0f, 1.0f).setDelay(readyDelay/2).setEase(LeanTweenType.easeOutQuad);
                
                LeanTween.value(goTxt, .0f, 1.0f, 0.125f).setEase(LeanTweenType.easeOutQuad).setOnUpdate((float f)=>{
                        goTxtTxt.alpha = f;
                }).setDelay(readyDelay);
                LeanTween.scale(goTxt.gameObject, new Vector3(1.5f, 1.5f,1.5f), 1.0f).setEase(LeanTweenType.easeOutQuad).setDelay(readyDelay).setOnComplete(()=>{
                    goTxt.GetComponent<AudioSource>().Play();
                    flashGO.color = Color.white;
                    LeanTween.value(goTxt, 1.0f, .0f, 2.5f).setEase(LeanTweenType.easeOutQuad).setOnUpdate((float f)=>{
                        goTxtTxt.alpha = f;
                    });
                    LeanTween.scale(goTxt, new Vector3(3.0f,3.0f,3.0f), 3.0f);
                    LeanTween.alpha(flashGO.GetComponent<RectTransform>(), .0f, 1.25f).setEase(LeanTweenType.easeOutQuad);

                    openingFader.interactable = false;
                    openingFader.blocksRaycasts = false;

                    LeanTween.value(openingFader.gameObject, 1.0f, .0f, 0.5f).setEase(LeanTweenType.easeOutQuad).setOnUpdate((float f)=>{
                        openingFader.alpha = f;
                    });


                    if(after != null)
                    {
                        after();
                    }

                });
            });
        });


        
    }
}
