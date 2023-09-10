using BehaviorDesigner.Runtime.ObjectDrawers;
using System.Collections;
using System.Collections.Generic;
using System;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UIElements;
using BehaviorDesigner.Runtime.Tasks.Unity.UnityParticleSystem;

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


    float playerSpeed;
    float sMp = 1f;

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

        #region set Obj references
        gameEventManagerScript = GameObject.Find("GameEventManager").GetComponent<GameEventManager>();
        #endregion

        playerCollider = this.GetComponent<Collider>();
    }

    private void Start()
    {

    }

    void Update()
    {
        CheckForDeath();
        DebugMsg();

        GetInput();
        if (!hasDied)
        {
            Flashlight();
            CalculateSpreadMP();
            FPSMovement();
        }
    }

    void DebugMsg()
    {
        //Debug.Log("Sprint MP is " + sprintMultiplier);
        //Debug.Log("Speed is " + playerSpeed);
        //if(isSprinting) Debug.Log("Player is Sprinting: " + isSprinting);
    }

    void GetInput()
    {
        xAxis = Input.GetAxis("Horizontal");
        zAxis = Input.GetAxis("Vertical");
        Sprinting();
    }

    void Flashlight()
    {
        if (Input.GetKeyDown(KeyCode.F))
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
        if (Input.GetButton("Sprint"))
        {
            //isSprinting = true;
            sMp = sprintMultiplier;
        }
        else
        {
            //isSprinting = false;
            sMp = 1f;
        }
    }

    void CalculateSpreadMP()
    {
        spreadMultiplier = Mathf.Clamp(((Mathf.Abs(zAxis)) + (Mathf.Abs(xAxis))), 0, 1) + jumpingSpread;
    }

    void FPSMovement()
    {
        // jumping with groundcheck
        isGrounded = Physics.CheckBox(groundCheck.position, new Vector3(groundCheckSize, groundCheckSize, groundCheckSize), groundCheck.transform.rotation, groundMask);
        if (isGrounded) jumpingSpread = 1f;
        else jumpingSpread = 3f;

        if (isGrounded && playerVelocity.y < 0)
        {
            playerVelocity.y = -2f;
        }

        if (Input.GetButtonDown("Jump") && isGrounded)
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


        //gravity
        playerVelocity.y += gravity * Time.deltaTime;


        //moving
        controller.Move(playerVelocity * Time.deltaTime);
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

    private void OnDestroy()
    {

    }
}


