using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;
using UnityEngine.UI;
using TMPro;

public enum ActionList
{
    Start_Game = 0,
    End_Game = 1
}

public class PhotonController : MonoBehaviourPunCallbacks
{
    public static PhotonController instance {get;set;}
    [SerializeField] DebugGameplayUI debugUI;
    [SerializeField] PlayerEntity you;

    [Header("uis")]
    [SerializeField] TextMeshProUGUI statusTxt;
    [SerializeField] TextMeshProUGUI lobbyTxt;
    [SerializeField] TextMeshProUGUI roomTxt;
    [SerializeField] TextMeshProUGUI whovswho;

    [Header("PLAYERS IN ROOM")]
    [SerializeField] List<Player> players;
    [SerializeField] List<PEntity> receivedEntity;
    [SerializeField] List<bool> confirmation;
    [SerializeField] List<int> viewIds;
    // Start is called before the first frame update
    private void Awake() {
        instance = this;
        you = FindObjectOfType<PlayerEntity>();
    }

    private void Start() {
        //TryingToConnect();
    }

    private void Update() {
        statusTxt.text = PhotonNetwork.NetworkClientState.ToString();
        
        if(PhotonNetwork.InLobby)
        {
            lobbyTxt.text = "Lobby : " + PhotonNetwork.CurrentLobby.Name;
        }
        
        if(PhotonNetwork.InRoom)
        {
            roomTxt.text = "Room : "+ PhotonNetwork.CurrentRoom.Name;
            
        }
        
    }

    public void ReceiveCredential(byte[] raw)
    {

    }

    public void PutAllPlayerCredentials()
    {
        GameplayController.instance.ResetAllCoreAttribute();
        
        for(int i = 0 ; i < players.Count ;i++)
        {
            //GameplayController.instance.PutPlayerCredentials(i, debugProfiles[i], identities[i], csds[i]);
        }
    }

