using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExitGames.Client.Photon;
using Photon.Realtime;
using Photon.Pun;

public enum MoveState
{
    idle = -1,
    walking = 0,
    running = 1,
    jump = 2
}

public class CharAnimationManager : MonoBehaviour
{
    [SerializeField] BaseChar act;
    [SerializeField] Animator rootAnim;
    [SerializeField] bool move = false;
    [SerializeField] MoveState mState = MoveState.idle;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        if(!move)
        return;

        if(act.GetMovement().magnitude == 0)
        {
            
            if(act.GetGrounded())
            {
                ChangeMoveState(MoveState.idle);
                PlayMovementAnim("Idle");
            }
            
            
        }
        else
        {
            if(act.GetGrounded())
            {
                if(act.GetRunning())
                {
                    ChangeMoveState(MoveState.running);
                    PlayMovementAnim("Run");
                }
                else
                {
                    //Debug.Log("Wanna Walk");
                    ChangeMoveState(MoveState.walking);
                    PlayMovementAnim("Walk");
                }
            }
            
            
        }
    }

    public void PlayMovementAnim(string _name)
    {
        if(!isPlaying(_name))
        {
            rootAnim.Play(_name,0);
        }
        
    }

    public void PlayNonMovementAnim(string _name)
    {
        if(!isPlaying(_name))
        {
            rootAnim.Play(_name,0);
        }
        
    }

    public float PlayAnimAbsolutely(string _name)
    {
        move = false;
        rootAnim.Play(_name,-1,0);
        AnimatorClipInfo[] currInfo = rootAnim.GetCurrentAnimatorClipInfo(0);
        //Debug.Log("Animation duration : "+currInfo[0].clip.length);
        return currInfo[0].clip.length;
    
    }

    public void ChangeMoveState(MoveState _mState)
    {
        this.mState = _mState;
        //rootAnim.SetInteger("mov_state", (int)mState);
    }

    bool isPlaying(string stateName)
    {
        if (rootAnim.GetCurrentAnimatorStateInfo(0).IsName(stateName) &&
                rootAnim.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
            return true;
        else
            return false;
    }

    public void Move()
    {
        this.move = true;
    }

    public void UnMove()
    {
        this.move = false;
    }
}
