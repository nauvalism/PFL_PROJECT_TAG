using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] PlayerEntity yourEntity;
    [SerializeField] List<PEntity> players;
    [SerializeField] List<TopChar> allPlayersChars;
    [SerializeField] bool debug;
    
    private void OnValidate() {
        if(debug)
        {
            allPlayersChars = new List<TopChar>(FindObjectsOfType<TopChar>());
        }
    }


    private void Start() {
        if(debug)
        {
            InitAllPlayers();
        }    
    }

    public void FillUniqueEntity(PlayerEntity entity)
    {

    }

    public void InitAllPlayers()
    {
        for(int i = 0 ; i < allPlayersChars.Count ; i++)
        {
            allPlayersChars[i].Init(null, players[i]);
            GameplayController.instance.UpdateLive((PlayerIdentity)i, players[i].mainLives);
        }
    }



    public void GachaBomb()
    {
        int gc = Random.Range(0, allPlayersChars.Count);
        allPlayersChars[0].GiveBomb();
    }

    public void EnableAllPlayerControl()
    {
        for(int i = 0 ; i < allPlayersChars.Count ; i++)
        {
            allPlayersChars[i].EnableMovement();
        }
    }

    public void DisableAllPlayerControl()
    {
        for(int i = 0 ; i < allPlayersChars.Count ; i++)
        {
            allPlayersChars[i].DisableMovement();
        }
    }

    public bool DecreaseLive(int who)
    {
        return players[who].DecreaseLive();
    }

    public bool DecreaseLive(int who, int which)
    {
        return players[who].DecreaseLive(which);
    }


    public PEntity GetPlayer(int order)
    {
        return players[order];
    }

    public PEntity GetPlayer(PlayerIdentity identity)
    {
        for(int i = 0 ; i < players.Count ;  i++)
        {
            if(players[i].GetID() == identity)
            {
                return players[i];
            }
        }

        return null;
    }

    public bool IsYou(PEntity credential)
    {
        Debug.Log("Isyou ? : "+credential.GetID() + "-"+yourEntity.GetID());
        if(credential.identity.yourID == yourEntity.GetID())
        {
            return true;
        }
        return  false;
    }

    public BaseChar GetChar(int id)
    {
        PlayerIdentity _id = (PlayerIdentity)id;
        for(int i = 0 ; i < allPlayersChars.Count ; i++)
        {
            BaseChar res = allPlayersChars[i].Compare(_id);
            if(res != null)
            {
                return res;
            }
        }
        return null;
    }

}

[System.Serializable]
public class PlayerProfile
{
    public string userID;
    public string nickName;
    public int exp;
    public List<int> charExps;

    public PlayerProfile()
    {

    }

    public PlayerProfile(PlayerProfile copy)
    {
        this.userID = copy.userID;
        this.nickName = copy.nickName;
        this.exp = copy.exp;
        this.charExps = new List<int>(copy.charExps);
    }

}


[System.Serializable]
public class CharacterSelectionData
{
    public int chosenCharacterID;
    public List<int> chosenCharacterIDs;
}

[System.Serializable]
public class Identities
{
    public PlayerIdentity yourID;
    public int yourOrder;

    public Identities()
    {

    }

    public Identities(Identities copy)
    {
        this.yourID = copy.yourID;
        this.yourOrder = copy.yourOrder;
    }

    public void SetID(PlayerIdentity _id, int _order)
    {
        this.yourID = _id;
        this.yourOrder = _order;
    }

    
}

[System.Serializable]
public class PEntity
{
    public PlayerProfile profile;
    public Identities identity;
    public CharacterSelectionData csd;
    public int mainLives;
    public List<int> lives;

    public PEntity()
    {
        mainLives = 3;
        try
        {
            lives = new List<int>();
            for(int i = 0 ; i < csd.chosenCharacterIDs.Count ; i++)
            {
                lives.Add(3);
            }
        }
        catch(System.Exception e)
        {

        }
        
    }

    public bool DecreaseLive()
    {
        mainLives--;
        GameplayController.instance.UpdateLive(identity.yourID, mainLives);
        return (mainLives == 0);
    }

    public bool DecreaseLive(int which)
    {
        lives[which]--;
        return (lives[which] == 0);
    }

    public void SetID(PlayerIdentity _id, int _order)
    {
        identity = new Identities();
        identity.SetID(_id,_order);
    }

    public PlayerIdentity GetID()
    {
        return identity.yourID;
    }

    public int GetOrder()
    {
        return identity.yourOrder;
    }

    public Identities GetIdentity()
    {
        return identity;
    }

    public CharacterSelectionData GetCSD()
    {
        return csd;
    }
}