    public void TryingToConnect()
    {
        receivedEntity = new List<PEntity>();
        AuthenticationValues aval;

        aval = new AuthenticationValues(you.GetUserID());
        PhotonNetwork.AuthValues = aval;
        PhotonNetwork.NickName = you.GetNickName();
        
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnLobbyStatisticsUpdate(List<TypedLobbyInfo> lobbyStatistics)
    {
        base.OnLobbyStatisticsUpdate(lobbyStatistics);
        // for(int i = 0 ; i < lobbyStatistics.Count ; i++)
        // {
        //     Debug.Log("Lobby Name :"+lobbyStatistics[i].ToString());
            
        // }
        
        // int lobbyID = SearchLobby(PhotonNetwork.CurrentLobby.Name.ToLower());
        
        // int playersInlobby = lobbyStatistics[lobbyID].PlayerCount;
        // int roomInLobby = lobbyStatistics[lobbyID].RoomCount;
        // int playerIdle = lobbyStatistics[lobbyID].PlayerCount - PhotonNetwork.CountOfPlayersInRooms;
        // debugUI.UpdateInfo(playersInlobby, roomInLobby, playerIdle);
    
    
        // int SearchLobby(string lobbyName)
        // {
        //     for(int i = 0 ; i < lobbyStatistics.Count ; i++)
        //     {
        //         if(lobbyStatistics[i].Name.ToLower() == lobbyName)
        //         {
        //             return i;
        //         }
        //     }

        //     return 0;
        // }
    }

    public void JoinLobby()
    {
        
        TypedLobby lobbyProfile = new TypedLobby("OneVOneWithTheGreatOne", LobbyType.Default);
        PhotonNetwork.JoinLobby(lobbyProfile);
    }

    public void JoinRandomRoom()
    {
        PhotonNetwork.JoinRandomRoom();
    }

    public void CreateRoom()
    {
        Debug.Log("CREATING ROOM...");
        RoomOptions option = new RoomOptions();
        option.MaxPlayers = 2;
        option.PublishUserId = true;
        option.CleanupCacheOnLeave = false;
        option.EmptyRoomTtl = 0;
        option.PlayerTtl = 600000;
        string roomName = you.GetNickName()+"-"+you.GetUserID();
        PhotonNetwork.CreateRoom(roomName,option, PhotonNetwork.CurrentLobby);
    }





    public override void OnCreatedRoom()
    {
        
    }


    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("Join Random Room Failed .. creating room.. "+returnCode+"_"+message);
        CreateRoom();
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log("Join Room Failed .. creating room.. "+returnCode+"_"+message);
        base.OnJoinRoomFailed(returnCode, message);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Joined Room Successfully, "+PhotonNetwork.LocalPlayer.UserId);
        whovswho.text = "";
        if(PhotonNetwork.CurrentRoom != null)
        {
            
            foreach(Player p in PhotonNetwork.PlayerList)
            {
                whovswho.text += p.NickName;
                whovswho.text += " vs ";
            }
        
            
        }

        

        if(PhotonNetwork.PlayerList.Length == 2)
        {
            ResetConfirmation();
            GameplayController.instance.ResetAllCoreAttribute();
            //report player to gameplaycontroller
            players = new  List<Player> (PhotonNetwork.PlayerList);
            receivedEntity = new List<PEntity>();
            for(int i = 0; i < PhotonNetwork.CurrentRoom.PlayerCount ; i++)
            {
                receivedEntity.Add(null);
            }

            for(int i = 0 ; i < PhotonNetwork.CurrentRoom.PlayerCount; i++)
            {
                var raw = PhotonNetwork.PlayerList[i].CustomProperties["AllData"];
                byte[] playerData = (byte[]) raw;
                receivedEntity[i] = (PEntity)ByteConverter.ByteArrayToObject(playerData);
            
                if(receivedEntity[i].IsYou(you.GetUserID()))
                {
                    you.SetID((PlayerIdentity)i, i);
                    
                }
                receivedEntity[i].SetID((PlayerIdentity)i, i);
            
            }


            GameplayController.instance.PutPlayerCredentials(receivedEntity);
            

            //spawn everything
            GameplayController.instance.SpawnPlayersLocal(false, true);
            
            
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);
        Debug.Log("New Player Arrived !! "+newPlayer.UserId + "_"+newPlayer.NickName);
        whovswho.text = "";


        

        
        
        if(PhotonNetwork.CurrentRoom != null)
        {
            
            foreach(Player p in PhotonNetwork.PlayerList)
            {
                
                whovswho.text += p.NickName;
                whovswho.text += " vs ";
            }
        
            
        }


        if(PhotonNetwork.PlayerList.Length == 2)
        {
            ResetConfirmation();
            GameplayController.instance.ResetAllCoreAttribute();
            //spawn everything'
            
            players = new  List<Player> (PhotonNetwork.PlayerList);
            receivedEntity = new List<PEntity>();
            for(int i = 0; i < PhotonNetwork.CurrentRoom.PlayerCount ; i++)
            {
                receivedEntity.Add(null);
            }

            for(int i = 0 ; i < PhotonNetwork.CurrentRoom.PlayerCount; i++)
            {
                var raw = PhotonNetwork.PlayerList[i].CustomProperties["AllData"];
                byte[] playerData = (byte[]) raw;
                receivedEntity[i] = (PEntity)ByteConverter.ByteArrayToObject(playerData);
            
                if(receivedEntity[i].IsYou(you.GetUserID()))
                {
                    you.SetID((PlayerIdentity)i, i);
                    
                }

                receivedEntity[i].SetID((PlayerIdentity)i, i);
            }

            GameplayController.instance.PutPlayerCredentials(receivedEntity);
            

            //spawn everything
            GameplayController.instance.SpawnPlayersLocal(false, true);
        }
    }

    

    public void AllocateViewIdsFromServer()
    {
        Debug.Log("Allocating View Ids");
        viewIds = new List<int>();

        for(int i = 0 ; i < receivedEntity.Count; i++)
        {
            viewIds.Add(-1);
        }


        List<BaseChar> allChar = GameplayController.instance.GetPlayerCharacters();
        for(int i = 0 ; i < allChar.Count ; i++)
        {
            if(PhotonNetwork.AllocateViewID(allChar[i].GetComponent<PhotonView>()))
            {
                viewIds[i] = (allChar[i].GetComponent<PhotonView>().ViewID);
            }
        }

        StartCoroutine(WaitAndSend());



        bool AllViewIDZero(List<int> vid)
        {
            for(int i = 0 ; i < viewIds.Count ; i++)
            {
                if(vid[i] == -1)
                return false;
            }
            return true;
        }


        IEnumerator WaitAndSend()
        {
            while(AllViewIDZero(viewIds) == false)
            {
                yield return null;
            }

            SendViewsToOthers();
        }

        void SendViewsToOthers()
        {
            List<object> param = new List<object>();
            param.Add(viewIds.ToArray());
            ResetConfirmation();
            confirmation[you.GetOrder()] = true;
            GPListener.instance.RaiseEvent(2, ReceiverGroup.Others, EventCaching.DoNotCache, param.ToArray());
        }
    }

