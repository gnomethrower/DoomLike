using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MantisEnemyAI : MonoBehaviour
{

    /*
    This is the Behavior Script for the Simple Mantis Enemy. I might iterate or improve on this in the future.

    The Mantis is the first enemy you encounter. Easy to dispose of, simple to kill. But can be annoying in numbers.
    Behavior:

    1. Starts off Idle or Patroling.
    2. If player walks in aggro Range, or Mantis is attacked by player. It has a chance to act out an aggro Anim.
        - Maybe Aggro Behavior can alert nearby Mantis' allies?
    3. After aggro animation, Mantis starts chasing player.
    4. As soon as player is in attack range, Mantis attacks player - continue until player is either out of range or dead.
        a) Out of Range: Mantis follows player.
        b) Dead: Mantis plays aggro animation, then continues patroling.
    5. Patroling:
        a) If it spawns in patrol mode, it uses its spawnPosition as the base for its patrol radius.
        b) If it loses player, it uses its current position as patrol radius.


    */

    
    // ********** VARIABLES **********

    // DEBUG

    // Object References
    private NavMeshAgent navMeshAgent;
    private GameObject player;
    private Transform playerTransform;

    // State Variables
    public enum MantisBehaviorStates {IDLE, PATROL, AGGRO, CHASING, ATTACK};
    public int behaviorStateInteger;
    public int previousBehaviorStateInt;


    public enum MantisAnimationStates { IDLING, WALKING, SPRINTING, AGGROING, HURTING, JUMPANTICIPATING, INAIRLOOPING, ATTACKING, DYING}
    public int animationStateInteger;
    public int previousAnimStateInt;


    //Patrol Variables
    private bool patrolPointIsSet = false;
    private bool hasReachedPatrolPoint = true;
    private bool pauseTimerFinished = true;

    private Vector3 spawnPosition;
    private Vector3 patrolPositionOrigin;
    private Vector3 nextPatrolPoint;

    [SerializeField] float patrolRadius = 5f;
    [SerializeField] float minimumDistanceToDestination = 0.2f;
    [SerializeField] float pauseDurationMax = 5f;
    [SerializeField] float pauseDurationMin = 1f;



    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player");
        playerTransform = player.transform;
    }

    void Start()
    {
        //spawnPosition = transform.position;
        //Usually enemy will be defaulting to Idle at start, but patrol is just a test.


        behaviorStateInteger = ((int)MantisBehaviorStates.PATROL); // int value is 1
        animationStateInteger = ((int)MantisAnimationStates.WALKING); // int value is 1
    }

    void Update()
    {
        RunStateMachine();
    }

    private void RunStateMachine()
    {
        switch (behaviorStateInteger)
        {
            case 0:
                Debug.Log("State is now IDLE");
                break;

            case 1:
                Patrol();
                break;

            case 2:
                Aggro();
                break;

            case 3:
                Chasing();
                break;

            case 4:
                Attacking();
                break;
        }
    }

    // Patrol Code
    void Patrol()
    {
        if (!patrolPointIsSet && hasReachedPatrolPoint && pauseTimerFinished)
        {
            hasReachedPatrolPoint = false;
            pauseTimerFinished = false;

            PreviousAnimationStateUpdate();
            PreviousBehaviorStateUpdate();

            Debug.Log("2: Setting Walking Anim State at: " + Time.realtimeSinceStartup);
            animationStateInteger = ((int)MantisAnimationStates.WALKING);

            //Debug.Log("State is now PATROL");
            SetNewPatrolPoint();
        }

        if (navMeshAgent.remainingDistance < minimumDistanceToDestination && !pauseTimerFinished && !hasReachedPatrolPoint)
        {
            PreviousAnimationStateUpdate();
            PreviousBehaviorStateUpdate();

            animationStateInteger = ((int)MantisAnimationStates.IDLING);

            hasReachedPatrolPoint = true;
            //Debug.Log("We've reached the Point, now pausing!");
            StartCoroutine("PatrolPauseTimer");
        }
    }

    void PreviousBehaviorStateUpdate()
    {
        if (previousBehaviorStateInt != behaviorStateInteger)
        {
            //Debug.Log("Updating previous Behavior State");
            previousBehaviorStateInt = behaviorStateInteger;
        }
    }
    void PreviousAnimationStateUpdate()
    {
        if (previousAnimStateInt != animationStateInteger)
        {
            //Debug.Log("Updating previous Anim State");
            previousAnimStateInt = animationStateInteger;
        }
    }

    private void SetNewPatrolPoint()
    {
        NextPatrolPoint();

        //Initializing movement towards patrol point
        navMeshAgent.SetDestination(nextPatrolPoint);
        patrolPointIsSet = true;
    }

    private void NextPatrolPoint()
    {
        // Finds the next patrol point.
        // Does not check whether the patrol point is on the same floor.


        //This variable is needed to approximate a point near the Navmesh.
        Vector3 patrolPointApproximation;

        // This NavMeshHit point is needed to find the exact point on the Navmesh.
        NavMeshHit navHitPoint;

        patrolPositionOrigin = transform.position;
        patrolPointApproximation = patrolPositionOrigin + Random.insideUnitSphere * patrolRadius;

        //Visualizing the original approximationpoint
        Debug.DrawLine(patrolPositionOrigin, patrolPointApproximation, Color.white, 1f);

        if (NavMesh.SamplePosition(patrolPointApproximation,out navHitPoint, patrolRadius, NavMesh.GetAreaFromName("Ground")))
        {
            nextPatrolPoint = navHitPoint.position;
            
            //Debug.DrawLine(patrolPositionOrigin, nextPatrolPoint, Color.cyan, 10f);
            //Debug.DrawLine(nextPatrolPoint, new Vector3(nextPatrolPoint.x,nextPatrolPoint.y+1,nextPatrolPoint.z), Color.green, 10f);
           //Debug.Log("Patrol point found!");
        }

        else
        {
            //Debug.DrawLine(patrolPositionOrigin, patrolPointApproximation, Color.red, 10f);
            //Debug.Log("Failure to find new patrolpoint: Couldn't find a navmesh close to the Next Position");
        }

    }

    IEnumerator PatrolPauseTimer()
    {
        //Debug.Log("Pause timer started");
        float pauseTimer = Random.Range(pauseDurationMin, pauseDurationMax);
        yield return new WaitForSeconds(pauseTimer);

        pauseTimerFinished = true;
        patrolPointIsSet = false;
        //Debug.Log("Timer finished after " + pausePatrolForSeconds + " seconds.");
    }

    //Aggro Code
    void Aggro()
    {
        // if the enemy spots the player (player is in range, or attacks the enemy)
        // mantis stops in their tracks
        // turns towards player
        // plays an animation for x seconds
        // switches to CHASING state
    }

    //Chasing Code
    void Chasing()
    {
        // if player is in sight and not in attack range, move towards player until in attack range.
        // if player is both in sight AND in attack range, switch to ATTACK state.
    }

    //Attacking Code
    void Attacking()
    {

    }
}
