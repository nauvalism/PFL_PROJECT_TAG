using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using Photon.Pun;
using Photon.Realtime;

public class GameplayController : MonoBehaviour
{
    public static GameplayController instance {get;set;}
    
    [Header("Debug UI")]
    [SerializeField] DebugGameplayUI debugUI;

    [Header("Main Attributes")]
    [SerializeField] ScoreManager score;
    [SerializeField] LoopManager loop;
    [SerializeField] PlayerManager pm;
    [SerializeField] UIManager UI;
    [SerializeField] MusicManager music;
    [SerializeField] BaseMap currentMap;
    public bool isMultiplayer;
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

    public void ResetGameplay()
    {
        if(isMultiplayer)
        {
            PhotonNetwork.LeaveLobby();
            PhotonNetwork.LeaveRoom();
        }
        else
        {
            DestroyAll();
        }
    }

    public void DestroyAll(bool resetting = true)
    {
        
        StartCoroutine(DestroyingAll());
        IEnumerator DestroyingAll()
        {
            List<BaseChar> allPlayers = pm.GetAllChars();
            for(int i = 0 ; i < allPlayers.Count ; i++)
            {
                TopChar t = allPlayers[i].GetRoot();
                if(isMultiplayer)
                    PhotonNetwork.Destroy(t.gameObject);
                else
                    Destroy(t.gameObject);
            }

            if(resetting)
            {
                if(isMultiplayer)
                {
                    yield return new WaitForSeconds(1.0f);
                    //PhotonController.instance.FillCredentialAndJoinLobby();
                }
                else
                {
                    debugUI.ShowUI();
                }
            }
        }

        
    }

    public bool SetLocalIDPlayer(string nickName)
    {
        pm.SetYou(nickName);
        return true;
    }
    
    public void PutPlayerCredentials(List<PEntity> raws)
    {
        pm.PutPlayerCredentials(raws);
    }

    public void PutPlayerCredentials(List<PlayerProfile> profile, List<Identities> identity, List<CharacterSelectionData> csd)
    {
        pm.PutPlayerCredentials(profile, identity, csd);
    }

    public void PutPlayerCredentials(int order, PlayerProfile profile, Identities identity, CharacterSelectionData csd)
    {
        pm.PutPlayerCredentials(order, profile, identity, csd);
    }

    public void AssignViewIds(List<int> vids)
    {
        pm.AssignViewIds(vids);
    }

    public void SpawnPlayersLocal(bool multi = false)
    {
        pm.SpawnPlayersLocal();
        if(multi)
        {
            List<object> param = new List<object>();
            param.Add(pm.GetYou().GetUserID());
            GPListener.instance.RaiseEvent(1, Photon.Realtime.ReceiverGroup.MasterClient, Photon.Realtime.EventCaching.DoNotCache, param.ToArray());
        }
    }

    #region ACTIONS

    public void DoActions(ActionList actionList, object[] param = null)
    {
        switch(actionList)
        {
            case ActionList.Start_Game : 
                GPListener.instance.GameIntro();
            break;

            case ActionList.End_Game : 
                int _whoWins = (int)param[0];
                loop.Resulting(pm.GetPlayer(_whoWins));
            break;
        }
    }

    #endregion

    public void GameIntro()
    {
        loop.Intro(()=>{
            pm.InitAllPlayers();
            EnableAllControl();
        });
    }

    public void IntroMultiplayer()
    {
        SetMultiplayer(true);
        pm.InitAllPlayers();
        loop.Intro(()=>{
            GPListener.instance.EnableAllControls();
        });
    }

    public void EnableAllControl()
    {
        SetState(GameState.midGame);
        //pm.GachaBombMultiplayer();
        if(isMultiplayer)
        {
            if(PhotonNetwork.IsMasterClient)
                GachaBombMultiplayer();
        }
        else
        {
            GachaBomb();
        }
        //GachaBomb();
    }

    public void DisableAllControl()
    {
        SetState(GameState.calculating);
    }

