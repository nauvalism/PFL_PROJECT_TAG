using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;
using TMPro;
using ExitGames.Client.Photon;
using Photon.Realtime;
using Photon.Pun;

public enum PlayerIdentity
{
    player_1 = 0,
    player_2 = 1,
    player_3 = 2,
    player_4 = 3
}

public class BaseChar : MonoBehaviourPunCallbacks, IPunObservable
{
    [Header("IDENTITY")]
    [SerializeField] TopChar root;
    [SerializeField] CharacterProfile stat;
    [SerializeField] PInputKeys keys;
    [SerializeField] CharBomb bomb;
    [SerializeField] bool AIOverride = false;
    [SerializeField] float stunModifier = 1;


    [Header("Movement")]
    [SerializeField] Vector2 movement;
    [SerializeField] Transform rootActorMovement;
    [SerializeField] Transform graphicRoot;
    [SerializeField] Rigidbody2D rb;
    [SerializeField] PhysicsMaterial2D bounceMat;
    [SerializeField] Collider2D col;
    [SerializeField] MovementAttributes movementAttributes;
    
    [Header("Ground and Jump")]
    [SerializeField] bool grounded = true;
    [SerializeField] bool launched = false;
    [SerializeField] GameObject groundCheckRoot;
    [SerializeField] Transform groundCheckF;
    [SerializeField] Transform groundCheck;
    [SerializeField] JumpAttributes jump;
    
    [Header("Animation")]
    [SerializeField] CharAnimationManager animManager;

    [Header("Bubbles")]
    [SerializeField] List<GameObject> bubbles;
    [SerializeField] Transform bubbleRoot;
    
    [Header("Extras")]    
    [SerializeField] BaseCharSkill charSkill;
    [SerializeField] InteractionSensor iSensor;
    [SerializeField] Explosion explo;
    [SerializeField] SoundManager impactSounds;
    
    
    public virtual void InitializeIdentity(PlayerIdentity iden, int _order)
    {
        
        keys = new PInputKeys();
        keys.SetIdentity(iden);

    }

    protected virtual void OnValidate()
    {

    }

    protected virtual void Awake() {
        this.iSensor.IgnoreCol(this.col);
    }

    // Start is called before the first frame update
    protected virtual void Start()
    {
        
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if(GameplayController.instance.GetState() != GameState.midGame)
        {
            return;
        }


        if(!root.GetAlive())
        return;
        
        if(movementAttributes.controllable)
        {
            if(GameplayController.instance.IsMultiplayer())
            {
                if(base.photonView.IsMine)
                {
                    movement.x = Input.GetAxisRaw(keys.moveKey);
                    movement.y = Input.GetAxisRaw(keys.jumpkey);
                }
            }
            else
            {
                movement.x = Input.GetAxisRaw(keys.moveKey);
                movement.y = Input.GetAxisRaw(keys.jumpkey);
            }
            
        
            //if(Input.GetKeyDown(KeyCode.LeftShift))
            if(Input.GetButtonDown(keys.run))
            {
                movementAttributes.running = true;
            }

            //if(Input.GetKeyUp(KeyCode.LeftShift))
            if(Input.GetButtonUp(keys.run))
            {
                movementAttributes.running = false;
            }

            //if(Input.GetButtonDown("Jump"))
            if(Input.GetButtonDown(keys.jumpkey))
            {
                Jump();
            }

            //if(Input.GetButtonDown("Take1"))
            if(Input.GetButtonDown(keys.take))
            {
                iSensor.Trigger();
            }

            if(Input.GetButtonDown(keys.tag))
            {
                Tag();
            }

            


        }

        if(movement.x > 0)
        {
            graphicRoot.transform.localScale = new Vector3(1.0f, graphicRoot.localScale.y, 0);
        }
        else if(movement.x < 0)
        {
            graphicRoot.transform.localScale = new Vector3(-1.0f, graphicRoot.localScale.y, 0);
        }
    }

