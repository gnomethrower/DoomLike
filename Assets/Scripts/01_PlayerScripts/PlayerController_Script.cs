using BehaviorDesigner.Runtime.Tasks.Unity.UnityQuaternion;
using System;
using UnityEngine;
using UnityEngine.Windows;

public class PlayerController_Script : MonoBehaviour
{


    //+++++++++++++++++++++++     Changing variables throughout play    +++++++++++++++++++++++++++++
    [Header("Health Settings")]
    public int maxHealth = 100;
    public int currentHealth = 50;

    //+++++++++++++++++++++++     Weapon Inventory    +++++++++++++++++++++++++++++

    public enum StartingGun { Pistol, Shotgun }
    public StartingGun startingGun;

    //[Header("Got Gun Bools")]
    //public bool gotShotgun;
    //public bool gotPistol;
    //public bool gotGrenade;

    [Header("Weapon Ammo Settings")]
    public int shotgunSpareAmmo;
    public int shotgunMaxAmmo;

    public int pistolSpareAmmo;
    public int pistolMaxAmmo;

    public int grenadesSpare;
    public int grenadesMax;

    [Header("Player Movement Speed Changables")]
    [SerializeField] private float currentWalkSpeed = 12f;

    [SerializeField] private float maxWalkSpeed = 12f;
    [SerializeField] private float speedExhaustionMultiplier = 0.75f;
    [SerializeField] private float sprintMultiplierValue = 1.25f;
    [SerializeField] private float gravity = -12f;

    public float currentStamina = 100f;
    public float maxStamina = 100f;
    public float staminaLastFrame;
    [SerializeField] private float timeToRecoverStaminaSeconds = 2f;
    [SerializeField] private float staminaRecoveryDuration;
    public float staminaTimer;
    private bool canRecoverStamina = true;
    private float timeToRecover;
    [SerializeField] private float staminaDepletionMultiplier;
    [SerializeField] private float staminaRefillMultiplier;
    [SerializeField] private float staminaExhaustionThreshold;
    private int sprintState = 0;
    private bool staminaTimerRunning = false;

    private float playerSpeed;
    private float sprintMultiplier = 1f;

    public float jumpHeight = .0001f;
    public float exhaustedJumpHeightMultiplier = 0.5f;
    private float jumpStamCost = 10f;

    public static float xAxis;
    public static float zAxis;

    public static float spreadMultiplier;
    float jumpingSpread;

    Collider playerCollider;

    [Header("Checkbools")]
    [SerializeField] private bool isGrounded;
    [SerializeField] public bool hasDied;
    [SerializeField] private bool isMoving;
    [SerializeField] public bool isExhausted;

    Quaternion currentLeaningRotation = Quaternion.identity;
    Quaternion targetRotation;
    [SerializeField] private float leanRotationTarget = 15f;

    //[SerializeField] private bool isSprinting;

    #region Change in Inspector
    #endregion

    #region References
    GameEventManager gameEventManagerScript;
    [SerializeField] private int flashlightIntensity;
    public CharacterController controller;
    public Vector3 playerVelocity;
    public Transform groundCheck;
    public float groundCheckSize = 0.5f;
    public LayerMask groundMask;
    public Animator viewCamAnim;
    public GameObject deathScreen;
    public AudioController_Script audioInstance;
    public GameObject uiCanvas;
    private GameObject flashLightObject;
    private Light flashLightSpotlight;
    private int flashLightMode = 0;
    [Tooltip("a value between 40000 and 80000 works well.")]
    [SerializeField] private int flashLightMaxIntensity;

    private GameObject leaningPivotPoint;
    [SerializeField] private float maxLeanAngleDegrees;

    private float currentLocalEulerAngleZ;
    private float targetLocalEulerAngleZ;
    private int leanDirection; // 0: none, 1: left, 2: right

    private Vector3 lastKnownPosition;
    private Vector3 currentPosition;
    #endregion

    #region Weapon Variables
    #endregion

    #region Movement Variables
    #endregion

    #region Events
    public static Action OnPlayerDeath;
    public static Action OnPlayerStaminaExhaustion;
    public static Action OnPlayerStaminaRecovery;
    #endregion

    private void Awake()
    {

        flashLightObject = GameObject.Find("FlashLight");
        flashLightSpotlight = flashLightObject.GetComponent<Light>();

        if (flashLightSpotlight == null) { Debug.Log("noFlashlight!"); }

        deathScreen = GameObject.Find("DeathScreen");
        gameEventManagerScript = GameObject.Find("GameEventManager").GetComponent<GameEventManager>();

        playerCollider = this.GetComponent<Collider>();

        leaningPivotPoint = GameObject.Find("LeaningPivotPoint");
        if (leaningPivotPoint == null) { Debug.Log("We have no leaningPivotPoint"); }
    }

    private void Start()
    {
        currentStamina = maxStamina;
        lastKnownPosition = transform.position;
        currentPosition = transform.position;
        timeToRecover = timeToRecoverStaminaSeconds;
    }

