using BehaviorDesigner.Runtime.Tasks.Unity.UnityInput;
using BehaviorDesigner.Runtime.Tasks.Unity.UnityQuaternion;
using System;
using UnityEngine;
using UnityEngine.Windows;
public class PlayerController_Script : MonoBehaviour
{
    #region Change in Inspector
    [Header("Health Settings")]
    public int maxHealth = 100;
    public int currentHealth = 50;
    public int criticalHealthThreshold = 25;
    private int healthLastFrame = 100;
    [SerializeField] bool healthCritical = false;

    [Header("Weapons, Items and Inventory")]
    //Equipped guns
    public StartingGun startingGun;
    public enum StartingGun { Pistol, Shotgun }
    
    //Flashlight
    [SerializeField] private int flashlightIntensity;
    [Tooltip("a value between 40000 and 80000 works well.")]
    [SerializeField] private int flashLightMaxIntensity;
    private int flashLightMode = 0;
    /* Got Gun Bools
    //[Header("Got Gun Bools")]
    //public bool gotShotgun;
    //public bool gotPistol;
    //public bool gotGrenade;
    */

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
    [SerializeField] private float maxLeanAngleDegrees;

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
    #endregion

    private float exhaustedJumpHeightMultiplier = 0.5f;
    public static float xAxis;
    public static float zAxis;
    public static float spreadMultiplier;


    [Header("Checkbools")]
    public bool playerHasDied;
    private bool debugModeActive = false;

    [SerializeField] public bool isGrounded;
    [SerializeField] public bool isMoving;
    [SerializeField] public bool isExhausted;


    //Movement
    private float currentLocalEulerAngleZ;
    private float targetLocalEulerAngleZ;
    private int leanDirection; // 0: none, 1: left, 2: right

    #region References
    private GameObject leaningPivotPoint;
    public GameObject deathScreen;
    public GameObject uiCanvas;
    private GameObject flashLightObject;
    private Light flashLightSpotlight;
    public CharacterController controller;
    public Animator viewCamAnim;
    public Transform groundCheck;
    public float groundCheckSize = 0.5f;
    public LayerMask groundMask;
    private Collider playerCollider;


    //Scripts
    GameHandler gameEventManagerScript;
    public AudioController_Script audioInstance;

    public Vector3 playerVelocity;
    private Vector3 lastKnownPosition;
    private Vector3 currentPosition;
    #endregion

    #region Events
    public static Action Action_PlayerDeath;
    public static Action Action_PlayerStaminaExhausted;
    public static Action Action_PlayerStaminaRecovered;
    public static Action Action_PlayerHealthCritical;
    public static Action Action_PlayerHealthRecoveredFromCritical;
    #endregion

    private void Awake()
    {
        flashLightObject = GameObject.Find("FlashLight");
        flashLightSpotlight = flashLightObject.GetComponent<Light>();
        if (flashLightSpotlight == null) { Debug.Log("noFlashlight!"); }
        deathScreen = GameObject.Find("DeathScreen");
        playerCollider = this.GetComponent<Collider>();
        leaningPivotPoint = GameObject.Find("LeaningPivotPoint");
        if (leaningPivotPoint == null) { Debug.Log("We have no leaningPivotPoint"); }

        gameEventManagerScript = GameObject.Find("GameHandler").GetComponent<GameHandler>();

        currentStamina = maxStamina;
        lastKnownPosition = transform.position;
        currentPosition = transform.position;

        Action_PlayerStaminaExhausted += OnPlayerStaminaExhasted;
        Action_PlayerStaminaRecovered += OnPlayerStaminaRecovered;
        Action_PlayerDeath += OnPlayerDeath;
        Action_PlayerHealthCritical += OnPlayerHealthCritical;
        Action_PlayerHealthRecoveredFromCritical += OnPlayerHealthRecoveredFromCritical;

        EventManagerMaster.Action_ToggleDebugMode += OnToggleDebugMode;
    }

    private void Start()
    {

    }

    void Update()
    {
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
                if (UnityEngine.Input.GetButton("Sprint") && isMoving && currentStamina > 0 && !isExhausted)
                {
                    if(!staminaTimerRunning) StartStaminaTimer(staminaRecTimeFresh);
                    currentStamina -= (Time.deltaTime * staminaDepletionMultiplier);
                    activeSprintMultiplier = sprintMultiplierValue;
                } else
                {
                    activeSprintMultiplier = 1f;
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

        if (healthLastFrame == currentHealth) // serves as a gate to the rest of the function - we don't need to check the rest if the health hasn't changed.
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

    void CalculateSpreadMP()
    {
        spreadMultiplier = Mathf.Clamp(((Mathf.Abs(zAxis)) + (Mathf.Abs(xAxis))), 0, 1) + jumpingSpread;
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
            if (!isExhausted) playerVelocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            if (isExhausted) playerVelocity.y = Mathf.Sqrt((jumpHeight * exhaustedJumpHeightMultiplier) * -2f * gravity);
        }

        //normalizing vector movement, so diagonal movement isn't twice as fast.
        Vector3 move = transform.right * xAxis + transform.forward * zAxis;
        if (move.magnitude > 1)
        {
            move /= move.magnitude;
        }

        // playerSpeed is always multiplied by sprintmultiplier
        playerSpeed = currentWalkSpeed * activeSprintMultiplier;

        //setting up the movement with the playerspeed and correcting for executiontime
        controller.Move((move * playerSpeed) * Time.deltaTime);

        controller.Move(playerVelocity * Time.deltaTime);
    }

    void DebugFunctions()
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
}