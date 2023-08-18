using BehaviorDesigner.Runtime.Tasks.Unity.UnityTransform;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;


public class BasicEnemyClass : MonoBehaviour
{
    #region Variables

    #region Scene References
    NavMeshAgent agent;
    GameObject player;
    float distanceToPlayer;
    Vector3 playerPos;
    Vector3 playerEnemyVector;
    Collider playerCollider;
    Collider chaseRadiusCollider;
    Collider attackRadiusCollider;
    Animator animator;
    #endregion

    #region Script References
    PlayerController_Script playerControllerScript;
    ChaseRadiusScript chaseRadiusScript;
    AttackRadiusScript attackRadiusScript;
    #endregion

    #region Behaviour
    [Header("Behavior")]
    [SerializeField] private bool canChase;
    [SerializeField] private bool CanMeleeAttack;
    #endregion

    #region Attack and Damage
    [Header("Health and Damage")]
    [SerializeField] private int damage;
    [SerializeField] private float attackRange;
    [SerializeField] private float attackTimer = 0f;
    [SerializeField] private float attackPauseDuration = 1f;

    #endregion

    #region Movement
    [Header("Movement")]
    #region speed
    [SerializeField] private float speed;
    [Tooltip("The multiplier can't be changed on runtime, as the multiplier is only applied during Awake.")]

    [SerializeField] private float chaseSpeedMultiplier;

    private float chasingSpeed; //This variable is the silent variable that is calculated in Awake.
    #endregion

    #region Idle
    [SerializeField] private float idleRange;
    [SerializeField] private float wanderingPauseDuration;

    [SerializeField] private bool isMoving = false;
    private bool isPausedWhileWandering = false;
    private float pauseEndTime = 0f;

    private Vector3 lastWanderPoint;
    private Vector3 currentWanderPoint;
    private Vector3 spawnPoint;
    #endregion

    #region NavAgent-Movement Variables
    [SerializeField] private float idleStoppingDistance;
    [SerializeField] private float chasingStoppingDistance;
    #endregion

    #endregion

    #region states
    [Header("States")]

    [SerializeField] private bool isIdle = false;
    [SerializeField] private bool isChasing = false;
    [SerializeField] private bool isAttacking = false;

    private string currentAnimationState;

    //private bool goingToIdle = false;
    private bool goingToChasing = false;
    private bool goingToAttacking = false;

    //private bool initializingIdle = false;
    private bool initializingChasing = false;
    private bool initializingAttacking = false;
    #endregion

    #endregion

    private void Awake()
    {
        #region initialize variables
        chasingSpeed = chaseSpeedMultiplier * speed;
        #endregion

        #region set object references
        player = GameObject.FindGameObjectWithTag("Player");

        agent = GetComponentInParent<NavMeshAgent>();
        if (agent == null) { Debug.Log("No NavAgent attached"); }

        chaseRadiusCollider = GameObject.Find("chaseRadius").GetComponent<Collider>();
        attackRadiusCollider = GameObject.Find("attackRadius").GetComponent<Collider>();
        playerControllerScript = player.GetComponent<PlayerController_Script>();

        animator = GetComponent<Animator>();
        #endregion

        #region script references
        chaseRadiusScript = chaseRadiusCollider.GetComponent<ChaseRadiusScript>();
        attackRadiusScript = attackRadiusCollider.GetComponent<AttackRadiusScript>();
        #endregion
    }

    private void Start()
    {
        spawnPoint = transform.position;
        lastWanderPoint = transform.position;
        currentWanderPoint = transform.position;
        GoTotIdleState();
    }

