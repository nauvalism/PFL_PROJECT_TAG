using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExitGames.Client.Photon;
using Photon.Realtime;
using Photon.Pun;

public class GPListener : MonoBehaviourPunCallbacks, IOnEventCallback
{
    const byte receivedData = 0;
    const byte allSpawned = 1;
    const byte distributeViews = 2;
    const byte distributeOwnership = 3;
    const byte anotherReconnect = 4; //CLIENT SEND
    const byte acknowledgeReconnect = 5; //MASTER REPLY
    const byte requestIDReconnect = 6; //CLIENT REQUEST
    const byte receiveIDReconnect = 7; // MASTER SEND
    const byte requestStateReconnect = 8; //CLIENT REQUEST
    const byte sendRequestedStateReconnect = 9; //MASTER SEND
    const byte applyingState = 10;
    const byte stateUpdated = 11;
    const byte masterResume = 12;

    public static GPListener instance {get;set;}

    

    public override void OnEnable()
    {
        base.OnEnable();
        PhotonNetwork.AddCallbackTarget(this);
    }

    public override void OnDisable()
    {
        base.OnDisable();
        PhotonNetwork.RemoveCallbackTarget(this);
    }


    private void Awake() {
        instance = this;
    }



    public void OnEvent(EventData photonEvent)
    {
        //Debug.Log("On Event Triggered "+photonEvent.ToStringFull());
        byte eventCode = photonEvent.Code;
        switch(eventCode)
        {
            
            #region CONNECTION CHECKS
            case receivedData:
                //Loger.Ler("CONNECTION CHECK");
                object[] _data = (object[])photonEvent.CustomData;
                object mainData = _data[0];
                PEntity p = new PEntity();
                //p = GameplayController.instance.ByteToEntity(mainData);

                //GameplayController.instance.PutPlayerCredentials()
                
                Debug.Log("Received Data : "+_data[0]);
                break;


            case allSpawned : 
                object[] _data1 = (object[])photonEvent.CustomData;
                object mainData1 = _data1[0];
                //int order1 = System.Convert.ToInt32(mainData1);
                string uID1 = (string)_data1[0];
                PEntity who = PhotonController.instance.Search(uID1);

                Debug.Log("Player "+who.GetOrder()+" Confirmed");


                PhotonController.instance.Confirm(who.GetOrder());

            break;

            case distributeViews : 
                object[] _data2 = (object[])photonEvent.CustomData;
                int[] mainData2 = (int[])_data2[0];
                List<int> vIds = new List<int>(mainData2);

                PhotonController.instance.ReceiveViews(vIds);
            break;


            case distributeOwnership :
                object[] _data3 = (object[])photonEvent.CustomData;
                int mainData3 = (int)_data3[0];
                
                PhotonController.instance.DistributeOwnership(mainData3);


                //PhotonController.instance.Confirm(mainData3, ActionList.Start_Game);
            break;

            #endregion
        }
    }

    public void RaiseEvent(byte code, ReceiverGroup toWho, EventCaching cache, object[] parameters)
    {
        Debug.Log("Event Raised");
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions
        {
            Receivers = toWho,
            CachingOption = cache
        };

        SendOptions sendOptions = new SendOptions
        {
            Reliability = true
        };

        PhotonNetwork.RaiseEvent(code, parameters, raiseEventOptions, sendOptions);
    }

    public void RaiseEvent(byte code, ReceiverGroup toWho, EventCaching cache, object parameters)
    {
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions
        {
            Receivers = toWho,
            CachingOption = cache
        };

        SendOptions sendOptions = new SendOptions
        {
            Reliability = true
        };

        PhotonNetwork.RaiseEvent(code, parameters, raiseEventOptions, sendOptions);
    }

    public void EnableAllControls()
    {
        if(PhotonNetwork.IsMasterClient)
            base.photonView.RPC("EnableAllControlsR", RpcTarget.AllViaServer, null);
    }

    [PunRPC]
    public void EnableAllControlsR()
    {
        GameplayController.instance.GameIntro();
    }

    public void GameIntro()
    {
        if(PhotonNetwork.IsMasterClient)
            base.photonView.RPC("GameIntroR", RpcTarget.AllViaServer, null);
    }

    [PunRPC]
    public void GameIntroR()
    {
        GameplayController.instance.IntroMultiplayer();
    }
}
