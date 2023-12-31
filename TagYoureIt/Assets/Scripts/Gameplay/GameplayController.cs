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
    private void Awake() {
        instance = this;
    }

    private void Start() {
        loop.Intro(()=>{
            GachaBomb();
        });
        //GachaBomb();
    }

    public PlayerManager GetPM()
    {
        return pm;
    }



    public void Explode(PlayerIdentity who)
    {
        if(pm.DecreaseLive((int)who))
        {
            loop.ChangeGameState(GameState.calculating);
        }
        else
        {
            pm.GachaBomb();
        }
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
}
