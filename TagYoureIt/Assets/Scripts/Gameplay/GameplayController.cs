using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayController : MonoBehaviour
{
    public static GameplayController instance {get;set;}
    [SerializeField] ScoreManager score;
    [SerializeField] LoopManager loop;
    [SerializeField] PlayerManager pm;
    [SerializeField] UIManager UI;
    [SerializeField] MusicManager music;
    [SerializeField] BaseMap currentMap;
    private void Awake() {
        instance = this;
    }

    private void Start() {
        
        //GachaBomb();
    }

    public PlayerManager GetPM()
    {
        return pm;
    }

    public void PlayMusic()
    {
        music.Play(0);
    }

    public void InitAllPlayers()
    {
        pm.InitAllPlayers();
    }

    public void ResetAllCoreAttribute()
    {
        pm.ResetCoreAttribute(2);
    }

    public void PutPlayerCredentials(List<PlayerProfile> profile, List<Identities> identity, List<CharacterSelectionData> csd)
    {
        pm.PutPlayerCredentials(profile, identity, csd);
    }

    public void PutPlayerCredentials(int order, PlayerProfile profile, Identities identity, CharacterSelectionData csd)
    {
        pm.PutPlayerCredentials(order, profile, identity, csd);
    }

    public void SpawnPlayersLocal()
    {
        pm.SpawnPlayersLocal();
    }

    public void GameIntro()
    {
        loop.Intro(()=>{
            SetState(GameState.midGame);
            GachaBomb();
        });
    }

    public bool Explode(PlayerIdentity who)
    {
        bool die = pm.DecreaseLive((int)who);
        if(die)
        {
            loop.ChangeGameState(GameState.calculating);
            music.FadeOut(()=>{
                loop.Resulting(GetOppositePlayer(who));
            }, 2.0f);
        }
        else
        {
            pm.GachaBomb();
        }

        return die;
    }



    public void FillUniqueEntity(int yourOrder)
    {
        this.pm.FillUniqueEntity(yourOrder);
    }

    public void FadeTaggable(System.Action during, System.Action after)
    {
        UI.FadeTag(during, after);
    }

    public void FadeUnTaggable(System.Action during, System.Action after)
    {
        UI.UnFadeTag(during, after);
    }

    public void UpdateLive(PlayerIdentity who, int live)
    {
        UI.UpdateLive(who, live);
    }

    public void GachaBomb()
    {
        pm.GachaBomb();
    }

    public PEntity GetPlayer(PlayerIdentity identity)
    {
        return pm.GetPlayer(identity);
    }

    public PEntity GetOppositePlayer(PlayerIdentity identity)
    {
        return pm.GetOppositePlayer(identity);
    }

    public bool IsYou(PEntity credential)
    {
        return pm.IsYou(credential);
    }

    public GameState SetState(GameState state)
    {
        loop.ChangeGameState(state);
        return loop.GetGameState();
    }

    public GameState GetState()
    {
        return loop.GetGameState();
    }

    public Transform GetSpawnPlace(int index)
    {
        return currentMap.GetSpawnPlace(index);
    }
}