    private void Update()
    {
        #region Debug Updates

        //Debug.Log("isWalking = " + animator.GetBool("isWalking"));

        #endregion
        #region SceneReference Updates
        distanceToPlayer = Vector3.Distance(player.transform.position, this.transform.position);
        playerEnemyVector = player.transform.position - this.transform.position;
        #endregion

        #region Idle Updates
        if (agent.remainingDistance <= agent.stoppingDistance && !agent.pathPending && isMoving)
        {
            //Debug.Log("I have arrived!");
            isMoving = false;
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
        #endregion

        #region Chase Updates
        if (canChase)
        {
            if (chaseRadiusScript.insideRadius && !goingToChasing && !isChasing)
            {
                //Debug.Log("goingToChasing set to True");
                chaseRadiusScript.insideRadius = false;
                goingToChasing = true;
            }

            if (goingToChasing)
            {
                GoToChasingState();
            }

            if (isChasing)
            {
                ChasingState();
            }
        }
        #endregion

        #region Attack Updates
        if (CanMeleeAttack)
        {
            if (attackRadiusScript.insideRadius && !goingToAttacking && !isAttacking)
            {
                Debug.Log("goingToAttacking set to True");
                //Debug.Log(distanceToPlayer);
                attackRadiusScript.insideRadius = false;
                goingToAttacking = true;
            }

            if (goingToAttacking)
            {
                GoToAttackingState();
            }

            if (isAttacking)
            {
                AttackState();
            }
        }
        #endregion

        #region Animation Updates


        #endregion
    }

    #region Methods

    #region Idle Methods

    void GoTotIdleState()
    {
        chaseRadiusCollider.enabled = true;
        attackRadiusCollider.enabled = true;

        StopChasingState();
        StopAttackState();

        //goingToIdle = false;
        //initializingIdle = true;
        isIdle = true;
    }
    void StopIdleState()
    {
        isIdle = false;
        //goingToIdle = false;
        //initializingIdle = false;
    }
    private void IdleState()
    {
        if (!isMoving)
        {
            isMoving = true;
            currentWanderPoint = PickRandomIdleWalkpoint();
            MoveToWalkPoint();
        }

    }
    private Vector3 PickRandomIdleWalkpoint()
    {
        Vector3 randomSpherePoint = Random.insideUnitSphere * idleRange;
        DebugIndicator.DrawDebugIndicator(randomSpherePoint, Color.yellow, 2f, .5f);

        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomSpherePoint, out hit, idleRange / 2, NavMesh.AllAreas))
        {
            //Debug.Log("We hit a SamplePosition");
            Debug.DrawLine(transform.position, hit.position, Color.blue, 10f);
            DebugIndicator.DrawDebugIndicator(hit.position, Color.magenta, 10f, 1f);
            lastWanderPoint = currentWanderPoint;
            return hit.position;
        }
        else
        {
            //Debug.LogError("No Walkpoint found, defaulting to spawnPoint!");
            isMoving = false;

            return spawnPoint;
        }
    }
    private void MoveToWalkPoint()
    {
        agent.SetDestination(currentWanderPoint);
        ChangeAnimationState("simplestEnemyWalking");
    }
    private void TriggerPause()
    {
        isPausedWhileWandering = true;
        ChangeAnimationState("simplestEnemyStanding");
        // Record the current time as the start of the pause
        float pauseStartTime = Time.time;
        // Calculate the end of the pause
        pauseEndTime = pauseStartTime + wanderingPauseDuration;

        // Pause started, do actions you want to perform during the pause here...
    }

    #endregion

    #region Chase Methods
    void GoToChasingState()
    {
        //Debug.Log("GoToChasingState started");

        StopIdleState();
        StopAttackState();

        //chaseRadiusCollider.enabled = false;

        goingToChasing = false;
        initializingChasing = true;
        isChasing = true;
    }
    private void ChasingState()
    {
        Debug.Log("ChasingState() called");
        if (agent.isStopped) //if attack state stopped the agent, restart them.
        {
            agent.isStopped = false;
        }
        if (initializingChasing) //functionality for the first time setup of the chase state.
        {
            //Debug.Log("Initialized GoToChasingState");
            ChangeAnimationState("simplestEnemyChasing");
            agent.speed = chasingSpeed;
            agent.stoppingDistance = chasingStoppingDistance;
            initializingChasing = false;
        }
        agent.SetDestination(player.transform.position);
    }
    void StopChasingState()
    {
        goingToChasing = false;
        initializingChasing = false;
        isChasing = false;
    }
    #endregion

    #region Attack Methods
    void GoToAttackingState()
    {
        StopIdleState();
        StopChasingState();

        //attackRadiusCollider.enabled = false;

        goingToAttacking = false;
        initializingAttacking = true;
        isAttacking = true;
    }
    private void AttackState()
    {
        //Debug.Log("AttackState() called");

        if (initializingAttacking)
        {
            initializingAttacking = false;
            agent.isStopped = true;
        }

        //Check if the player is still within attack range. If not, start chasing.
        if (distanceToPlayer > attackRange)
        {
            GoToChasingState();
        }

        if (attackTimer < attackPauseDuration)
        {
            //Debug.Log("timer is at " + attackTimer);
            attackTimer += Time.deltaTime;
        }
        else
        {
            attackTimer = 0;
            playerControllerScript.currentHealth -= damage;
            ChangeAnimationState("simplestEnemyAttacking");
        }

    }
    void StopAttackState()
    {
        goingToAttacking = false;
        initializingAttacking = false;
        isAttacking = false;
    }
    #endregion

    #region Animation Methods

    void ChangeAnimationState(string newState)
    {
        //I want to reset the position and rotation of the animator to the navAgent's before continuing.
        animator.transform.rotation = agent.transform.rotation;
        animator.transform.position = agent.transform.position;

        //stop the same anim from interrupting itself
        if (currentAnimationState == newState) { return; }

        //play the next animation state
        animator.Play(newState);

        //reassign current state
        currentAnimationState = newState;
        Debug.Log("Now playing animation State: " + newState);
    }

    #endregion

    #endregion

}
