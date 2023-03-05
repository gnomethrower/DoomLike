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


    /* ********** VARIABLES ********** */

    // DEBUG

    // Object References
    private NavMeshAgent navMeshAgent;
    private GameObject player;
    private Transform playerTransform;

    // State Variables
    enum MantisStates {IDLE, PATROL, AGGRO, WALKING, SPRINTING, HURT, JUMPANT, INAIR, ATTACK, DEATH};
    int mantisStateValue;



    //Patrol Variables
    private bool hasReachedPatrolPoint = true;
    private Vector3 spawnPosition;
    private Vector3 patrolPositionOrigin;
    private Vector3 nextPatrolPoint;
    [SerializeField] float patrolRadius = 5f;
    //[SerializeField] float patrolPauseSeconds = 1f;


    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player");
        playerTransform = player.transform;
    }

    void Start()
    {
        //spawnPosition = transform.position;
        NextPatrolPoint();

        //Usually enemy will be defaulting to Idle at start, but patrol is just a test.
        mantisStateValue = ((int)MantisStates.PATROL);
    }

    void Update()
    {
        RunStateMachine();
    }

    private void RunStateMachine()
    {
        switch (mantisStateValue)
        {
            case 0:
                Debug.Log("State is now IDLE");
                break;

            case 1:
                    if(hasReachedPatrolPoint) PatrolState();
                break;
        }
    }

    private void PatrolState()
    {
        Debug.Log("State is now PATROL");

        hasReachedPatrolPoint = false;

        NextPatrolPoint();

        //Initializing movement towards patrol point
        navMeshAgent.SetDestination(nextPatrolPoint);

        if (navMeshAgent.remainingDistance > 0.1f)
        {
            Debug.Log("On my way");
        }

        else
        {
            hasReachedPatrolPoint = true;
            Debug.Log("Arrived at PatrolPoint");
        }
    }
        


    private void NextPatrolPoint()
    {
        /* Finds the next patrol point.
        * Does not check whether the patrol point is on the same floor.
        */

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
            
            Debug.DrawLine(patrolPositionOrigin, nextPatrolPoint, Color.cyan, 10f);
            Debug.DrawLine(nextPatrolPoint, new Vector3(nextPatrolPoint.x,nextPatrolPoint.y+1,nextPatrolPoint.z), Color.green, 10f);
            Debug.Log("Patrol point found!");
        }

        else
        {
            Debug.DrawLine(patrolPositionOrigin, patrolPointApproximation, Color.red, 10f);
            Debug.Log("Failure to find new patrolpoint: Couldn't find a navmesh close to the Next Position");
        }

    }
    


}
