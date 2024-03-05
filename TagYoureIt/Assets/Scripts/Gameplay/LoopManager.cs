using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
    initializing = 0,
    preStart = 1,
    opening = 2,
    midGame = 3,
    processing = 4,
    calculating = 5,
    result = 6,
    endGame = 7
}

public class LoopManager : MonoBehaviour
{
    [SerializeField] GameState gameState;
    [SerializeField] OpeningUI openingUI;
    [SerializeField] ResultUI result;
    
    

    public void Intro(System.Action next, bool instant = false)
    {
        ChangeGameState(GameState.opening);
        openingUI.OpeningUISequence(next, instant);
    }

    public void Resulting(PEntity whoWins)
    {
        result.Resulting(whoWins.GetCSD().chosenCharacterID, whoWins.profile.nickName);
    }

    public void ChangeGameState(GameState to)
    {
        this.gameState = to;
    }


    public GameState GetGameState()
    {
        return gameState;
    }

    public void Opening()
    {

    }
}
