using BehaviorDesigner.Runtime.Tasks.Unity.UnityInput;
using BehaviorDesigner.Runtime.Tasks.Unity.UnityNavMeshAgent;
using BehaviorDesigner.Runtime.Tasks.Unity.UnityQuaternion;
using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Windows;
public class PlayerController_Script : MonoBehaviour

{
    // Lambda Bools
    private bool ShouldCrouch => UnityEngine.Input.GetKeyDown(crouchKey) && canCrouch && playerController.isGrounded;    //A bool with a prerequisite attached to it. After the "=>" (Lambda Expression), the conditions are shown that need to be fulfilled in order for this bool to be positive;
    private bool ShouldJump => UnityEngine.Input.GetKeyDown(jumpKey) && playerController.isGrounded && !isCrouching;
    private bool ShouldLeanRight => UnityEngine.Input.GetKey(leanRightKey);
    private bool ShouldLeanLeft => UnityEngine.Input.GetKey(leanLeftKey);

    [Header("Abilities")]
    [SerializeField] private bool canCrouch = true;
    [SerializeField] private bool canLean = true;
    [SerializeField] private bool canJump = true;
    [SerializeField] private bool canSprint = true;
    [SerializeField] private bool canMove = true;

    [Header("Health Settings")]
    public int maxHealth = 100;
    public int currentHealth = 50;
    public int criticalHealthThreshold = 25;
    private int healthLastFrame = 100;
    [SerializeField] bool healthCritical = false;

    [Header("Weapons")]
    //Equipped guns
    public StartingGun startingGun;
    public enum StartingGun { Pistol, Shotgun }

    [Header("Flashlight")]
    [SerializeField] private int flashlightIntensity;
    [Tooltip("a value between 40000 and 80000 works well.")]
    [SerializeField] private int flashLightMaxIntensity;
    private int flashLightMode = 0;

    [Header("Ammo")]
    public int shotgunSpareAmmo;
    public int shotgunMaxAmmo;
    public int pistolSpareAmmo;
    public int pistolMaxAmmo;
    public int grenadesSpare;
    public int grenadesMax;

    [Header("Player Movement")]
    [SerializeField] private int sprintState = 0;
    [SerializeField] private float playerSpeed;
    private float currentWalkSpeed = 5f;
    [SerializeField] private float freshWalkSpeed = 5f;
    [SerializeField] private float exhaustedWalkspeed = 3.5f;
    [SerializeField] private float sprintMultiplierValue = 2f;
    [SerializeField] private float activeSprintMultiplier = 1f;
    public Vector3 playerVelocity;
    private Vector3 lastKnownPosition;
    private Vector3 currentPosition;

    [Header("Leaning")]
    [SerializeField] private AnimationCurve leanAnimationCurve;
    [SerializeField] private bool isLeaning = false;
    [SerializeField] private float maxLeanAngleDegrees;
    [SerializeField] private float leaningDuration;
    [SerializeField] private float timeToLean;
    [SerializeField] private float leaningTimeElapsed;
    [SerializeField] private float targetLocalEulerAngleZ;
    [SerializeField] private float currentLocalEulerAngleZ;
    [SerializeField] private int currentLeaningState; // 0: none, 1: left, 2: right
    [SerializeField] private int lastLeaningState;

    [Header("Stamina")]
    public float staminaTimerSec;
    public float currentStamina = 100f;
    public float maxStamina = 100f;
    public float staminaLastFrame;
    [SerializeField] private float staminaTimeToRecover = 2f;
    [SerializeField] private float staminaRecTimeFresh = 2f;
    [SerializeField] private float staminaRecTimeExhausted = 2f;
    [SerializeField] private float staminaDepletionMultiplier;
    [SerializeField] private float staminaRefillMultiplier;
    [SerializeField] private float staminaExhaustionThreshold;
    [SerializeField] private bool canRecoverStamina = true;
    [SerializeField] private bool staminaTimerRunning = false;

    [Header("Jumping")]
    [SerializeField] private float jumpingSpread = 1f;
    [SerializeField] private float jumpHeight = 1f;
    [SerializeField] private float gravity = -12f;
    [SerializeField] private float jumpStamCost = 10f;
    public static float xAxis;
    public static float zAxis;
    public static float spreadMultiplier;
    private float exhaustedJumpHeightMultiplier = 0.5f;

