using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugGameplayUI : MonoBehaviour
{
    [SerializeField] CanvasGroup mainCG;
    [SerializeField] GameObject mainDebugGameplay;
    [SerializeField] int you;
    [SerializeField] List<PlayerProfile> debugProfiles;
    [SerializeField] List<Identities> identities;
    [SerializeField] List<CharacterSelectionData> csds;
    
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

    public void HideUI()
    {
        LeanTween.cancel(mainCG.gameObject);
        mainCG.interactable = true;
        mainCG.blocksRaycasts = true;
        LeanTween.value(mainCG.gameObject, 1.0f, .0f, 1.0f).setOnUpdate((float f)=>{
            mainCG.alpha = f;
            
        }).setOnComplete(()=>{
            mainCG.interactable = false;
            mainCG.blocksRaycasts= false; 
        });
    }


    public void StartLocal()
    {
        GameplayController.instance.ResetAllCoreAttribute();
        for(int i = 0 ; i < debugProfiles.Count ;i++)
        {
            GameplayController.instance.PutPlayerCredentials(i, debugProfiles[i], identities[i], csds[i]);
        }
        
        GameplayController.instance.SpawnPlayersLocal();
        GameplayController.instance.InitAllPlayers();
        GameplayController.instance.GameIntro();
        GameplayController.instance.PlayMusic();
        GameplayController.instance.FillUniqueEntity(you);
        mainDebugGameplay.SetActive(false);
    }

    public void StartQueue()
    {
        // for(int i = 0 ; i < debugProfiles.Count ;i++)
        // {
        //     GameplayController.instance.PutPlayerCredentials(i, debugProfiles[i], identities[i], csds[i]);
        // }



        //GameplayController.instance.PutPlayerCredentials(a, debugProfiles[a], identities[a], csds[a]);
        //GameplayController.instance.FillUniqueEntity(you);
        PhotonController.instance.JoinRandomRoom();
        mainDebugGameplay.SetActive(false);
    }
}