    public void ReceiveViews(List<int> vids)
    {
        viewIds = new List<int>(vids);
        DistributeViewIDs();
        List<object> param = new List<object>();
        param.Add(you.GetOrder());
        GPListener.instance.RaiseEvent(3, ReceiverGroup.MasterClient, EventCaching.DoNotCache, param.ToArray());


        void DistributeViewIDs()
        {
            GameplayController.instance.AssignViewIds(vids);
        }


    }

    public void DistributeOwnership(int who)
    {
        Debug.Log("Distributing Ownership");
        List<BaseChar> chars = GameplayController.instance.GetPM().GetAllChars();
        chars[who].GetComponent<PhotonView>().TransferOwnership((who + 1));
        Confirm(who, ActionList.Start_Game);
    }

    public override void OnJoinedLobby()
    {
        base.OnJoinedLobby();
        InvokeRepeating("RefreshRoomInfo",0, 1.0f);

        
        

        debugUI.ShowUI();
        
        

    }

    public void RefreshRoomInfo()
    {
        int pil = PhotonNetwork.CountOfPlayers;
        int playersWaiting = PhotonNetwork.CountOfPlayersInRooms % 2;
        int ril = PhotonNetwork.CountOfRooms;

        debugUI.UpdateInfo(pil, ril, playersWaiting);
    }



    public override void OnConnectedToMaster()
    {
        
        Debug.Log("Connected to Master Server !!");
        FillCredentialAndJoinLobby();
        base.OnConnectedToMaster();
    }

    public void FillCredentialAndJoinLobby()
    {
        Debug.Log("Filling Credentials and Joining Lobby");
        PhotonNetwork.LocalPlayer.CustomProperties["SelectionData"] = ByteConverter.ObjectToByteArray(you.GetCSD());
        PhotonNetwork.LocalPlayer.CustomProperties["AllData"] = ByteConverter.ObjectToByteArray(you.GetEntity());
        JoinLobby();
    }

    void ResetConfirmation()
    {
        confirmation = new List<bool>();
        for(int i = 0 ; i < PhotonNetwork.PlayerList.Length ; i++)
        {
            confirmation.Add(false);
        }
    }

    public void Confirm(int order, ActionList actions)
    {
        this.confirmation[order] = true;

        if(AllConfirmed())
        {
            if(PhotonNetwork.IsMasterClient)
                GameplayController.instance.DoActions(actions);
        }
        
    }

    public void Confirm(int order)
    {
        this.confirmation[order] = true;

        if(AllConfirmed())
        {
            if(PhotonNetwork.IsMasterClient)
            {
                AllocateViewIdsFromServer();
            }
        }
    }

    public void Confirm(string id)
    {
        PEntity who = Search(id);
        this.confirmation[who.GetOrder()] = true;

        if(AllConfirmed())
        {
            if(PhotonNetwork.IsMasterClient)
            {
                AllocateViewIdsFromServer();
            }
        }
    }

    public PEntity Search(string id)
    {
        for(int i = 0 ; i < receivedEntity.Count ; i++)
        {
            if(receivedEntity[i].GetUserID() == id)
            {
                return receivedEntity[i];
            }
        }

        return null;
    }

    public bool AllConfirmed()
    {
        for(int i = 0 ; i < confirmation.Count; i++)
        {
            if(confirmation[i] == false)
            return false;
        }

        

        return true;
    }

    


    public override void OnLeftRoom()
    {
        base.OnLeftRoom();
        Debug.Log("You've left the room");
        GameplayController.instance.DestroyAll();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        base.OnPlayerLeftRoom(otherPlayer);
        Debug.Log(otherPlayer.NickName+" has left the room");
        
    }







    
}
