using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DebugGameplayUI : MonoBehaviour
{
    
    [SerializeField] CanvasGroup mainCG;
    [SerializeField] CanvasGroup waitingForPlayerCG;
    [SerializeField] GameObject switchToLocalBtn;
    [SerializeField] GameObject mainDebugGameplay;
    [SerializeField] int you;
    [SerializeField] List<PlayerProfile> debugProfiles;
    [SerializeField] List<Identities> identities;
    [SerializeField] List<CharacterSelectionData> csds;
    [SerializeField] TextMeshProUGUI waitingInQueue;
    
    public void ShowUI()
    {
        LeanTween.cancel(mainCG.gameObject);
        mainCG.interactable = false;
        mainCG.blocksRaycasts = false;
        LeanTween.value(mainCG.gameObject, .0f, 1.0f, 1.0f).setOnUpdate((float f)=>{
            mainCG.alpha = f;
            
        }).setOnComplete(()=>{
            mainCG.interactable = true;
            mainCG.blocksRaycasts= true; 
        });
    }

    public void HideUI(float duration, System.Action then)
    {
        LeanTween.cancel(mainCG.gameObject);
        mainCG.interactable = false;
        mainCG.blocksRaycasts = false;
        LeanTween.value(mainCG.gameObject, 1.0f, .0f, duration).setOnUpdate((float f)=>{
            mainCG.alpha = f;
            
        }).setOnComplete(()=>{
            mainCG.interactable = false;
            mainCG.blocksRaycasts= false; 
            then();
        });
    }


    public void StartLocal(bool withAI)
    {
        HideUI(0.5f, ()=>{
            GameplayController.instance.ResetAllCoreAttribute();
            for(int i = 0 ; i < debugProfiles.Count ;i++)
            {
                GameplayController.instance.PutPlayerCredentials(i, debugProfiles[i], identities[i], csds[i]);
            }
            


            GameplayController.instance.SpawnPlayersLocal(withAI);
            GameplayController.instance.InitAllPlayers();
            GameplayController.instance.GameIntro();
            GameplayController.instance.PlayMusic();
            GameplayController.instance.FillUniqueEntity(you);
        });
        
        //mainDebugGameplay.SetActive(false);
    }

    public void UpdateInfo(int pil, int ril, int pidle)
    {
        //Debug.Log("info : "+pil+"_"+ril+"_"+pidle);
        string message = "Players currently playing is : "+pil.ToString();
        message += "\n Player waiting to play is : "+pidle.ToString();
    
        waitingInQueue.text = message;
    }

    public void UpdateInfo(int total, int pil, int ril, int pidle)
    {
        //Debug.Log("info : "+total+"_"+pil+"_"+ril+"_"+pidle);
        string message = "Players in server : "+pil.ToString();
        message += "\n Player waiting to play is : "+pidle.ToString();
    
        waitingInQueue.text = message;
    }

    public void UpdateInQueue()
    {

    }

    public void StartQueue()
    {
        // for(int i = 0 ; i < debugProfiles.Count ;i++)
        // {
        //     GameplayController.instance.PutPlayerCredentials(i, debugProfiles[i], identities[i], csds[i]);
        // }



        //GameplayController.instance.PutPlayerCredentials(a, debugProfiles[a], identities[a], csds[a]);
        //GameplayController.instance.FillUniqueEntity(you);
        HideUI(0.5f, ()=>{
            PhotonController.instance.JoinRandomRoom();
        });
        
        //mainDebugGameplay.SetActive(false);
    }
}
