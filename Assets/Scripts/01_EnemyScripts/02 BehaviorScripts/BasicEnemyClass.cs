using BehaviorDesigner.Runtime.Tasks.Movement;
using BehaviorDesigner.Runtime.Tasks.Unity.UnityTransform;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using Random = UnityEngine.Random;

public class BasicEnemyClass : MonoBehaviour
{
    #region Variables

    #region Scene References
    NavMeshAgent agent;
    [SerializeField] private GameObject player;
    float distanceToPlayer;
    Vector3 playerPos;
    Vector3 playerEnemyVector;
    Collider playerCollider;
    GameObject chaseRadius;
    Collider chaseRadiusCollider;
    Collider attackRadiusCollider;
    Transform chaseRadiusTransform;
    Transform attackRadiusTransform;

    Animator animator;
    private string lastAnimation = null;
    string currentAnimation = null;
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
    private float attackPauseStart;
    private float attackPauseEnd;
    [SerializeField] private float attackPauseInSeconds = 1f;
    private Mortality_Script mortalityScript;
    private quaternion savedAttackRotation;
    private LayerMask layersToRaycastAgainst = 1 << 3 | 1 << 6 | 1 << 8;
    #endregion

    #region Movement
    [Header("Movement")]
    #region speed
    //private float turnTimeCount = 0f;
    [SerializeField] private float walkingSpeed;
    [SerializeField] private float chasingSpeed;
    [Tooltip("Determines the length of chasing behavior after the player has exited the chase range.")]
    private bool chaseTimerInProgress;
    private float chaseTimer = 0f;
    [SerializeField] private float chaseDurationInSeconds = 0f;
    private float currentSpeed;
    [Tooltip("The multiplier can't be changed on runtime, as the multiplier is only applied during Awake.")]

    private Vector3 newRotation;

    private float xVel;
    private float yVel;
    private float zVel;
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
    [Tooltip("0 = idle, 1 = chasing, 2 = attacking, 3 = hurtAnimation")]
    [SerializeField] private int currentState = 0;
    [SerializeField] private int lastState = 0;

    [SerializeField] private int IdleSubState = 2; //0 = pausing   |    1 = moving    |    2 = getting wanderpoint;
    [SerializeField] private int attackSubState = 0; // 0 = attack    |    1 = timer;
    [SerializeField] private int hurtSubState = 0;

    [SerializeField] private bool isChasing = false;
    [SerializeField] private bool isAttacking = false;

    int busyIterator = 0;
    private bool currentlyBusy = false;

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
        agent.updateRotation = false;
        //if (agent == null) { Debug.Log("No NavAgent attached"); }
        chaseRadiusCollider = GameObject.Find("chaseRadius").GetComponentInChildren<Collider>();


        chaseRadiusTransform = this.gameObject.transform.GetChild(1);
        attackRadiusTransform = this.gameObject.transform.GetChild(2);

        playerCollider = GameObject.Find("Player").GetComponent<Collider>();
        animator = GetComponent<Animator>();
        #endregion

        #region script references
        chaseRadiusScript = chaseRadiusTransform.GetComponent<ChaseRadiusScript>();
        attackRadiusScript = attackRadiusTransform.GetComponent<AttackRadiusScript>();
        mortalityScript = this.gameObject.GetComponent<Mortality_Script>();
        playerControllerScript = player.GetComponent<PlayerController_Script>();
        #endregion

        #region former Start Region
        spawnPoint = transform.position;
        lastWanderPoint = transform.position;
        currentWanderPoint = transform.position;
        #endregion

        #region Animation Speed
        animator.SetFloat("animationSpeedMultiplier", 1 / attackPauseInSeconds);
        #endregion

