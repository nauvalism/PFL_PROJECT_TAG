using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugGameplayUI : MonoBehaviour
{
    [SerializeField] GameObject mainDebugGameplay;
    [SerializeField] int you;
    [SerializeField] List<PlayerProfile> debugProfiles;
    [SerializeField] List<Identities> identities;
    [SerializeField] List<CharacterSelectionData> csds;
    
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

    public void StartQueue(int a)
    {
        // for(int i = 0 ; i < debugProfiles.Count ;i++)
        // {
        //     GameplayController.instance.PutPlayerCredentials(i, debugProfiles[i], identities[i], csds[i]);
        // }
        GameplayController.instance.PutPlayerCredentials(a, debugProfiles[a], identities[a], csds[a]);
        GameplayController.instance.FillUniqueEntity(you);

        mainDebugGameplay.SetActive(false);
    }
}
