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
    [SerializeField] GameObject player;
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
    private float attackTimer = 0f;
    [SerializeField] private float attackPauseInSeconds = 1f;
    private Mortality_Script mortalityScript;
    #endregion

    #region Movement
    [Header("Movement")]
    #region speed
    [SerializeField] private float walkingSpeed;
    [SerializeField] private float chasingSpeed;
    private float currentSpeed;
    [Tooltip("The multiplier can't be changed on runtime, as the multiplier is only applied during Awake.")]

    #endregion

    #region Idle
    [SerializeField] private float idleRange;
    [SerializeField] private float wanderingPauseDuration;
    private float pauseStartTime = 0;

    //[SerializeField] private bool isMoving = false;
    //private bool isPausedWhileWandering = false;
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
    [Tooltip("0 = idle, 1 = chasing, 2 = attacking")]
    [SerializeField] private int currentState = 0;
    [SerializeField] private int IdleSubState = 0; //0 = pausing   |    1 = moving    |    2 = getting wanderpoint;
    [SerializeField] private int attackSubState = 0; // 0 = attack    |    1 = timer;


    [SerializeField] private bool isChasing = false;
    [SerializeField] private bool isAttacking = false;

    private string currentAnimationState;

    private bool initializingChasing = false;
    //private bool initializingAttacking = false;
    #endregion

    #endregion

    private void Awake()
    {
        #region initialize variables
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
        mortalityScript = GetComponent<Mortality_Script>();
        #endregion

        #region former Start Region
        spawnPoint = transform.position;
        lastWanderPoint = transform.position;
        currentWanderPoint = transform.position;
        #endregion

        #region Animation Speed
        animator.SetFloat("animationSpeedMultiplier", 1 / attackPauseInSeconds);
        #endregion
    }

    private void Start()
    {
        InitializeIdleState();
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

        #region Chase Updates

        if (canChase)
        {
            if (chaseRadiusScript.insideChaseRadius && !attackRadiusScript.insideAttackRadius)
            {
                SetState(1);
            }
        }
        #endregion

        #region Attack Updates
        if (CanMeleeAttack)
        {
            if (chaseRadiusScript.insideChaseRadius && attackRadiusScript.insideAttackRadius)
            {
                SetState(2);
            }
        }

        #endregion

        #region Pain and Hurt
        if (mortalityScript.gotHurt)
        {
            if (!initializingChasing && !isChasing)
            {
                //Debug.Log("Got hurt, now chasing!");
                initializingChasing = true;
                currentState = 1;
            }
            mortalityScript.gotHurt = false;
        }
        #endregion
        StateFrameUpdate();
    }

    #region Methods

    #region Idle Methods

    private void InitializeIdleState()
    {
        agent.speed = walkingSpeed;
        agent.stoppingDistance = chasingStoppingDistance;

        Debug.Log("Initializing IdleState");
        chaseRadiusCollider.enabled = true;
        attackRadiusCollider.enabled = true;

        SetState(0);
        IdleSubState = 2;
        ChangeAnimationState("simplestEnemyStanding");
    }
    private void IdleState()
    {
        //IdleSubState:       pausing = 0    |    moving = 1    |    getting waypoint = 2;
        switch (IdleSubState)
        {
            case 0:
                if (Time.time < pauseEndTime)
                {
                    return;
                }
                else
                {
                    IdleSubState = 2;
                }
                break;

            case 1:
                if (agent.remainingDistance <= agent.stoppingDistance && !agent.pathPending)
                {
                    TriggerPause();
                }
                break;

            case 2:
                currentWanderPoint = PickRandomIdleWalkpoint();
                MoveToWalkPoint();
                break;
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
            return spawnPoint;
        }
    }
    private void MoveToWalkPoint()
    {
        IdleSubState = 1;
        agent.SetDestination(currentWanderPoint);
        ChangeAnimationState("simplestEnemyWalking");
    }
    private void TriggerPause()
    {
        ChangeAnimationState("simplestEnemyStanding");
        pauseStartTime = Time.time;
        pauseEndTime = pauseStartTime + wanderingPauseDuration;
        IdleSubState = 0;
    }

    #endregion

    #region Chase Methods

    private void InitializeChasingState()
    {
        Debug.Log("Initializing ChasingState");
        initializingChasing = true;
        currentState = 1;
        isChasing = true;
    }

    private void ChasingState() // Substates: 0: change currentSpeed and stopping distance, initialize animation. 1: Set destination to current player pos.
    {
        if (initializingChasing) //initializing the functionality for the chase state.
        {
            initializingChasing = false;
            ChangeAnimationState("simplestEnemyChasing");
            agent.speed = chasingSpeed;
            agent.stoppingDistance = chasingStoppingDistance;
        }

        if (agent.isStopped) //if attack state stopped the agent, restart them.
        {
            agent.isStopped = false;
        }

        if (isChasing) //if chasing is active, update the player's position each frame.
        {
            agent.SetDestination(player.transform.position);
        }
    }
    #endregion

    #region Attack Methods

    private void InitializeAttackState()
    {
        Debug.Log("Initializing Attack State");
        currentState = 2;
        //agent.isStopped = true;
        StopEnemyAtCurrentPosition();
        isAttacking = true;
    }
    private void AttackState()
    {
        //Check if the player is still within attack range. If not, start chasing.
        if (distanceToPlayer > attackRange)
        {
            //Debug.Log("out of range CHASE!");
            isAttacking = false;
        }

        if (isAttacking)
        {
            switch (attackSubState) // 0 = attack    |    1 = timer;
            {
                case 0:
                    //Debug.Log("Attacking!");
                    attackTimer = 0;
                    ChangeAnimationState("simplestEnemyAttacking");
                    playerControllerScript.currentHealth -= damage;
                    attackSubState = 1;
                    break;

                case 1:
                    if (attackTimer < attackPauseInSeconds)
                    {
                        attackTimer += Time.deltaTime;
                    }
                    else
                    {
                        LookAtThePlayer();
                        attackSubState = 0;
                    }
                    break;
            }
        }
    }

    #endregion

    #region Animation Methods

    void StopEnemyAtCurrentPosition()
    {
        agent.isStopped = true;
    }

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
    }

    void LookAtThePlayer()
    {
        Vector3 correctedPlayerHeight = new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z);
        transform.LookAt(correctedPlayerHeight);
    }

    #endregion

    #region Calm Down Methods
    void CalmingDown()
    {

    }

    #endregion

    #endregion

    #region State Management
    void SetState(int nextState)
    {
        if (nextState == currentState)
        {
            return;
        }
        currentState = nextState;

        InitializingNextState(currentState);
    }

    void InitializingNextState(int initState)
    {
        switch (initState)
        {
            case 0:
                InitializeIdleState();
                break;
            case 1:
                InitializeChasingState();
                break;
            case 2:
                InitializeAttackState();
                break;
        }
    }

    void StateFrameUpdate()
    {
        switch (currentState)
        {
            case 0:
                IdleState();
                break;
            case 1:
                ChasingState();
                break;
            case 2:
                AttackState();
                break;
        }
    }


    #endregion
}