    private void FixedUpdate() {

        if(!root.GetAlive())
        return;

        grounded = Physics2D.Linecast(groundCheckF.position, groundCheck.position, 1 << LayerMask.NameToLayer("Ground"));
    
        Vector2 moveDirection = new Vector2(movement.x, 0).normalized;
        if(GameplayController.instance.IsMultiplayer())
        {
            if(base.photonView.IsMine)
            {
                rootActorMovement.Translate(((movementAttributes.speed + (movementAttributes.runSpeedAddition * System.Convert.ToInt32(movementAttributes.running)))/movementAttributes.runSpeedSlow * Time.deltaTime * moveDirection) * stunModifier);
            }
        }
        else
        {
            rootActorMovement.Translate(((movementAttributes.speed + (movementAttributes.runSpeedAddition * System.Convert.ToInt32(movementAttributes.running)))/movementAttributes.runSpeedSlow * Time.deltaTime * moveDirection) * stunModifier);
        }
    
        if (!grounded)
        {
            if(!jump.land)
            {
                if (rb.velocity.y < (-jump.jumpHeavyPeak))
                {
                    jump.land = true;
                    rb.gravityScale = jump.defaultGravityScale * jump.jumpHeavyMultiplier;
                    animManager.PlayNonMovementAnim("Fall");
                }
                else
                {

                }
            }
        }
        else
        {
            if(jump.land)
            {
                jump.land = false;
                jump.jump = false;
                rb.gravityScale = jump.defaultGravityScale;
                rb.sharedMaterial = null;
                StartCoroutine(Land());
            }
            else
            {
                if(jump.jump)
                {

                }
            }
            
        }

        // if(rb.velocity.magnitude < 0.25f)
        // {
        //     rb.sharedMaterial = null;
        // }
    }

    IEnumerator Land()
    {
        float _w = animManager.PlayAnimAbsolutely("Ground");
        yield return new WaitForSeconds(_w);
        animManager.Move();
    }

    public virtual void Jump()
    {
        if (!grounded)
            return;
        
        jump.jump = true;
        animManager.UnMove();
        rb.velocity = new Vector3(rb.velocity.x, jump.jumpForce);
        animManager.PlayNonMovementAnim("Jump");
    }

    public Vector2 GetMovement()
    {
        return movement;
    }

    public bool GetRunning()
    {
        return movementAttributes.running;
    }
    
    public bool GetGrounded()
    {
        return grounded;
    }

    public bool IsMidAir()
    {
        return jump.jump;
    }

    public void ShowBubble(int id)
    {
        LeanTween.cancel(bubbleRoot.gameObject);
        bubbleRoot.transform.localScale = Vector3.zero;
        HideAllbubbles();
        bubbles[id].SetActive(true);
        LeanTween.scale(bubbleRoot.gameObject, Vector3.one, 0.25f).setEase(LeanTweenType.easeOutBack);
    }

    public void HideBubble()
    {
        LeanTween.cancel(bubbleRoot.gameObject);
        LeanTween.scale(bubbleRoot.gameObject, Vector3.zero, 0.125f).setEase(LeanTweenType.easeOutQuad);
    }

    void HideAllbubbles()
    {
        for(int i = 0 ; i< bubbles.Count ; i++)
        {
            bubbles[i].SetActive(false);
        }
    }


    public virtual void TriggerSkill(int id)
    {
        
    }


    public virtual void TakeItem()
    {

    }



    public virtual void Tag()
    {
        Debug.Log("Tag Button");
        if(iSensor.GetInteractedChar() != null)
        {
            Debug.Log("Tag Button not null");
            BaseChar ch = iSensor.GetInteractedChar();
            
            if(GameplayController.instance.isMultiplayer)
            {
                List<object> param = new List<object>();
                param.Add(root.GetOrder());
                param.Add(ch.GetRoot().GetOrder());
                GPListener.instance.RaiseEvent(9, ReceiverGroup.MasterClient, EventCaching.DoNotCache, param.ToArray());
            }
            else
            {
                TransferBombLocal((int)ch.GetID());
            }
            
            
        }
        else
        {
            Debug.Log("Tag Button null");
        }
    }

    public virtual void ReceiveBomb()
    {
        iSensor.NullifyInteractedChar();
        bomb.ReceiveBomb();
    }

    public virtual void GiveBomb()
    {
        iSensor.NullifyInteractedChar();
        bomb.GiveBomb();
    }

    public virtual void TransferredBomb(Vector3 fromPos)
    {
        iSensor.NullifyInteractedChar();
        Vector2 _force;
        Vector2 _dir =  GetPosition() - (Vector2)fromPos;
        _dir.Normalize();
        _force = new Vector2(_dir.x * 10, 10);
        _force.Normalize();
        ReceiveBomb();
        Hit();
        Launch(_force, 250);
    }

    public virtual void TransferBombLocal(int who)
    {
        

        
        BaseChar bc = GameplayController.instance.GetPM().GetChar(who);
        Vector2 _force;
        Vector2 _dir = bc.GetPosition() - (Vector2)rootActorMovement.position;
        _dir.Normalize();
        _force = new Vector2(_dir.x * 10, 10);
        _force.Normalize();
        bc.Launch(_force, 250);
        bomb.GiveBomb();
        bc.ReceiveBomb();
        bc.Hit();
        iSensor.NullifyInteractedChar();
        //animManager.PlayAnimAbsolutely("Hit");
    }

