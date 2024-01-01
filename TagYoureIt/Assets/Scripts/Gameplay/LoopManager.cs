using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
    initializing = 0,
    preStart = 1,
    opening = 2,
    midGame = 3,
    calculating = 4,
    result = 5,
    endGame = 6
}

public class LoopManager : MonoBehaviour
{
    [SerializeField] GameState gameState;
    [SerializeField] OpeningUI openingUI;
    
    

    public void Intro(System.Action next)
    {
        ChangeGameState(GameState.opening);
        openingUI.OpeningUISequence(next);
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