    [Header("Crouching")]
    [SerializeField] private AnimationCurve couchAnimCurve;
    [SerializeField] private bool isCrouching = false;
    [SerializeField] private float standingHeight = 1.75f;
    [SerializeField] private float crouchingHeight = 1f;
    [SerializeField] private Vector3 standingCenterPoint = new Vector3(0f, .75f, 0f);
    [SerializeField] private Vector3 crouchingCenterPoint = new Vector3(0f, .75f, 0f);
    [SerializeField] private float timeToCrouch = 1f;
    [SerializeField] private float safetyBufferForFunsies = 0.001f;
    private bool inCrouchingAnimation = false;

    [Header("Controls")]
    [SerializeField] private KeyCode crouchKey = KeyCode.LeftControl;
    [SerializeField] private KeyCode jumpKey = KeyCode.Space;
    [SerializeField] private KeyCode sprintKey = KeyCode.LeftShift;
    [SerializeField] private KeyCode leanLeftKey = KeyCode.Q;
    [SerializeField] private KeyCode leanRightKey = KeyCode.E;
    [SerializeField] private KeyCode flashLightKey = KeyCode.F;

    [Header("Object References")]
    public GameObject deathScreen;
    public GameObject uiCanvas;
    public CharacterController playerController;
    public Animator viewCamAnim;
    public AudioController_Script audioInstance;
    private GameObject leaningPivotPoint;
    private GameObject flashLightObject;
    private Light flashLightSpotlight;
    private Collider playerCollider;
    private CapsuleCollider playerCapsuleCollider;
    private GameHandler gameEventManagerScript;

    [Header("Events")]
    public static Action Action_PlayerDeath;
    public static Action Action_PlayerStaminaExhausted;
    public static Action Action_PlayerStaminaRecovered;
    public static Action Action_PlayerHealthCritical;
    public static Action Action_PlayerHealthRecoveredFromCritical;

    [Header("Checkbools")]
    [SerializeField] public bool isGrounded;
    [SerializeField] public bool isMoving;
    [SerializeField] public bool isExhausted;
    [SerializeField] public bool leanAnimPlaying;
    public bool playerHasDied;
    private bool debugModeActive = false;

    private void Awake()
    {
        flashLightObject = GameObject.Find("FlashLight");
        flashLightSpotlight = flashLightObject.GetComponent<Light>();
        
        deathScreen = GameObject.Find("DeathScreen");
        playerCollider = this.GetComponent<Collider>();
        playerCapsuleCollider = this.GetComponent <CapsuleCollider>();
        leaningPivotPoint = GameObject.Find("LeaningPivotPoint");
        playerController = this.GetComponent<CharacterController>();

        gameEventManagerScript = GameObject.Find("GameHandler").GetComponent<GameHandler>();

        currentStamina = maxStamina;
        lastKnownPosition = transform.position;
        currentPosition = transform.position;
        standingHeight = playerController.height;
        standingCenterPoint = playerController.center;
        lastLeaningState = currentLeaningState;

        Action_PlayerStaminaExhausted += OnPlayerStaminaExhasted;
        Action_PlayerStaminaRecovered += OnPlayerStaminaRecovered;
        Action_PlayerDeath += OnPlayerDeath;
        Action_PlayerHealthCritical += OnPlayerHealthCritical;
        Action_PlayerHealthRecoveredFromCritical += OnPlayerHealthRecoveredFromCritical;

        EventManagerMaster.Action_ToggleDebugMode += OnToggleDebugMode;

        #region nullchecks
        //if (deathScreen == null) Debug.Log("No deathScreen!");
        if (flashLightSpotlight == null) Debug.Log("noFlashlight!"); 
        if (leaningPivotPoint == null) Debug.Log("We have no leaningPivotPoint");
        #endregion
    }

    private void Start()
    {

    }

