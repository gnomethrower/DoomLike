using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Carl_State_Script : MonoBehaviour
{
    //3 zones:
    //1. Aggro Zone
    //2. Wary Zone
    //3. Hearing Range

    //3 States:
    //0 Peaceful Idle
    //1 Wary
    //2 Aggressive

    //Peaceful Idle: rotates around, gazing, paying you no further attention.
    //when you enter Carl's Wary Zone, he becomes wary.
    //If you fire while in his Hearing Range he becomes wary.

    //Wary: You get too close, he locks onto you, maybe making a hissing sound to warn you off!
    // if he is wary and you are not in his wary zone for 5s, he becomes idle again.

    //Aggressive: He runs at you, trying to kill you.
    //if you shoot him, he becomes aggressive.
    //if you stay inside the wary range for 1s he will hiss again, after 2s he will growl after 3s he will become aggressive.
    //if you enter Aggro Zone, he gets aggressive immediately.
    //he will never exit Aggressive state, unless player dies.


    //Variables to fiddle with

    public int startingState = 0;
    public int state;

    [Header("Rotation Angles")]
    [SerializeField] float currentAngle;
    [SerializeField] float startingAngle;
    [SerializeField] float targetAngle;

    [Header("Timed Variables")]

    public float minCoolDown, maxCooldown;
    private float coolDown;
    private float coolDownControl;
    public float maxGazingVariation;
    public float turnSpeed;
    public float rotatingAngle;
    public float sightRange, attackRange;

    Quaternion _targetRotation;

    //Checkbools
    public bool rotationFinished;
    public bool isCooldownFinished;
    public bool hasPlayedChangeSound;
    bool isSpeedSet;

    public float waryToPeaceful;

    //objects
    public Transform playerTransform;
    GameObject playerObj;
    GameObject audioController;
    AudioController_Script audioController_Script;

    //masks
    public LayerMask whatIsGround, whatIsPlayer;

    //patrolling vars
    public float calmSpeed;
    public float aggroSpeed;
    public Vector3 walkPoint;
    bool isWalkPointSet = false;
    public float walkPointRange, maxWalkPointCooldown;
    public bool playerInSightRange, playerInAttackRange;
    bool isStateChanged;

    RaycastHit hitCollider;
    RaycastHit hitGroundCheck;
    NavMeshPath path;
    public NavMeshAgent agent;

    [Header("Debug Variables")]
    public float debugLineDuration;

    private void Start()
    {
        audioController = GameObject.FindGameObjectWithTag("AudioController");
        audioController_Script = audioController.GetComponent<AudioController_Script>();
    }

    private void Awake()
    {
        playerObj = GameObject.FindGameObjectWithTag("Player");
        playerTransform = playerObj.transform;
        agent = GetComponent<NavMeshAgent>();
        startingAngle = transform.rotation.y;
        SetRotationTarget(0);
        int state = startingState;
    }

    private void OnDestroy()
    {
        audioController_Script.PlayEnemyDeath();
    }
    private void Update()
    {
        StateChangeCheck();
        if (state == 0) Peaceful();
        if (state == 1) Wary();
        if (state == 2) Aggro();
        SetSpeed();
    }

    void StateChangeCheck()
    {
        // Check if the state has been changed since the last time the frame has been called!
        //if still the same state, skip the rest of the frame update.
    }

    void Peaceful()
    {

        PeacefulPatroling();

        //transform.rotation = Quaternion.RotateTowards(transform.rotation, _targetRotation, turnSpeed * Time.deltaTime);

        //if (transform.rotation == _targetRotation && !rotationFinished) rotationFinished = true;

        //if (rotationFinished && coolDownControl > 0f) coolDownControl -= Time.deltaTime;

        //if (rotationFinished && !isCooldownFinished && coolDownControl <= 0f)
        //{
        //    isCooldownFinished = true;
        //    float newIdleAngle = Random.Range(-maxGazingVariation, maxGazingVariation);
        //    SetRotationTarget(newIdleAngle);
        //}

        //PeacefulPatrolling
        //Determine New Waypoint
        //Turn Towards the Waypoint
        //MoveTowards the Waypoint

        //Create checksphere.
        //OnTriggerStay cooldown.

    }

    void PeacefulPatroling()
    {
        if (!isWalkPointSet)
        {
            SearchWalkPoint();
        }

        if (isWalkPointSet)
        {
            agent.SetDestination(walkPoint);
        }

        Vector3 distanceToWalkPoint = transform.position - walkPoint;
        if (distanceToWalkPoint.magnitude < 2f) isWalkPointSet = false;
    }

    void SearchWalkPoint()
    {
        Vector3 currentPos = transform.position;
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(currentPos.x + randomX, currentPos.y, currentPos.z + randomZ);

        if (Physics.Raycast(currentPos, walkPoint - currentPos, out hitCollider, 2f * walkPointRange, whatIsGround))
        {
            Vector3 walkingVector = walkPoint - currentPos;
            Debug.Log(walkingVector);
            //walkingVector.y = 0f;
            walkPoint = hitCollider.point - walkingVector.normalized;
            Debug.DrawLine(currentPos, walkPoint, Color.red, debugLineDuration);
        }


        if (Physics.Raycast(walkPoint, -Vector3.up, out hitGroundCheck, 2f, whatIsGround))
        {
            Debug.DrawLine(walkPoint, hitGroundCheck.point, Color.green, debugLineDuration);
            isWalkPointSet = true;
        }
    }

    void SetRotationTarget(float newRotAngle)
    {
        rotationFinished = false;
        isCooldownFinished = false;

        coolDown = Random.Range(minCoolDown, maxCooldown);
        coolDownControl = coolDown;

        startingAngle = transform.eulerAngles.y;
        currentAngle = startingAngle;

        _targetRotation = Quaternion.Euler(0, newRotAngle, 0);
    }

    void Wary()
    {
        if (!isSpeedSet) SetSpeed();
        Vector3 angleToPlayer = playerObj.transform.position - transform.position;
        angleToPlayer.y = 0f;
        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(angleToPlayer), 2 * turnSpeed * Time.deltaTime);
        //PlayWarySound

        //To stop him patrolling and gaze suspiciously
        if (state == 1 && isWalkPointSet)
        {
            walkPoint = transform.position;
            agent.SetDestination(walkPoint);
            isWalkPointSet = false;
        }
    }

    void Aggro()
    {

        agent.SetDestination(playerObj.transform.position);
    }

    void SetSpeed()
    {
        if (state == 0 || state == 1)
        {
            agent.speed = calmSpeed;
        }

        if (state == 2)
        {
            agent.speed = aggroSpeed;
        }
    }
}
