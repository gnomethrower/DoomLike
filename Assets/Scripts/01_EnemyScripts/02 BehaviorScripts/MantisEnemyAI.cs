using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MantisEnemyAI : MonoBehaviour
{
    #region VARIABLES

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
    [SerializeField] private bool patrolPointIsSet = false;
    [SerializeField] private bool hasReachedPatrolPoint = true;
    [SerializeField] private bool pauseTimerFinished = true;

    private Vector3 spawnPosition;
    private Vector3 patrolPositionOrigin;
    private Vector3 nextPatrolPoint;

    [SerializeField] float patrolRadius = 5f;
    [SerializeField] float minimumDistanceToDestination = 0.2f;
    [SerializeField] float pauseDurationMax = 5f;
    [SerializeField] float pauseDurationMin = 1f;

    #endregion


    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player");
        playerTransform = player.transform;
    }

    void Start()
    {
        //spawnPosition = transform.position;
        //Usually _spawnObject will be defaulting to Idle at start, but patrol is just a test.


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
                PatrolState();
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

    #region PATROL CODE
    void PatrolState()
    {
        if (!patrolPointIsSet && hasReachedPatrolPoint && pauseTimerFinished)
        {
            hasReachedPatrolPoint = false;
            pauseTimerFinished = false;

            PreviousAnimationStateUpdate();
            PreviousBehaviorStateUpdate();

            Debug.Log("2: Setting Walking Anim State at: " + Time.realtimeSinceStartup);
            animationStateInteger = ((int)MantisAnimationStates.WALKING);

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
    #endregion

    #region AGGRO CODE
    void Aggro()
    {
        // if the _spawnObject spots the player (player is in range, or attacks the _spawnObject)
        // mantis stops in their tracks
        // turns towards player
        // plays an animation for x seconds
        // switches to CHASING state
    }
    #endregion

    #region CHASING CODE
    void Chasing()
    {
        // if player is in sight and not in attack range, move towards player until in attack range.
        // if player is both in sight AND in attack range, switch to ATTACK state.
    }
    #endregion

    #region ATTACKING CODE
    void Attacking()
    {

    }
    #endregion
}
