using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEntity : MonoBehaviour
{
    [SerializeField] Identities yourID;
    [SerializeField] PEntity detailEntity;

    private void Awake() {
        
    }

    public PlayerEntity()
    {
        
    }

    public PlayerEntity(PEntity entity)
    {
        this.detailEntity = entity;
        this.yourID = entity.identity;
    }

    public PlayerEntity(PlayerProfile prof, Identities identity)
    {
        
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

}



