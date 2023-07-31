using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;


public class BasicEnemyClass : MonoBehaviour
{
    #region Variables

    #region Scene References
    GameObject player;
    NavMeshAgent agent;
    Collider chaseRadius;
    Collider attackRadius;
    #endregion

    #region Health and Damage
    [SerializeField] private float health;
    [SerializeField] private float damage;
    #endregion

    #region Movement
    //[SerializeField] private float enemyHeightOffset = 1f;
    [SerializeField] private float walkingSpeed;
    [SerializeField] private float chaseSpeedMultiplier;
    [SerializeField] private float idleRange;
    [SerializeField] private bool onTheMove;
    [SerializeField] private bool isPausedWhileWandering = false;
    [SerializeField] private float pauseDuration;
    private float pauseEndTime = 0f;
    private Vector3 lastWanderPoint;
    private Vector3 currentWanderPoint;
    private Vector3 spawnPoint;
    #endregion

    #region states
    private bool isIdle, isChasing, isAttacking = false;
    #endregion

    #endregion

    // Enemy starts as Idle - he walks around to random points around himself, in his idlerange
    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        agent = GetComponent<NavMeshAgent>();
        isIdle = true;
        onTheMove = false;
    }

    private void Start()
    {
        spawnPoint = transform.position;
        lastWanderPoint = transform.position;
        currentWanderPoint = transform.position;
    }

    private void Update()
    {
        if (agent.remainingDistance <= agent.stoppingDistance && !agent.pathPending && onTheMove)
        {
            Debug.Log("I have arrived!");
            onTheMove = false;
            TriggerPause();
        }

        if (Time.time >= pauseEndTime)
        {
            isPausedWhileWandering = false;
        }

        if (isIdle && !isPausedWhileWandering)
        {
            IdleState();
        }

        if (isChasing)
        {

        }

        if (isAttacking)
        {

        }
    }

    #region Idle Methods
    private void IdleState()
    {
        if(!onTheMove)
        {   
            /* 1. Picks a random point near him, limited to his walking range.
             * 2. walks to the point.
             * 3. once point is reached, he picks a new point.
             * 4. repeat.
             */

            onTheMove = true;
            Debug.Log("IdleState started");

            currentWanderPoint = PickRandomIdleWalkpoint();
            MoveToWalkPoint();
        } 

    }
    private Vector3 PickRandomIdleWalkpoint()
    {
        Vector3 randomSpherePoint = Random.insideUnitSphere * idleRange;
        DebugIndicator.DrawDebugIndicator(randomSpherePoint, Color.yellow, 2f, .5f);

        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomSpherePoint, out hit, idleRange/2, NavMesh.AllAreas))
        {
            Debug.Log("We hit a SamplePosition");
            Debug.DrawLine(transform.position, hit.position, Color.blue, 10f);
            DebugIndicator.DrawDebugIndicator(hit.position, Color.magenta, 10f, 1f);
            lastWanderPoint = currentWanderPoint;
            return hit.position;
        }
        else
        {
            Debug.LogError("No Walkpoint found, defaulting to zero!");
            onTheMove = false;

            return spawnPoint;
        }
    }
    private void MoveToWalkPoint()
    {
        Debug.Log("MoveToWalkpoint started");
        agent.SetDestination(currentWanderPoint);
    }
    private void TriggerPause()
    {
        isPausedWhileWandering = true;
        // Record the current time as the start of the pause
        float pauseStartTime = Time.time;
        // Calculate the end of the pause
        pauseEndTime = pauseStartTime + pauseDuration;

        // Pause started, do actions you want to perform during the pause here...
        Debug.Log($"Pause started. Pausing for {pauseDuration} seconds...");
    }
    #endregion

    #region Chase Methods

    private void ChasingState()
    {
        /*
         * 1. Once the player gets into the enemie's chase radius, he starts chasing the player at a higher speed.
         * 2. Once the player enters the attack radius, we stop the chase state and go into the attack state, after which we go back into chase state.
         */

    }

    private void OnTriggerEnter(Collider other)
    {
        
    }
    #endregion

    #region Attack Methods
    #endregion
    #region start state methods
    void StartIdleState()
    {
        isIdle = true;

        isChasing = false;
        isAttacking = false;
    }
    void StartChasingState()
    {
        isChasing = true;

        isIdle = false;
        isAttacking = false;
    }
    void StartAttackingState()
    {
        isAttacking = true;

        isIdle = false;
        isChasing = false;
    }
    #endregion  


}
