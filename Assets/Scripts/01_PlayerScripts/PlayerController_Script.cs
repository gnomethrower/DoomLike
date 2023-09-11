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
    public float baseSpeed = 12f;
    public float sprintMultiplier = 1.25f;
    public float gravity = -12f;
    private float stamina = 100f;
    [SerializeField] private float staminaDepletionMultiplier;
    [SerializeField] private float staminaRefillMultiplier;
    [SerializeField] private float staminaExhaustionThreshold;
    private int sprintState = 0;

    private float playerSpeed;
    private float sMp = 1f;

    public float jumpHeight = .0001f;
    public static float xAxis;
    public static float zAxis;

    public static float spreadMultiplier;
    float jumpingSpread;

    Collider playerCollider;

    [Header("Checkbools")]
    [SerializeField] private bool isGrounded;
    [SerializeField] public bool hasDied;
    [SerializeField] private bool isMoving;

    Quaternion currentLeaningRotation = Quaternion.identity;
    Quaternion targetRotation;
    [SerializeField] private float leanRotationTarget = 15f;

    //[SerializeField] private bool isSprinting;

    #region Variables

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
    #endregion

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
        lastKnownPosition = transform.position;
        currentPosition = transform.position;
    }

    void Update()
    {
        DebugMsg();
        CheckForMovement();
        if (!hasDied)
        {
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

    void Sprinting()
    {
        Debug.Log(stamina);
        Debug.Log(isMoving);

        if (stamina > 100f)
        {
            Debug.Log("OverflowCorrection, stamina > 100");
            stamina = 100f;
        }

        if (stamina < 0)
        {
            Debug.Log("OverflowCorrection > 0");
            stamina = 0;
        }

        switch (sprintState) //0: fresh;   1: exhausted;   2: reset to fresh
        {
            case 0:
                if (UnityEngine.Input.GetButton("Sprint") && isMoving && stamina > 0)
                {
                    stamina -= (Time.deltaTime * staminaDepletionMultiplier);
                    sMp = sprintMultiplier;

                    if (stamina <= 0) // When stamina is entirely depleted, exhaustion sets in. The player moves more slowly.
                    {
                        //Setting Exhausted Parameters
                        Debug.Log("EXHAUSTED!");
                        playerSpeed = baseSpeed / 1.5f;
                        sprintState = 1;
                    }
                }
                else if (stamina < 100f) // Stamina refills at normal rate.
                {
                    stamina += (Time.deltaTime * staminaRefillMultiplier);
                }

                sMp = 1f;
                break;

            case 1:

                if (stamina < staminaExhaustionThreshold) //When exhausted, stamina refills more slowly
                {
                    stamina += (Time.deltaTime * staminaRefillMultiplier / 1.5f);
                }

                //Resetting, if Stamina refilled to threshold.
                if (stamina >= staminaExhaustionThreshold)
                {
                    sprintState = 2;
                }
                break;

            case 2:
                playerSpeed = baseSpeed;
                break;
        }

    }

    void CalculateSpreadMP()
    {
        spreadMultiplier = Mathf.Clamp(((Mathf.Abs(zAxis)) + (Mathf.Abs(xAxis))), 0, 1) + jumpingSpread;
    }

    void MoveAndJump()
    {
        // jumping with groundcheck
        isGrounded = Physics.CheckBox(groundCheck.position, new Vector3(groundCheckSize, groundCheckSize, groundCheckSize), groundCheck.transform.rotation, groundMask);
        if (isGrounded) jumpingSpread = 1f;
        else jumpingSpread = 3f;

        if (isGrounded && playerVelocity.y < 0)
        {
            playerVelocity.y = -2f;
        }

        if (UnityEngine.Input.GetButtonDown("Jump") && isGrounded)
        {
            playerVelocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }


        //normalizing vector movement, so diagonal movement isn't twice as fast.
        Vector3 move = transform.right * xAxis + transform.forward * zAxis;
        if (move.magnitude > 1)
        {
            move /= move.magnitude;
        }

        // playerSpeed is always multiplied by sprintmultiplier
        playerSpeed = baseSpeed * sMp;

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

    private void OnDestroy()
    {

    }

}