    public virtual void TransferBomb(BaseChar bc)
    {
        
        
        
        Vector2 _force;
        Vector2 _dir = bc.GetPosition() - (Vector2)rootActorMovement.position;
        _dir.Normalize();
        _force = new Vector2(_dir.x * 10, 0);
        bc.Launch(_force);
    }

    public virtual void LaunchTarget(int who)
    {
        
        BaseChar bc = GameplayController.instance.GetPM().GetChar(who);
        Vector2 _force;
        Vector2 _dir = bc.GetPosition() - (Vector2)rootActorMovement.position;
        _dir.Normalize();
        _force = new Vector2(_dir.x * 10, 0);
        bc.Launch(_force);
    }

    public virtual void Launch(Vector2 _f)
    {
        Debug.Log("LAUNCH !!");
        Stunned();
        rb.sharedMaterial = bounceMat;
        rb.AddForce(_f * 75);
        iSensor.DisableCol();
        launched = true;
        StopCoroutine("Recovery");
        StartCoroutine(Recovery());
        // LeanTween.value(rootActorMovement.gameObject, rb.velocity.x, .0f, 5.0f).setEase(LeanTweenType.easeOutQuad).setOnUpdate((float f)=>{
        //     rb.velocity = new Vector2(f, rb.velocity.y);
        // });

        IEnumerator Recovery()
        {
            yield return new WaitForSeconds(1.0f);
            RecoverLaunch();
        }

    }

    public virtual void Launch(Vector2 _f, float multiplier = 75.0f, bool dead = false)
    {
        Debug.Log("LAUNCH !!");
        Stunned();
        rb.sharedMaterial = bounceMat;
        rb.AddForce(_f * multiplier);
        iSensor.DisableCol();
        launched = true;
        StopCoroutine(Recovery());
        if(!dead)
            StartCoroutine(Recovery());
        // LeanTween.value(rootActorMovement.gameObject, rb.velocity.x, .0f, 5.0f).setEase(LeanTweenType.easeOutQuad).setOnUpdate((float f)=>{
        //     rb.velocity = new Vector2(f, rb.velocity.y);
        // });

        IEnumerator Recovery()
        {
            yield return new WaitForSeconds(1.0f);
            RecoverLaunch();
        }

    }

    public void RecoverLaunch()
    {
        iSensor.EnableCol();
        rb.sharedMaterial = null;
        col.enabled = true;
        EnableMovement();
        animManager.Move();
        animManager.PlayNonMovementAnim("Idle");
        launched = false;
        stunModifier = 1;
    }

    public void Explode()
    {
        

        
        
        if(GameplayController.instance.isMultiplayer)
        {
            GameplayController.instance.SetState(GameState.processing);
            List<object> param = new List<object>();
            param.Add(root.GetOrder());
            if(PhotonNetwork.IsMasterClient)
            {
                GPListener.instance.RaiseEvent(6, ReceiverGroup.MasterClient, EventCaching.DoNotCache, param.ToArray());
            }
            
        }
        else
        {
            Vector2 _force;
            float x = 1;
            if(Random.Range(0,2) == 0)
            {
                x = 1;
            }
            else
            {
                x = -1;
            }
            _force = new Vector2(x, 1);
            _force.Normalize();
            
            explo.Explode();
            Effects.instance.TriggerEffect(EffectList.camShake); 

            if(GameplayController.instance.Explode(root.GetID()))
            {
                animManager.PlayAnimAbsolutely("DeadHit");
                Launch(_force, 400, true);
            }
            else
            {
                animManager.PlayAnimAbsolutely("Hit");
                Launch(_force, 300);
            }
        }
        
        
        
    }

    public virtual void ExplodeEffect(bool dead)
    {
        Vector2 _force;
        float x = 1;
        if(Random.Range(0,2) == 0)
        {
            x = 1;
        }
        else
        {
            x = -1;
        }
        _force = new Vector2(x, 1);
        _force.Normalize();
        
        explo.Explode();
        Effects.instance.TriggerEffect(EffectList.camShake); 
        if(dead)
        {
                animManager.PlayAnimAbsolutely("DeadHit");
                Launch(_force, 400, true);
            }
            else
            {
                animManager.PlayAnimAbsolutely("Hit");
                Launch(_force, 300);
            }
    }

