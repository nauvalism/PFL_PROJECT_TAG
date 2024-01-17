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
    const byte gachaBomb = 4;
    const byte doGachaBomb = 5;
    const byte explode = 6;
    const byte explosionEffect = 7;
    const byte calculating = 8;
    const byte transferBomb = 9;
    const byte doTransferBomb = 10;
    

    

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

            case gachaBomb : 
                Debug.Log("Distributing Gacha Bomb");
                object[] _data4 = (object[])photonEvent.CustomData;
                int rangeData4_1 = (int)_data4[0];
                //int exception4 = (int)_data4[1];
                List<int> exception4 = new List<int>((int[])_data4[1]);
                int _result = Random.Range(0, rangeData4_1);
                
                
                if(exception4.Count > 0)
                {
                    while(ContainsInException())
                    {
                        _result = Random.Range(0, rangeData4_1);
                    }
                }


                bool ContainsInException()
                {
                    for(int i = 0 ; i < exception4.Count ; i++)
                    {
                        if(_result == exception4[i])
                        {
                            return true;
                        }
                    }

                    return false;
                }
                RaiseEvent(5, ReceiverGroup.All, EventCaching.DoNotCache, _result);
                //GameplayController.instance.GachaBomb(_result);
            break;


            case doGachaBomb : 
                int _data5 = (int)photonEvent.CustomData;
                GameplayController.instance.GachaBomb(_data5);
            break;

            case explode :
                
                object[] _data6 = (object[])photonEvent.CustomData;
                int who6 = (int)_data6[0];

                GameplayController.instance.SetState(GameState.processing);

                if(PhotonNetwork.IsMasterClient)
                {
                    List<int> updatedLives = new List<int>(GameplayController.instance.DecreaseHealth(who6));
                
                    int dead = SomeoneDied();

                    List<object> param = new List<object>();

                    param.Add(who6);
                    param.Add(dead);
                    param.Add(updatedLives.ToArray());
                    RaiseEvent(7, ReceiverGroup.All, EventCaching.DoNotCache, param.ToArray());

                

                    int SomeoneDied()
                    {
                        for(int i = 0 ; i < updatedLives.Count ;i++)
                        {
                            if(updatedLives[i] <= 0)
                            {
                                return i;
                            }
                        }

                        return -1;
                    }
                }

                

            break;

            case explosionEffect : 
                object[] _data7 = (object[])photonEvent.CustomData;
                int who7 = (int)_data7[0];
                int whoD7 = (int)_data7[1];
                int[] ulive = (int[])_data7[2];
                List<int> ulives = new List<int>(ulive);
                GameplayController.instance.UpdateLive(ulives);
                if(whoD7 != -1)
                {
                    GameplayController.instance.SetState(GameState.calculating);
                    GameplayController.instance.Explode((PlayerIdentity)who7, true);
                }
                else
                {
                    GameplayController.instance.SetState(GameState.midGame);
                    GameplayController.instance.Explode((PlayerIdentity)who7, false);
                    
                }

            break;

            case calculating : 
                
                int whoDead = (int)photonEvent.CustomData;
                int whoWin = GameplayController.instance.GetOppositePlayer((PlayerIdentity)whoDead).GetOrder();
                
                if(PhotonNetwork.IsMasterClient)
                {
                    List<object> paramEnd = new List<object>();
                    paramEnd.Add(whoWin);
                    GameplayController.instance.DoActions(ActionList.End_Game, paramEnd.ToArray());
                }

            break;

            case transferBomb : 
                object[] raw9 = (object[])photonEvent.CustomData;
                int from9 = (int)raw9[0];
                int to9 = (int)raw9[1];
                GameplayController.instance.SetState(GameState.processing);
                List<BaseChar> bc = GameplayController.instance.GetPlayerCharacters();

                List<object> param9 = new List<object>();
                param9.Add(from9);
                param9.Add(to9);

                if(PhotonNetwork.IsMasterClient)
                {
                    RaiseEvent(10, ReceiverGroup.All, EventCaching.DoNotCache, param9.ToArray());
                }

            break;

            case doTransferBomb : 
                object[] raw10 = (object[])photonEvent.CustomData;
                int from10 = (int)raw10[0];
                int to10 = (int)raw10[1];

                List<object> param10 = new List<object>();
                param10.Add(from10);
                param10.Add(to10);

                GameplayController.instance.SetState(GameState.midGame);
                GameplayController.instance.GetPlayerCharacter(from10).GiveBomb();

                Vector2 fromPos = GameplayController.instance.GetPlayerCharacter(from10).transform.position;

                GameplayController.instance.GetPlayerCharacter(to10).TransferredBomb(fromPos);
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

    public void DisableAllControls()
    {

    }

    [PunRPC]
    public void DisableAllControlsR()
    {
        GameplayController.instance.EnableAllControl();
    }

    public void EnableAllControls()
    {
        if(PhotonNetwork.IsMasterClient)
            base.photonView.RPC("EnableAllControlsR", RpcTarget.AllViaServer, null);
    }

    [PunRPC]
    public void EnableAllControlsR()
    {
        GameplayController.instance.EnableAllControl();
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