        #region NavAgent
        agent.obstacleAvoidanceType = ObstacleAvoidanceType.HighQualityObstacleAvoidance;
        #endregion
    }
    private void Start()
    {
        SetState(0);
    }
    private void Update()
    {
        #region Debug Updates
        #endregion

        #region SceneReference Updates
        distanceToPlayer = Vector3.Distance(player.transform.position, this.transform.position);
        playerEnemyVector = player.transform.position - this.transform.position;
        #endregion

        #region Chase Updates
        if (canChase)
        {
            if (CanSeeThePlayer() && chaseRadiusScript.insideChaseRadius && !attackRadiusScript.insideAttackRadius && !isChasing)
            {
                SetState(1);
            }
        }
        #endregion

        #region Attack Updates
        if (CanMeleeAttack)
        {
            if (attackRadiusScript.insideAttackRadius && !isAttacking)
            {
                SetState(2);
            }
        }
        #endregion

        #region Hurt Update
        if (mortalityScript.gotHurt)
        {
            mortalityScript.gotHurt = false;

            SetState(3);
        }
        #endregion

        StateFrameUpdate();
    }

    #region Behavior Methods

    #region Idle Methods
    private void InitializeIdleState()
    {
        IdleSubState = 2;
        agent.speed = walkingSpeed;
        agent.stoppingDistance = chasingStoppingDistance;
        ChangeAnimation("simplestEnemyStanding", true);
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
                if (currentWanderPoint != null) { FaceTarget(currentWanderPoint, .5f, true); }
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
        ChangeAnimation("simplestEnemyWalking", true);
    }
    private void TriggerPause()
    {
        ChangeAnimation("simplestEnemyStanding", true);
        pauseStartTime = Time.time;
        pauseEndTime = pauseStartTime + wanderingPauseDuration;
        IdleSubState = 0;
    }
    private void ExitIdle()
    {
        IdleSubState = 0;
    }
    #endregion

    #region Chase Methods
    private void InitializeChasingState()
    {
        Debug.Log("Chase Init");
        initializingChasing = true;
        //currentState = 1;
        isChasing = true;
        chaseTimer = 0;
    }
    private void ChasingState() // Substates: 0: change currentSpeed and stopping distance, initialize animation. 1: Set destination to current player pos.
    {
        if (!currentlyBusy)
        {

            if (initializingChasing) //initializing the functionality for the chase state.
            {
                initializingChasing = false;
                ChangeAnimation("simplestEnemyChasing", true);
                agent.speed = chasingSpeed;
                agent.stoppingDistance = chasingStoppingDistance;
            }

            if (isChasing) //if chasing is active, update the player's position each frame.
            {
                FaceTarget(player.transform.position, .15f, true);
                agent.SetDestination(player.transform.position);
                CheckToStopChasing();

            }
        }
    }
    void CheckToStopChasing()
    {
        /* To-Do if I have time:
         * 
         * Start a Stop-Chasing timer if: 
         * 1. I am out of the chaseradius and the enemy cannot see me.
         * 2. I am out of the chaseradius and the enemy can see me, but is X Meters away (from their spawn?).
         * 3. I am inside the chaseradius and the enemy cannot see me for X seconds - This is to prevent an enemy to chase me in a maze, when they might've lost my trail.
         */

        if (!chaseRadiusScript.insideChaseRadius && !CanSeeThePlayer())
        {
            if (!chaseTimerInProgress)
            {
                chaseTimerInProgress = true;
            }

            else if (chaseTimerInProgress)
            {
                if (chaseTimer < chaseDurationInSeconds)
                {
                    chaseTimer += Time.deltaTime;
                }

                if (chaseTimer >= chaseDurationInSeconds)
                {
                    chaseTimer = 0;
                    chaseTimerInProgress = false;
                    SetState(0);
                }
            }
        }
    }
    private bool CanSeeThePlayer()
    {
        if (PlayerHeadIsVisible() | PlayerTorsoIsVisible())
        {
            return true;
        }
        else { return false; }
    }
    private void ExitChasing()
    {
        isChasing = false;
    }
    private bool PlayerHeadIsVisible()
    {
        RaycastHit hitHead;
        Vector3 thisEnemyEyeHeight = transform.position + new Vector3(0f, .40f, 0f);
        Vector3 playerHead = player.transform.position + new Vector3(0f, .85f, 0f);
        Vector3 playerHeadDirection = playerHead - thisEnemyEyeHeight;

        if (Physics.Raycast(thisEnemyEyeHeight, playerHeadDirection, out hitHead, 50f, layersToRaycastAgainst))
        {
            if (hitHead.transform.name != "Player")
            {
                Debug.DrawLine(thisEnemyEyeHeight, hitHead.point, Color.red);
                return false;
            }
            else
            {
                Debug.DrawRay(thisEnemyEyeHeight, playerHeadDirection, Color.green);
                return true;

            }
        }
        return false;
    }
    private bool PlayerTorsoIsVisible()
    {
        RaycastHit hitTorso;
        Vector3 thisEnemyEyeHeight = transform.position + new Vector3(0f, .40f, 0f);
        Vector3 playerTorsoDirection = player.transform.position - thisEnemyEyeHeight;


        if (Physics.Raycast(thisEnemyEyeHeight, playerTorsoDirection, out hitTorso, 50f, layersToRaycastAgainst))
        {
            if (hitTorso.transform.name != "Player")
            {
                Debug.DrawLine(thisEnemyEyeHeight, hitTorso.point, Color.red);
                return false;
            }
            else
            {
                Debug.DrawRay(thisEnemyEyeHeight, playerTorsoDirection, Color.green);
                return true;

            }
        }
        return false;
    }
    #endregion

    #region Attack Methods
    private void InitializeAttackState()
    {
        isAttacking = true;
        Debug.Log("Attack Init");
        //currentState = 2;
        StopEnemyAtCurrentPosition();
    }
    private void AttackState()
    {
        FaceTarget(player.transform.position, .15f, true);

        if (distanceToPlayer > attackRange)
        {
            isAttacking = false;
        }

        if (isAttacking)
        {

            switch (attackSubState) // 0 = attack    |    1 = timer;
            {
                case 0:
                    //Debug.Log("Attack!");
                    ChangeAnimation("simplestEnemyAttacking", true);
                    attackSubState = 1;
                    break;

                case 1:
                    if (currentAnimation != "simplestEnemyAttacking")
                    {
                        ChangeAnimation("simplestEnemyAttacking", true);
                    }

                    if (attackTimer < attackPauseInSeconds)
                    {
                        attackTimer += Time.deltaTime;
                    }
                    else
                    {
                        attackTimer = 0;
                        attackSubState = 0;
                    }
                    break;
            }
        }
    }
    private void ExitAttacking()
    {
        isAttacking = false;
        agent.isStopped = false;
    }
    private void AnimationAttackDamage() //Only called by animation
    {
        //Debug.Log("Attack Event Calling Out!");
        if (distanceToPlayer <= attackRange)
        {
            playerControllerScript.currentHealth -= damage;
        }
    }
    private void AnimationToggleBusy() //Only called via animations
    {
        switch (busyIterator)
        {
            case 0:
                Debug.Log("Currently busy, can't be Interrupted!");
                currentlyBusy = true;
                busyIterator = 1;
                break;
            case 1:
                Debug.Log("I'm ready to do whatever!");
                busyIterator = 0;
                currentlyBusy = false;
                break;
        }
    }
    #endregion

    #region Hurt Methods

    private float hurtTimer;
    private float hurtTimerEnd;
    private void InitializingHurtState()
    {
        //Slow Down Movement to 0.5 x walkspeed.
        currentSpeed = walkingSpeed * 0.5f;

        //Setup hurt Timer.
        hurtTimer = Time.time;
        hurtTimerEnd = Time.time + .5f;
    }
    void HurtState()
    {
        switch (hurtSubState) // 0 = initializing; 0 = start animation; 1 = Hurt Timer; 2 = Go to next State (either chase or attack)
        {
            case 0:        //Initiate hurt animation.
                Debug.Log("Initiating Hurt State");
                ChangeAnimation("simplestEnemyPain", false);
                hurtSubState++;
                break;
            case 1:

                //Go through Hurt Timer
                if (hurtTimer >= hurtTimerEnd)
                {
                    Debug.Log("Hurt Timer Done!");
                    ChangeAnimation(lastAnimation, true);
                    hurtSubState++;
                }
                else if (hurtTimer < hurtTimerEnd)
                {
                    hurtTimer += Time.deltaTime;
                    break;
                }

                break;
            case 2:        //If lastState was chase or attack, resume in respective states. If laststate was idle, goto chase.
                if (lastState == 0) { SetState(1); }
                else { SetState(lastState); }
                hurtSubState = 0;
                break;
        }
    }
    #endregion

    private void ExitHurt()
    {
        hurtSubState = 0;
        mortalityScript.gotHurt = false;
    }

    #endregion

    #region Animation and State Methods

    #region Animation Methods
    public void FaceTarget(Vector3 _targetPos, float _smoothSpeed = .5f, bool _isLockedToYAxis = false)
    {
        Vector3 direction = _targetPos - agent.transform.position;
        Vector3 goalRotation = Quaternion.LookRotation(direction).eulerAngles;

        if (!_isLockedToYAxis)
        {
            newRotation = new Vector3(
            Mathf.SmoothDampAngle(newRotation.x, goalRotation.x, ref xVel, _smoothSpeed),
            Mathf.SmoothDampAngle(newRotation.y, goalRotation.y, ref yVel, _smoothSpeed),
            Mathf.SmoothDampAngle(newRotation.z, goalRotation.z, ref zVel, _smoothSpeed)
            );
        }
        else
        {
            newRotation = new Vector3(
            agent.transform.eulerAngles.x,
            Mathf.SmoothDampAngle(newRotation.y, goalRotation.y, ref yVel, _smoothSpeed),
            agent.transform.eulerAngles.z
            );
        }
        agent.transform.eulerAngles = newRotation;
    }
    void StopEnemyAtCurrentPosition()
    {
        agent.isStopped = true;
    }
    void ChangeAnimation(string newAnimation, bool loopingAnim)
    {
        //stop the same anim from interrupting itself
        if (currentAnimationState == newAnimation)
        {
            Debug.Log("No changed animation!");
            return;
        }

        //I want to reset the position and rotation of the animator to the navAgent's before continuing.
        animator.transform.rotation = agent.transform.rotation;
        animator.transform.position = agent.transform.position;

        if (lastAnimation != currentAnimation) { lastAnimation = currentAnimation; }

        currentAnimation = newAnimation;

        if (loopingAnim)
        {
            //play the next animation state
            animator.Play(newAnimation);

            //reassign current state
            currentAnimationState = newAnimation;
        }
        else
        {
            animator.Play(newAnimation);
        }
    }

    #endregion

    #region State Management
    void SetState(int nextState)
    {
        if (nextState == currentState)
        {
            return;
        }

        //Debug.Log("Last state was: " + lastState + ".");
        //Debug.Log("Current state is " + currentState + ".");
        //Debug.Log("Next state is going to be " + nextState + ".");

        ExitState(currentState);
        lastState = currentState;
        currentState = nextState;
        InitializingNextState(currentState);
    }
    void ExitState(int lastState)
    {
        switch (lastState)
        {
            case 0:
                ExitIdle();
                break;
            case 1:
                ExitChasing();
                break;
            case 2:
                ExitAttacking();
                break;
            case 3:
                ExitHurt();
                break;
        }
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
            case 3:
                InitializingHurtState();
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
            case 3:
                HurtState();
                break;
        }
    }
    #endregion

    #region Debug Functions
    void PrintEnemyName()
    {
        Debug.Log(transform.parent.transform.parent.name);
    }
    #endregion
    #endregion
}