    void Update()
    {
        if (ShouldLeanLeft) { Debug.Log("ShouldLeanLeft"); }
        if (ShouldLeanRight) { Debug.Log("ShouldLeanRight"); }
        Debug.Log(targetLocalEulerAngleZ);
        if (debugModeActive)
        {
            DebugMsg();
            DebugFunctions();
        }

        if (!playerHasDied)
        {
            StaminaTimer();
            CheckForHealthChanges();
            CheckForStaminaChanges();
            CheckForMovement();
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
        
    }

    void GetInput()
    {
        xAxis = UnityEngine.Input.GetAxis("Horizontal");
        zAxis = UnityEngine.Input.GetAxis("Vertical");
        Sprinting();
        if(canLean) HandleLeaning();
        if (canCrouch)
        {
            HandleCrouching();
        }
    }

    void Flashlight()
    {
        if (UnityEngine.Input.GetKeyDown(flashLightKey))
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
        
        if (canRecoverStamina) StaminaRecovery();

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

        // When currentStamina is entirely depleted, exhaustion sets in. The player moves more slowly.
        if (!isExhausted && currentStamina <= 0) 
        {
            Action_PlayerStaminaExhausted?.Invoke();
        }

        switch (sprintState) //0: fresh;   1: exhausted;   2: reset to fresh
        {
            case 0:
                if (canSprint)
                {
                    if (UnityEngine.Input.GetKey(sprintKey) && isMoving && currentStamina > 0 && !isExhausted)
                    {
                        if (!staminaTimerRunning) StartStaminaTimer(staminaRecTimeFresh);
                        currentStamina -= (Time.deltaTime * staminaDepletionMultiplier);
                        activeSprintMultiplier = sprintMultiplierValue;
                    }
                    else
                    {
                        activeSprintMultiplier = 1f;
                    }
                }
                break;

            case 1:
                if (currentStamina >= staminaExhaustionThreshold)
                {
                    sprintState = 2;
                }
                break;

            case 2:
                Action_PlayerStaminaRecovered?.Invoke();
                break;
        }
    }

    /* PseudoCode Sprinting & Stamina:
     * 
     * Stamina = 100-0: You can sprint. If you sprint you lose stamina at a constant rate and your player character moves faster. If you stop sprinting, your stamina starts recovering after 2s at normal speed.
     * Stamina = 0: If stam reaches 0, you cannot sprint. Exhaustion sets in. Your movement speed slows, and stamina recovery slows. Your stamina starts recovering after 4s at slower speed.
     * Stamina recovers to 50: Once Stamina >= 50 again, exhaustion stops, your movement is normal again. Your stamina recovers at the normal speed. Stamina recovery timer is back at 2s.
     * 
     * */

    void StaminaRecovery()
    {
        if (isExhausted)
        {
            currentStamina += (Time.deltaTime * staminaRefillMultiplier/2);
        } else
        {
            currentStamina += (Time.deltaTime * (staminaRefillMultiplier));
        }
    }
    
    void OnPlayerStaminaExhasted() //Called on Action_PlayerStaminaExhausted
    {
        activeSprintMultiplier = 1f;
        isExhausted = true;
        currentWalkSpeed = exhaustedWalkspeed;
        sprintState = 1;
        StartStaminaTimer(staminaRecTimeExhausted);
    }

    void OnPlayerStaminaRecovered()//Called by Action_PlayerStaminaRecovered
    {
        isExhausted = false;
        currentWalkSpeed = freshWalkSpeed;
        sprintState = 0;
    }

    void StartStaminaTimer(float timer)
    {
        canRecoverStamina = false;
        staminaTimerSec = 0f;
        staminaTimerRunning = true;
        staminaTimeToRecover = timer;

    }

    void StaminaTimer()
    {
        if (staminaTimerRunning)
        {
            if (staminaTimerSec < staminaTimeToRecover)
            {
                staminaTimerSec += Time.deltaTime;
            }
            if (staminaTimerSec >= staminaTimeToRecover)
            {
                EndStaminaTimer();
            }
        }
    }

    void EndStaminaTimer()
    {
        if(!isExhausted)staminaTimeToRecover = staminaRecTimeFresh;
        if (isExhausted) staminaTimeToRecover = staminaRecTimeExhausted;
        staminaTimerSec = 0f;
        staminaTimerRunning = false;
        canRecoverStamina = true;
    }

    void CheckForStaminaChanges() // if stamina is used, set stamina recovery timer to zero. Wait until the usage stops and THEN start the timer.
    {
        if (staminaLastFrame == currentStamina)
        {
            //Debug.Log("Stagnant stamina!");
        }
        if (staminaLastFrame < currentStamina)
        {
            //Debug.Log("Rising stamina!");
        }
        if (staminaLastFrame > currentStamina)
        {
            //Debug.Log("Depleting stamina!");
            if (!staminaTimerRunning)
            {
                StartStaminaTimer(staminaRecTimeFresh);
            }
            else
            {
                staminaTimerSec = 0f;
            }
        }
        staminaLastFrame = currentStamina;
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

    void CheckForHealthChanges()
    {
        // Function is checking if we lost or gained health compared to last frame.

        if (healthLastFrame == currentHealth) // serves as a gate to the rest of the function - we don'crouchTime need to check the rest if the health hasn'crouchTime changed.
        {
            return;
        }

        if (healthLastFrame < currentHealth) // Gaining Health
        {
            if (healthCritical && currentHealth > criticalHealthThreshold)
            {
                Action_PlayerHealthRecoveredFromCritical?.Invoke();
            }
        }

        if (healthLastFrame > currentHealth) // Losing Health
        {            
            if (currentHealth <= 0)
            {
                if (Action_PlayerDeath == null)
                {
                    Debug.Log("OnPlayerDeath Action not found!");
                }
                else
                {
                    Action_PlayerDeath?.Invoke();
                }
            }
        }

        if (!healthCritical && currentHealth <= criticalHealthThreshold)
        {
            Action_PlayerHealthCritical?.Invoke();
        }

        healthLastFrame = currentHealth;
    }

    private void OnPlayerHealthCritical()
    {
        healthCritical = true;
    }

    private void OnPlayerHealthRecoveredFromCritical()
    {
        healthCritical = false;
    }

    private void OnPlayerDeath()
    {

        playerHasDied = true;
        playerCollider.enabled = false;
        //audioInstance.PlayPlayerDeath();
    }

    private void HandleLeaning()
    {
        //check which leaning direction you went for.
        //If you press both LeanKeys, we don't lean.
        //if(ShouldLeanLeft || ShouldLeanRight) { isLeaning = true; } else { isLeaning = false; }

        isLeaning = (ShouldLeanLeft || ShouldLeanRight) ? true : false;

        if (isLeaning)
        {
            if (ShouldLeanLeft)
            {
                if (currentLeaningState != 1)
                {
                    leaningTimeElapsed = 0f;
                    currentLeaningState = 1;
                }
            }
            if (ShouldLeanRight)
            {
                if (currentLeaningState != 2)
                {
                    leaningTimeElapsed = 0f;
                    currentLeaningState = 2;
                }
            }
        }
        else if (currentLeaningState != 0)
        {
            leaningTimeElapsed = 0f;
            currentLeaningState = 0;
        }

        if (currentLeaningState != lastLeaningState)
        {
            // Setting what happens in the leaning state
            switch (currentLeaningState)  // 0: none, 1: left, 2: right
            {
                case 0:
                    targetLocalEulerAngleZ = 0f;
                    StopCoroutine(LeaningLerped());
                    StartCoroutine(LeaningLerped());
                    break;
                case 1:
                    targetLocalEulerAngleZ = maxLeanAngleDegrees;
                    StopCoroutine(LeaningLerped());
                    StartCoroutine(LeaningLerped());
                    break;
                case 2:
                    targetLocalEulerAngleZ = -maxLeanAngleDegrees;
                    StopCoroutine(LeaningLerped());
                    StartCoroutine(LeaningLerped());
                    break;
            }
        }

        lastLeaningState = currentLeaningState;
    }

    private IEnumerator LeaningLerped()
    {
        Debug.Log("Starting LerpRoutine");
        //float timeElapsed = 0f;
        leaningTimeElapsed = 0f;
        leanAnimPlaying = true;

        while (leaningTimeElapsed < leaningDuration)
        {
            float leanTime = leaningTimeElapsed / leaningDuration;
            leanTime = leanAnimationCurve.Evaluate(leanTime);
            currentLocalEulerAngleZ = Mathf.LerpAngle(currentLocalEulerAngleZ, targetLocalEulerAngleZ, leanTime);
            leaningPivotPoint.transform.localEulerAngles = new Vector3(0f, 0f, currentLocalEulerAngleZ);
            leaningTimeElapsed += Time.deltaTime;
            yield return null;
        }

        leaningTimeElapsed = 0f;
        currentLocalEulerAngleZ = targetLocalEulerAngleZ;
        leaningPivotPoint.transform.localEulerAngles = new Vector3(0f, 0f, currentLocalEulerAngleZ);
        leanAnimPlaying = false;
    }



    private void CalculateSpreadMP()
    {
        spreadMultiplier = Mathf.Clamp(((Mathf.Abs(zAxis)) + (Mathf.Abs(xAxis))), 0, 1) + jumpingSpread;
    }

    private void MoveAndJump()
    {
        //jumping with groundcheck
        //isGrounded = Physics.CheckBox(groundCheck.position, new Vector3(groundCheckSize, groundCheckSize, groundCheckSize), groundCheck.transform.rotation, groundMask);

        if (playerController.isGrounded) jumpingSpread = 1f;
        else jumpingSpread = 3f;

        if (playerController.isGrounded && playerVelocity.y < 0)
        {
            playerVelocity.y = -2f;
        }

        if (canJump && ShouldJump)
        {
            currentStamina -= jumpStamCost;
            if (!isExhausted) playerVelocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            if (isExhausted) playerVelocity.y = Mathf.Sqrt((jumpHeight * exhaustedJumpHeightMultiplier) * -2f * gravity);
        }

        //normalizing vector movement, so diagonal movement isn'crouchTime twice as fast.
        Vector3 move = transform.right * xAxis + transform.forward * zAxis;
        if (move.magnitude > 1)
        {
            move /= move.magnitude;
        }

        // playerSpeed is always multiplied by sprintmultiplier
        playerSpeed = currentWalkSpeed * activeSprintMultiplier;

        //setting up the movement with the playerspeed and correcting for executiontime
        playerController.Move((move * playerSpeed) * Time.deltaTime);

        playerController.Move(playerVelocity * Time.deltaTime);
    }

    private void DebugFunctions()
    {
        if (UnityEngine.Input.GetKeyDown(KeyCode.PageDown))
        {
            currentHealth -= 20;
            //Debug.Log("Damage");
        }
    }

    private void OnToggleDebugMode()
    {
        debugModeActive = !debugModeActive;
    }

    private void HandleCrouching() // this function only gets called when the shouldcrouch is set to true
    {
        if (ShouldCrouch) StartCoroutine(CrouchStand());
    }

    private IEnumerator CrouchStand()
    {
        inCrouchingAnimation = true;

        //declaring local vars
        float timeElapsed = 0f;

        float sizeDifferenceFromLastFrame = 0f;
        float heightAtLastFrame = playerController.height;

        float targetHeight = isCrouching ? standingHeight : crouchingHeight;
        float currentHeight = playerController.height;
        Vector3 targetCenterPoint = isCrouching ? standingCenterPoint : crouchingCenterPoint;
        Vector3 currentCenterPoint = playerController.center;

        while (timeElapsed < timeToCrouch)
        {
            float crouchTime = timeElapsed / timeToCrouch;
            crouchTime = couchAnimCurve.Evaluate(crouchTime);

            playerController.height = Mathf.Lerp(currentHeight, targetHeight, crouchTime);
            playerCapsuleCollider.height = Mathf.Lerp(currentHeight, targetHeight, crouchTime);

            if(isCrouching)
            {
                sizeDifferenceFromLastFrame = playerController.height - heightAtLastFrame;
                sizeDifferenceFromLastFrame = (sizeDifferenceFromLastFrame) + safetyBufferForFunsies;

                playerCollider.transform.position += new Vector3(0f,sizeDifferenceFromLastFrame,0f);
            }

            playerController.center = Vector3.Lerp(currentCenterPoint, targetCenterPoint, crouchTime);
            playerCapsuleCollider.center = Vector3.Lerp(currentCenterPoint, targetCenterPoint, crouchTime);

            timeElapsed += Time.deltaTime;

            heightAtLastFrame = playerController.height;
            yield return null;
        }


        playerController.height = targetHeight;
        playerCapsuleCollider.height = targetHeight;
        playerController.center = targetCenterPoint;
        playerCapsuleCollider.center = targetCenterPoint;

        isCrouching = !isCrouching;

        inCrouchingAnimation = false;
    }
}