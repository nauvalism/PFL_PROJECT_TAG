using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;


[CreateAssetMenu(fileName = "New Character", menuName = "Scriptables/Character", order = 1)]
public class CharacterProfile : BaseProfile
{
    public MovementAttributes moveStat;
    public JumpAttributes jumpStat;



    public void FillStatFromJSON(string json)
    {
        JSONNode _stat = JSON.Parse(json);
        JSONNode mStat = _stat["mv"];
        JSONNode jStat = _stat["jm"];
    }

    
}
