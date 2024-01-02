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
    private void Awake() {
        instance = this;
    }

    private void Start() {
        loop.Intro(()=>{
            SetState(GameState.midGame);
            GachaBomb();
        });
        //GachaBomb();
    }

    public PlayerManager GetPM()
    {
        return pm;
    }



    public bool Explode(PlayerIdentity who)
    {
        bool die = pm.DecreaseLive((int)who);
        if(die)
        {
            loop.ChangeGameState(GameState.calculating);
            music.FadeOut(null, 2.0f);
        }
        else
        {
            pm.GachaBomb();
        }

        return die;
    }



    public void FillUniqueEntity(PlayerEntity entity)
    {
        this.pm.FillUniqueEntity(entity);
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
}
