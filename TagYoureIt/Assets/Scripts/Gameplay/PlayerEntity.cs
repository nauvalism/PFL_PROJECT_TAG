using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEntity : MonoBehaviour
{
    [SerializeField] PEntity detailEntity;
    [SerializeField] CharacterSelectionData csd;

    private void Awake() {
        
    }

    public void SetDetailData(PlayerProfile prof)
    {
        detailEntity.profile = new PlayerProfile(prof);
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

    public CharacterSelectionData GetCSD()
    {
        return csd;
    }
}



