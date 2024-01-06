using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ResultUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI whoWinsTxt;
    [SerializeField] string whoWins;
    [SerializeField] int charID;
    [SerializeField] List<GameObject> resultGOs;
    [SerializeField] GameObject toBeSpawned;
    [SerializeField] GameObject spawned;
    [SerializeField] Transform spawnTransform;
    [SerializeField] CanvasGroup cg;
    [SerializeField] CanvasGroup tapToContinue;
    
    private void OnValidate() {
        resultGOs = new List<GameObject>(Resources.LoadAll<GameObject>("Prefabs/Characters/Result"));
    }


    public void Resulting(int charID, string who)
    {
        cg.alpha = 0;
        cg.interactable = true;
        cg.blocksRaycasts = true;

        if(spawned != null)
        {
            Destroy(spawned);
        }

        this.whoWins = who;
        this.whoWinsTxt.text = who;
        toBeSpawned = Resources.Load<GameObject>("Prefabs/Characters/Result/WC-"+(charID+1));
        spawned = (GameObject)Instantiate(toBeSpawned, Vector3.zero, Quaternion.identity, spawnTransform);
        spawned.transform.localPosition = Vector3.zero;
        LeanTween.cancel(cg.gameObject);
        LeanTween.value(cg.gameObject, .0f, 1.0f, 1.0f).setOnUpdate((float f)=>{
            cg.alpha = f;
        }).setEase(LeanTweenType.easeOutQuad).setOnComplete(()=>{
            LeanTween.cancel(tapToContinue.gameObject);
            tapToContinue.interactable = true;
            tapToContinue.blocksRaycasts = true;
            LeanTween.value(tapToContinue.gameObject, .0f, 1.0f, 2.0f).setOnUpdate((float f)=>{
                	tapToContinue.alpha = f;
            }).setLoopType(LeanTweenType.pingPong);
        });
    }

    public void HideResult()
    {
        LeanTween.cancel(tapToContinue.gameObject);
        tapToContinue.interactable = false;
        tapToContinue.blocksRaycasts = false;
        tapToContinue.alpha = 0;

        LeanTween.cancel(cg.gameObject);
        LeanTween.value(cg.gameObject, 1.0f, .0f, 1.0f).setOnUpdate((float f)=>{
            cg.alpha = f;
        }).setEase(LeanTweenType.easeOutQuad).setOnComplete(()=>{
            	Destroy(spawned);
        });
    }
}
