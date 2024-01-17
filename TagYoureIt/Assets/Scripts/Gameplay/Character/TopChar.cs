using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TopChar : MonoBehaviour
{
    
    [SerializeField] PEntity detailIdentity;
    [SerializeField] CharacterProfile characterAttributes;
    [SerializeField] BaseChar coreChar;
    [SerializeField] bool isYou = false;
    [SerializeField] bool alive = false;
    

    public void Init(PlayerProfile profile, PEntity entity)
    {
        
        detailIdentity = new PEntity();
        detailIdentity.identity = new Identities(entity.identity);
        detailIdentity.profile = new PlayerProfile(entity.profile);
        coreChar.InitializeIdentity(detailIdentity.identity.yourID, detailIdentity.identity.yourOrder);
        IsYou();
    }

    public void GiveBomb()
    {
        coreChar.ReceiveBomb();
    }

    public virtual void EnableMovement()
    {
        coreChar.EnableMovement();
    }

    public virtual void DisableMovement()
    {
        coreChar.DisableMovement();
    }

    public PlayerIdentity GetID()
    {
        return detailIdentity.GetID();
    }

    public int GetOrder()
    {
        return detailIdentity.GetOrder();
    }

    public PEntity GetFullID()
    {
        return detailIdentity;
    }

    public bool IsYou()
    {
        isYou = GameplayController.instance.IsYou(detailIdentity);
        return isYou;
    }

    public BaseChar Compare(PlayerIdentity iden)
    {
        if(iden == detailIdentity.GetID())
        {
            return coreChar;
        }
        return null;
    }

    public BaseChar GetCoreChar()
    {
        return coreChar;
    }

    public void SetAlive(bool t)
    {
        this.alive = t;
    }

    public bool GetAlive()
    {
        return alive;
    }
}