    void Update()
    {
        DebugMsg();
        if (!hasDied)
        {
            StaminaTimer();
            CheckForStaminaUse();
            CheckForMovement();
            CheckForDeath();
            GetInput();
            Flashlight();
            CalculateSpreadMP();
            MoveAndJump();
        }
    }

    private void FixedUpdate()
    {
        //Player Gravity Calculation
        if (!isGrounded) playerVelocity.y += gravity * Time.deltaTime;
    }

    void DebugMsg()
    {
        //if(isMoving)
        //{
        //    Debug.Log(playerSpeed);
        //    Debug.Log(sprintMultiplier);
        //}
    }

    void GetInput()
    {
        xAxis = UnityEngine.Input.GetAxis("Horizontal");
        zAxis = UnityEngine.Input.GetAxis("Vertical");

        Sprinting();

        // Updating Rotation
        Leaning();
    }

    void Flashlight()
    {
        if (UnityEngine.Input.GetKeyDown(KeyCode.F))
        {
            flashLightMode++;
            if (flashLightMode > 2) { flashLightMode = 0; }

            switch (flashLightMode)
            {
                case 0:
                    Debug.Log("Flaslight = off");
                    flashLightSpotlight.intensity = 0;
                    break;
                case 1:
                    Debug.Log("Flaslight = half-light");
                    flashLightSpotlight.intensity = 75000 / 2;
                    break;
                case 2:
                    Debug.Log("Flaslight = full force");
                    flashLightSpotlight.intensity = 75000;
                    break;
            }

        }
    }

    void Sprinting() // Refactor! When stamina is used, start stamina timer!
    {
        if (!isExhausted && !staminaTimerRunning) StaminaRecovery();

        // Overflow Check over 100
        if (currentStamina > 100f) 
        {
            currentStamina = 100f;
        }

        // Overflow Check under 0
        if (currentStamina < 0) 
        {
            currentStamina = 0;
        }

        /*
        //Release Sprinting Button Check
        if (UnityEngine.Input.GetButtonUp("Sprint") && !staminaTimerRunning && !isExhausted)
        {
            staminaTimer = 0f;
        }
        */

        switch (sprintState) //0: fresh;   1: exhausted;   2: reset to fresh
        {
            case 0:
                
                if (UnityEngine.Input.GetButton("Sprint") && isMoving && currentStamina > 0)
                {
                    StartStaminaTimer(timeToRecover);
                    currentStamina -= (Time.deltaTime * staminaDepletionMultiplier);
                    sprintMultiplier = sprintMultiplierValue;

                    if (currentStamina <= 0) // When currentStamina is entirely depleted, exhaustion sets in. The player moves more slowly.
                    {
                        //Setting Exhausted Parameters
                        currentWalkSpeed = maxWalkSpeed * speedExhaustionMultiplier;
                        isExhausted = true;
                        canRecoverStamina = false;
                        OnPlayerStaminaExhaustion?.Invoke();
                        sprintState = 1;
                    }
                }
                else
                {
                    if(currentStamina < 100f && canRecoverStamina) // Stamina refills at normal rate.
                    {
                        sprintMultiplier = 1f;
                    }
                }
                break;

            case 1:
                if (!staminaTimerRunning) StartStaminaTimer(timeToRecoverStaminaSeconds * 2f);
                //Resetting, if Stamina refilled to threshold.
                break;

            case 2:
                OnPlayerStaminaRecovery?.Invoke();
                Debug.Log("Reset to base Speed");
                sprintMultiplier = 1f;
                currentWalkSpeed = maxWalkSpeed;
                isExhausted = false;
                timeToRecover = timeToRecoverStaminaSeconds;
                sprintState = 0;
                break;
        }

    }

    void StaminaRecovery()
    {
            currentStamina += (Time.deltaTime * staminaRefillMultiplier);
        if (isExhausted && currentStamina <= staminaExhaustionThreshold)
        {
            sprintState = 2;
        }
    }

    void StartStaminaTimer(float timer)
    {
        staminaTimer = 0f;
        staminaTimerRunning = true;
        timeToRecover = timer;
    }

    void StaminaTimer()
    {
        if (staminaTimerRunning)
        {
            if(staminaTimer < timeToRecover)
            { 
                staminaTimer += Time.deltaTime; 
            }
            if(staminaTimer >= timeToRecover)
            {
                EndStaminaTimer();
            } 
        }

        /*
        //Debug.Log(timeToRecover);
        //Debug.Log(staminaTimer);

        //if (!staminaTimerRunning && staminaTimer != 0f)
        //{
        //    staminaTimer = 0f;
        //}

        //if (staminaTimer < timeToRecover)
        //{
        //    staminaTimerRunning = true;
        //    Debug.Log("Stamina timer running!");
        //    staminaTimer += Time.deltaTime;
        //    canRecoverStamina = false;
        //}
        //else if (staminaTimer >= timeToRecover) //timer to delay stamina recovery
        //{
        //    if (staminaTimerRunning)
        //    {
        //        staminaTimerRunning = false;
        //        Debug.Log("Stamina Timer Done!");
        //    }

        //    if (!staminaTimerRunning)
        //    {
        //        staminaTimer = 0f;
        //    }

            //if(!canRecoverStamina) canRecoverStamina = true;
            //if(staminaTimer != timeToRecover) staminaTimer = timeToRecover;
        }
       */ 
    }