    public void GachaBombMultiplayer()
    {
        List<PEntity> p = pm.GetPlayers();
        List<int> exceptions = new List<int>();
        for(int i = 0 ; i < pm.GetPlayers().Count; i++)
        {
            if(p[i].GetHP() <= 0)
            {
                exceptions.Add(i);
            }
        }


        int whoBomb = UnityEngine.Random.Range(0,p.Count);

        if(exceptions.Count > 0)
        {
            while(ContainsInException())
            {
                whoBomb = UnityEngine.Random.Range(0,p.Count);
            }
        }

        pm.GiveBomb(whoBomb);

        bool ContainsInException()
            {
                for(int i = 0 ; i < exceptions.Count ; i++)
                {
                    if(whoBomb == exceptions[i])
                    {
                        return true;
                    }
                }

                return false;
            }

    }

    public List<int> DecreaseHealth(int who)
    {
        return pm.DecreaseLiveMultiplayer(who);
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

    public void Explode(PlayerIdentity who, bool dead, bool multiplayer = false)
    {
        pm.GetChar((int)who).ExplodeEffect(dead);
        if(dead)
        {
            DisableAllControl();
            loop.ChangeGameState(GameState.calculating);
            music.FadeOut(()=>{
                if(multiplayer)
                {
                    if(PhotonNetwork.IsMasterClient)
                    {
                        GPListener.instance.RaiseEvent(8, ReceiverGroup.MasterClient, EventCaching.DoNotCache, (int)who);
                    }
                }
                else
                {
                    loop.Resulting(GetOppositePlayer(who));
                }
                
            }, 2.0f);
        }
        else
        {

            StartCoroutine(GachaNext());
            IEnumerator GachaNext()
            {
                yield return new WaitForSeconds(1.0f);
                List<int> exception = new List<int>(pm.GetDead());
                List<object> param = new List<object>();
                param.Add(pm.GetPlayers().Count);
                param.Add(exception.ToArray());
                if(PhotonNetwork.IsMasterClient)
                {
                    GPListener.instance.RaiseEvent(4, ReceiverGroup.MasterClient, EventCaching.DoNotCache, param.ToArray());
                }
                
            }
            
        }
        
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

    public void UpdateLive()
    {
        List<PEntity> p = pm.GetPlayers();
        for(int i = 0 ; i < p.Count ; i++)
        {
            UI.UpdateLive((PlayerIdentity)i, p[i].GetHP());
        }
    }

    public void UpdateLive(PlayerIdentity who, int live)
    {
        UI.UpdateLive(who, live);
    }

    public void UpdateLive(List<int> lives)
    {
        for(int i = 0 ; i < lives.Count ; i++)
        {
            UI.UpdateLive((PlayerIdentity)i, lives[i]);
        }
    }

    public void GachaBomb()
    {
        pm.GachaBomb();
    }

    public void GachaBomb(int who)
    {
        pm.GachaBomb(who);
    }

    public BaseChar GetPlayerCharacter(int who)
    {
        return pm.GetChar(who);
    }

    public List<BaseChar> GetPlayerCharacters()
    {
        return pm.GetAllChars();
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


    public void SetMultiplayer(bool stat)
    {
        this.isMultiplayer = stat;
    }

    public bool IsMultiplayer()
    {
        return this.isMultiplayer;
    }



    public PEntity ByteToEntity(byte[] Data)
    {
        PEntity result = new PEntity();
        
        if (Data != null && Data.Length > 0)
        {
            BinaryFormatter BF = new BinaryFormatter();
            MemoryStream ms = new MemoryStream();
            ms.Write(Data, 0, Data.Length);
            ms.Seek(0, SeekOrigin.Begin);
            result = (PEntity)BF.Deserialize(ms);
            return result;
        }
        else
        {
            //SelectChar(0);
            //GPGHelper.Instance.OpenSave (true);
            //MenuController.Instance.FillDatas (true);
            return result;
        }
    }
}
