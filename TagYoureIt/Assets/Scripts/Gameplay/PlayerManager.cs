using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;

public class PlayerManager : MonoBehaviourPunCallbacks
{
    [Header("Characters")]
    [SerializeField] List<GameObject> characters;

    [Header("Identities")]
    [SerializeField] PlayerEntity yourEntity;
    [SerializeField] List<PEntity> players;
    [SerializeField] List<TopChar> allPlayersChars;
    [SerializeField] bool debug;

    [Header("BOMB")]
    public int nextBomb = 0;
    
    private void OnValidate() {
        if(debug)
        {
            allPlayersChars = new List<TopChar>(FindObjectsOfType<TopChar>());
        }

        characters = new List<GameObject>(Resources.LoadAll<GameObject>("Prefabs/Characters/Chars"));
    }


    private void Start() {
        if(debug)
        {
            InitAllPlayers();
        }    
    }

    public void ResetCoreAttribute(int amount)
    {
        allPlayersChars = new List<TopChar>();
        players = new List<PEntity>();
        for(int i = 0 ; i < amount ; i++)
        {
            players.Add(new PEntity());
            allPlayersChars.Add(null);
        }

    }

    public void PutPlayerCredentials(List<PEntity> raw)
    {
        for(int i = 0 ; i < raw.Count ; i++)
        {
            players[i] = new PEntity();
            players[i] = raw[i];
        }

        //InitAllPlayers();
    }

    public void PutPlayerCredentials(List<PlayerProfile> profile, List<Identities> identity, List<CharacterSelectionData> csd)
    {
        for(int i = 0 ; i < profile.Count ; i++)
        {
            players[i].SetPlayerProfile(profile[i]);
            players[i].SetID(identity[i]);
            players[i].SetSelection(csd[i]);
        }

        InitAllPlayers();
    }

    public void PutPlayerCredentials(int order, PlayerProfile profile, Identities identity, CharacterSelectionData csd)
    {
        players[order].SetPlayerProfile(profile);
        players[order].SetID(identity);
        players[order].SetSelection(csd);
    }

    public void FillUniqueEntity(int yourOrder)
    {
        yourEntity = new PlayerEntity(players[yourOrder]);
    }

    public void SpawnPlayersLocal()
    {
        for(int i = 0 ; i < players.Count; i++)
        {
            GameObject ii;
            ii = (GameObject)Instantiate(characters[players[i].csd.chosenCharacterID], GameplayController.instance.GetSpawnPlace(players[i].identity.yourOrder).position, Quaternion.identity);
            allPlayersChars[i] = ii.GetComponent<TopChar>();
        }
    }

    public void InitAllPlayers()
    {
        for(int i = 0 ; i < allPlayersChars.Count ; i++)
        {
            allPlayersChars[i].Init(null, players[i]);
            GameplayController.instance.UpdateLive((PlayerIdentity)i, players[i].mainLives);
        }
    }

    public void AssignViewIds(List<int> vals)
    {
        List<BaseChar> cores = GetAllChars();
        for(int i = 0 ; i < cores.Count ; i++)
        {
            cores[i].GetComponent<PhotonView>().ViewID = vals[i];
        }
    }

    public void GachaBomb()
    {
        int gc = Random.Range(0, allPlayersChars.Count);
        allPlayersChars[0].GiveBomb();
    }

    public void GachaBomb(int who)
    {
        Debug.Log("Give Bomb To");
        allPlayersChars[who].GiveBomb();
    }

    public void GiveBomb(int who)
    {
        Debug.Log("Give Bomb to : "+(PlayerIdentity)who);
        GPListener.instance.RaiseEvent(5, ReceiverGroup.All, EventCaching.DoNotCache, who);
    }

    public void GachaBombMultiplayer(int exception = -1)
    {
        List<object> param = new List<object>();
        param.Add(players.Count);
        param.Add(exception);
        GPListener.instance.RaiseEvent(4, ReceiverGroup.MasterClient, EventCaching.DoNotCache, param.ToArray());
    }

    

    public void GachaBombMultiplayer(List<int> exceptions)
    {
        List<object> param = new List<object>();
        param.Add(players.Count);
        param.Add(exceptions.ToArray());
        GPListener.instance.RaiseEvent(4, ReceiverGroup.MasterClient, EventCaching.DoNotCache, param.ToArray());
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
        bool result = players[who].DecreaseLive();
        allPlayersChars[who].SetAlive(!result);
        return result;
    }

    public List<int> DecreaseLiveMultiplayer(int who)
    {
        players[who].DecreaseLive();
        List<int> result = new List<int>();
        for(int i = 0 ; i < players.Count ; i++)
        {
            result.Add(players[i].mainLives);
        }
        return result;
    }

    public bool DecreaseLive(int who, int which)
    {
        return players[who].DecreaseLive(which);
    }

    public PEntity GetYou()
    {
        return yourEntity.GetEntity();
    }

    public PEntity GetPlayer(int order)
    {
        return players[order];
    }

    public List<PEntity> GetPlayers()
    {
        return players;
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

    public PEntity GetOppositePlayer(PlayerIdentity identity)
    {
        for(int i = 0 ; i < players.Count ;  i++)
        {
            if(players[i].GetID() != identity)
            {
                return players[i];
            }
        }

        return null;
    }

    public bool IsYou(PEntity credential)
    {
        //Debug.Log("Isyou ? : "+credential.GetID() + "-"+yourEntity.GetID());
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

    public List<BaseChar> GetAllChars()
    {
        List<BaseChar> result = new List<BaseChar>();
        for(int i = 0 ; i < allPlayersChars.Count ; i++)
        {
            BaseChar res = allPlayersChars[i].GetCoreChar();
            result.Add(res);
        }

        return result;
    }

    public List<int> GetDead()
    {
        List<int> result = new List<int>();
        for(int i = 0 ; i < players.Count ; i++)
        {
            if(players[i].GetHP() <= 0)
            {
                result.Add(i);
            }
        }

        return result;
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
        mainLives = 1;
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

    public void SetPlayerProfile(PlayerProfile prof)
    {
        this.profile = new PlayerProfile(prof);
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

    public void SetID(Identities ide)
    {
        identity = new Identities(ide);
    }

    public void SetID(PlayerIdentity _id, int _order)
    {
        identity = new Identities();
        identity.SetID(_id,_order);
    }

    public void SetSelection(CharacterSelectionData _csd)
    {
        this.csd = new CharacterSelectionData();
        this.csd.chosenCharacterID = _csd.chosenCharacterID;
        
    }

    public int GetHP()
    {
        return mainLives;
    }

    public string GetUserID()
    {
        return profile.userID;
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

    public bool IsYou(string id)
    {
        if(profile.userID == id)
        {
            return true;
        }

        return false;
    }
}
