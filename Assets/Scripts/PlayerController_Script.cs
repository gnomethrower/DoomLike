/**using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    public float walkingSpeed = 12f;
    public float sprintMultiplierValue = 1.25f;
    public float gravity = -12f;


    float playerSpeed;
    float activeSprintMultiplier = 1f;

    public float jumpHeight = .0001f;
    public static float xAxis;
    public static float zAxis;

    public static float spreadMultiplier;
    float jumpingSpread;

    [Header("Object References")]
    public CharacterController controller;
    public Vector3 playerVelocity;
    public Transform groundCheck;
    public float groundCheckSize = 0.5f;
    public LayerMask groundMask;
    public Animator viewCamAnim;
    public GameObject deathScreenPrefab;
    public AudioController_Script audioInstance;
    public GameObject uiCanvas;

    [Header("Checkbools")]
    [SerializeField] private bool isGrounded;
    [SerializeField] public bool playerHasDied;
    [SerializeField] private bool isMoving;
    //[SerializeField] private bool isSprinting;

    private void Start()
    {

    }

    void Update()
    {

        DebugMsg();

        if (!playerHasDied)
        {
            GetInput();
            CalculateSpreadMP();
            MoveAndJump();
        }
    }

    void DebugMsg()
    {
        //Debug.Log("Sprint MP is " + sprintMultiplierValue);
        //Debug.Log("Speed is " + playerSpeed);
        //if(isSprinting) Debug.Log("Player is Sprinting: " + isSprinting);
    }

    void GetInput()
    {
        xAxis = Input.GetAxis("Horizontal");
        zAxis = Input.GetAxis("Vertical");
        Sprinting();
    }

    void Sprinting()
    {
        if (Input.GetButton("Sprint"))
        {
            //isSprinting = true;
            activeSprintMultiplier = sprintMultiplierValue;
        }
        else
        {
            //isSprinting = false;
            activeSprintMultiplier = 1f;
        }
    }

    
    void CalculateSpreadMP()
    {
        //Used to calculate the weapon spread based.
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
        playerSpeed = walkingSpeed * activeSprintMultiplier;

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
        if (!playerHasDied)
        {
            playerHasDied = true;
            Debug.Log("YOU DIED! GIT GUD!");
            audioInstance.PlayPlayerDeath();
            Destroy(uiCanvas);
            //Call deathscreen
            //call death anim
            //call text mocking player
        }
    }
}


**/