    public virtual void Stunned()
    {
        DisableMovement();
        movement.x = 0;
        movement.y = 0;
        stunModifier = 0;
        animManager.UnMove();
    }

    public void HitWall()
    {
        //Debug.Log("Hit Wall");
        

        if(root.GetAlive())
        {
            impactSounds.PlayRandomBetween(0,2);
            animManager.PlayAnimAbsolutely("Hit");
        }
            
    }

    public void Hit()
    {
        if(root.GetAlive())
            animManager.PlayAnimAbsolutely("Hit");
        //impactSounds.Play(0);
    }

    public void HitDead()
    {
        animManager.PlayAnimAbsolutely("DeadHit");
    }

    public virtual void EnableMovement()
    {
        if(!AIOverride)
            movementAttributes.controllable = true;
    }

    public virtual void DisableMovement()
    {
        movementAttributes.controllable = false;
    }

    public Vector2 GetPosition()
    {
        return rootActorMovement.position;
    }

    public bool IsYou()
    {
        return root.IsYou();
    }

    public PlayerIdentity GetID()
    {
        return root.GetID();
    }

    public TopChar GetRoot()
    {
        return root;
    }

    public bool Bombed()
    {
        return bomb.GetBombed();
    }

    
    public void SetAIMovement(Vector2 direction)
    {
        this.movement.x = direction.x;
        this.movement.y = direction.y;
    }










    private void OnCollisionEnter2D(Collision2D other) {
        Debug.Log("Collide With : "+other.collider.tag);
        if(other.collider.CompareTag("Ground"))
        {
            if(launched)
            {
                Debug.Log("Collide With Wall");
                HitWall();
            }
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if(stream.IsWriting)
        {
            // List<object> toSend = new List<object>();
            // toSend.Add(movement.x);
            // toSend.Add(movementAttributes.running);
            // toSend.Add(jump.jump);
            // stream.SendNext(toSend.ToArray());
            //Debug.Log("Sending : "+movement.x);
            stream.SendNext(movement.x);
            if(base.photonView.IsMine)
            {
                
            }
            
        }
        else if(stream.IsReading)
        {
            // List<object> received = new List<object>((object[])stream.ReceiveNext());
            //     this.movement.x = (float)received[0];
            //     this.movementAttributes.running = (bool)received[1];
            //Debug.Log("Receiving : "+(float)stream.ReceiveNext());
            float v = (float)stream.ReceiveNext();
            //Debug.Log("Receiving : "+v);
            movement.x = v;
            
            if(base.photonView.IsMine == false)
            {
                
            }
            
            
        }
    }
}


[System.Serializable]
public class JumpAttributes
{
    public float jumpForce = 5.0f;
    public float jumpHeavyPeak = 2.0f;
    public float jumpHeavyMultiplier = 3.0f;
    public float defaultGravityScale = 2.0f;
    public string jKey = "Jump";
    public bool jump = false;
    public bool land = false;

    public void FillFromJSON(JSONNode json)
    {
        jumpForce = (float)System.Convert.ToDouble(json["jf"].Value);
        jumpHeavyPeak = (float)System.Convert.ToDouble(json["jhp"].Value);
        jumpHeavyMultiplier = (float)System.Convert.ToDouble(json["jhm"].Value);
        defaultGravityScale = (float)System.Convert.ToDouble(json["dgs"].Value);
    }
}

[System.Serializable]
public class MovementAttributes
{
    public float speed = 2.0f;
    public float runSpeedAddition = 2.0f;
    public float runSpeedSlow = 1.0f;
    public float runSpeedDefaultValue = 1.0f;
    public string Hkey = "Horizontal";
    
    public bool controllable = true;
    public bool running = true;

    public void FillFromJSON(JSONNode json)
    {
        runSpeedAddition = (float)System.Convert.ToDouble(json["run"].Value);
        runSpeedSlow = (float)System.Convert.ToDouble(json["slw"].Value);
        runSpeedDefaultValue = (float)System.Convert.ToDouble(json["spd"].Value);
    }
}

[System.Serializable]
public class PInputKeys
{
    public PlayerIdentity identity;
    public string moveKey = "Horizontal";
    public string jumpkey = "Jump";
    public string interact = "Interact";
    public string take = "Take";
    public string run = "run";
    public string tag = "tag";

    public void SetIdentity(PlayerIdentity i)
    {
        this.identity = i;
        int _i = (int)(i+1);
        moveKey += _i.ToString();
        jumpkey += _i.ToString();
        interact += _i.ToString();
        take += _i.ToString();
        run += _i.ToString();
        tag += _i.ToString();
    }
}