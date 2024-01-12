using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using System.Runtime.Serialization.Formatters.Binary;

public class PlayerEntity : MonoBehaviour
{
    [SerializeField] PEntity detailEntity;

    private void Awake() {
        
    }

    public PlayerEntity()
    {
        
    }

    public PlayerEntity(PEntity entity)
    {
        this.detailEntity = entity;
        this.detailEntity.SetID(entity.identity);
    }

    public PlayerEntity(PlayerProfile prof, Identities identity)
    {
        
    }

    public PEntity GetEntity()
    {
        return detailEntity;
    }

    public byte[] GetEntityByte()
    {
        BinaryFormatter BF = new BinaryFormatter();
        MemoryStream ms = new MemoryStream();
        BF.Serialize(ms, detailEntity);
        return ms.ToArray();
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


    public void SetDetailData(PlayerProfile prof)
    {
        detailEntity.profile = new PlayerProfile(prof);
    }

    public void SetID(Identities id)
    {
        detailEntity.identity = new Identities();
        detailEntity.identity.SetID(id.yourID,id.yourOrder);
    }

    public void SetID(PlayerIdentity _id, int _order)
    {
        detailEntity.identity = new Identities();
        detailEntity.identity.SetID(_id,_order);
    }

    public string GetNickName()
    {
        return detailEntity.profile.nickName;
    }

    public string GetUserID()
    {
        return detailEntity.profile.userID;
    }

    public PlayerIdentity GetID()
    {
        return detailEntity.identity.yourID;
    }

    public int GetOrder()
    {
        return detailEntity.identity.yourOrder;
    }

    public Identities GetIdentity()
    {
        return detailEntity.identity;
    }

    public CharacterSelectionData GetCSD()
    {
        return detailEntity.GetCSD();
    }

}



