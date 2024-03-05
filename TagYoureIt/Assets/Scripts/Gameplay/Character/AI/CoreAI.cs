using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class CoreAI : MonoBehaviour
{
    [SerializeField] TopChar topChar;
    [SerializeField] BaseChar coreChar;

    [Header("AI")]
    [SerializeField] Seeker seeker;
    [SerializeField] Path path;
    [SerializeField] List<Transform> pathVisualizer;
    [SerializeField] int currentWayPoint;
    [SerializeField] bool reachedFinal;
    [SerializeField] Transform target;
    [SerializeField] float yDelta = 0;
    [SerializeField] List<Transform> randomTargets;
    [SerializeField] Vector2 moveDirection;
    [SerializeField] Collider2D bombSensor;

    [Header("STATES")]
    [SerializeField] bool overrideControl = false;

    private void OnValidate() {
        Debug.Log("Called");
        if(overrideControl)
        {
            OverrideControl();
        }
    }


    private void Start() {
        
    }


    public void OverrideControl()
    {
        randomTargets = new List<Transform>();
        List<GameObject> randomT = new List<GameObject>(GameObject.FindGameObjectsWithTag("RandomTargets"));
        for(int i = 0 ; i < randomT.Count ; i++)
        {
            randomTargets.Add(randomT[i].transform);
        }
        
        target = GameplayController.instance.GetOppositeChar(coreChar).GetCenterActorMovement();
        coreChar.OverrideAI(true);
        CheckState();
        //InvokeRepeating("CheckState",0,.5f);
    }

    public void UnAI()
    {
        CancelInvoke("CheckState");
        coreChar.OverrideAI(false);
    }

    public void CheckState()
    {
        if(coreChar.Bombed())
        {
            //chasePlayer
            GeneratePathToPlayer();
        }
        else
        {
            GeneratePathFromRandom();
            //away from player
        }
    }

    public void Bombed()
    {

    }

    public void GeneratePathToPlayer()
    {
        if(seeker.IsDone())
        {
            seeker.StartPath(coreChar.GetCenterActorMovement().position, target.position, OnPathComplete);
        }
    }

    public void GeneratePathFromRandom()
    {
        Debug.Log("Generating");
        Transform currentRunTo = randomTargets[Random.Range(0,randomTargets.Count)];
        if(seeker.IsDone())
        {
            
            seeker.StartPath(coreChar.GetCenterActorMovement().position, currentRunTo.position, OnPathComplete);
        }
    }
    
    public void StartGeneratePlayerPath()
    {
        InvokeRepeating("GeneratePathToPlayer",0,.5f);
    }

    public void StartEvading()
    {
        GeneratePathFromRandom();
    }

    public void ResetChecker()
    {
        CancelInvoke();
    }

    public void OnPathComplete(Path p)
    {
        if(!p.error)
        {
            path = p;

            if(coreChar.Bombed())
            {
                currentWayPoint = 1;
            }
            else
            {
                currentWayPoint = 0;
            }
            

            if(pathVisualizer != null)
            {
                if(pathVisualizer.Count > 0)
                {
                    for(int i = 0 ; i < pathVisualizer.Count ; i++)
                    {
                        Destroy(pathVisualizer[i].gameObject);
                    }
                }
            }

            pathVisualizer = new List<Transform>();

            for(int i = 0 ; i < p.vectorPath.Count; i++)
            {
                GameObject g = new GameObject("Path Visualizer "+i.ToString());
                g.transform.position = p.vectorPath[i];
                pathVisualizer.Add(g.transform);
            }
        }
        else
        {
            Debug.LogError("Path Error : "+p.errorLog);
        }
    }

    private void FixedUpdate() {
        if(path == null)
        return;

        if(currentWayPoint >= path.vectorPath.Count)
        {
            reachedFinal = true;
            if(coreChar.Bombed() == false)
                GeneratePathFromRandom();
            else
            {
                ResetChecker();
                currentWayPoint = 0;
                StartGeneratePlayerPath();
                
            }

            return;
        }
        else
        {
            reachedFinal = false;
            
        }
        

        moveDirection = ((Vector2)path.vectorPath[currentWayPoint] - (Vector2)coreChar.GetCenterActorMovement().position).normalized;
        coreChar.SetAIMovement(moveDirection);

        yDelta = coreChar.GetCenterActorMovement().position.y - path.vectorPath[currentWayPoint].y;
        if(yDelta < 0)
        {
            //di bawahnya
            if(Mathf.Abs(yDelta) > 0.75f)
            {
                if(coreChar.GetGrounded())
                {
                    coreChar.SetAIJump();
                }
                
            }
        }

        float distance = Vector2.Distance(coreChar.GetCenterActorMovement().position, (Vector2)path.vectorPath[currentWayPoint]);

        if(distance < 0.9f)
        {
            currentWayPoint++;
        }

        
    }


}