    void EndStaminaTimer()
    {
        staminaTimerRunning = false;
        //staminaTimer = 0f;
        sprintState = 2;
    }

    void CalculateSpreadMP()
    {
        spreadMultiplier = Mathf.Clamp(((Mathf.Abs(zAxis)) + (Mathf.Abs(xAxis))), 0, 1) + jumpingSpread;
    }

    void CheckForStaminaUse() // if stamina is used, set stamina recovery timer to zero. Wait until the usage stops and THEN start the timer.
    {
        //if (isExhausted && staminaRecoveryDuration != timeToRecoverStaminaSeconds * 2f)
        //{
        //    staminaRecoveryDuration = timeToRecoverStaminaSeconds * 2f;
        //}  else {
        //    staminaRecoveryDuration = timeToRecoverStaminaSeconds;
        //}

        if (staminaLastFrame == currentStamina)
        {
            Debug.Log("Stagnant stamina!");
        }

        if (staminaLastFrame < currentStamina)
        {
            Debug.Log("Rising stamina!");
        }

        if (staminaLastFrame > currentStamina)
        {
            Debug.Log("Depleting stamina!");

            if (!staminaTimerRunning)
            {
                StartStaminaTimer(timeToRecover);
            }
            else
            {
                staminaTimer = 0f;
            }
        }
        staminaLastFrame = currentStamina;
    }

    void MoveAndJump()
    {
        //jumping with groundcheck
        isGrounded = Physics.CheckBox(groundCheck.position, new Vector3(groundCheckSize, groundCheckSize, groundCheckSize), groundCheck.transform.rotation, groundMask);
        if (isGrounded) jumpingSpread = 1f;
        else jumpingSpread = 3f;

        if (isGrounded && playerVelocity.y < 0)
        {
            playerVelocity.y = -2f;
        }

        if (UnityEngine.Input.GetButtonDown("Jump") && isGrounded)
        {
            currentStamina -= jumpStamCost;
            if(!isExhausted) playerVelocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            if(isExhausted) playerVelocity.y = Mathf.Sqrt((jumpHeight * exhaustedJumpHeightMultiplier) * -2f * gravity);
        }

        //normalizing vector movement, so diagonal movement isn't twice as fast.
        Vector3 move = transform.right * xAxis + transform.forward * zAxis;
        if (move.magnitude > 1)
        {
            move /= move.magnitude;
        }

        // playerSpeed is always multiplied by sprintmultiplier
        playerSpeed = currentWalkSpeed * sprintMultiplier;

        //setting up the movement with the playerspeed and correcting for executiontime
        controller.Move((move * playerSpeed) * Time.deltaTime);

        //gravity - now found in fixedUpdate!
        //playerVelocity.y += gravity * Time.deltaTime;

        //moving
        controller.Move(playerVelocity * Time.deltaTime);
    }

    private void CheckForMovement()
    {
        if (lastKnownPosition == transform.position) isMoving = false;
        else isMoving = true;
        lastKnownPosition = transform.position;
    }

    public void GetHealth(int healAmount)
    {
        currentHealth += healAmount;
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
    }

    public void CheckForDeath()
    {
        if (currentHealth <= 0)
        {
            if (OnPlayerDeath == null)
            {
                Debug.Log("OnPlayerDeath Action not found!");
            }
            else
            {
                DieFunctions();
                OnPlayerDeath?.Invoke();
            }
        }
    }

    private void DieFunctions()
    {
        hasDied = true;
        playerCollider.enabled = false;
        //audioInstance.PlayPlayerDeath();
    }

    private void Leaning()
    {

        if (UnityEngine.Input.GetKey(KeyCode.Q) || UnityEngine.Input.GetKey(KeyCode.E))
        {
            if (UnityEngine.Input.GetKey(KeyCode.Q))
            {
                leanDirection = 1;
            }
            else
            if (UnityEngine.Input.GetKey(KeyCode.E))
            {
                leanDirection = 2;
            }
        }
        else leanDirection = 0;


        switch (leanDirection)  // 0: none, 1: left, 2: right
        {
            case 0:
                if (targetLocalEulerAngleZ != 0f) targetLocalEulerAngleZ = 0f;
                break;
            case 1:
                if (targetLocalEulerAngleZ != -maxLeanAngleDegrees) targetLocalEulerAngleZ = maxLeanAngleDegrees;
                break;
            case 2:
                if (targetLocalEulerAngleZ != maxLeanAngleDegrees) targetLocalEulerAngleZ = -maxLeanAngleDegrees;
                break;
        }

        if (currentLocalEulerAngleZ != targetLocalEulerAngleZ)
        {
            currentLocalEulerAngleZ = Mathf.LerpAngle(currentLocalEulerAngleZ, targetLocalEulerAngleZ, .1f);
            leaningPivotPoint.transform.localEulerAngles = new Vector3(0, 0, currentLocalEulerAngleZ);
        }
    }

}


