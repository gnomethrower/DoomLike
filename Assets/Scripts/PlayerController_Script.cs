using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController_Script : MonoBehaviour
{


    //+++++++++++++++++++++++     Changing variables throughout play    +++++++++++++++++++++++++++++
    [Header("Health Settings")]
    public static int maxHealth = 100;
    public static int currentHealth = 50;

    //+++++++++++++++++++++++     Weapon Inventory    +++++++++++++++++++++++++++++

    public enum StartingGun { Pistol, Shotgun }
    public StartingGun startingGun;

    //[Header("Got Gun Bools")]
    //public bool gotShotgun;
    //public bool gotPistol;
    //public bool gotGrenade;

    //+++++++++++++++++++++++     Weapon Ammo    +++++++++++++++++++++++++++++
    [Header("Gun Ammo Settings")]
    public int shotgunSpareAmmo;
    public int shotgunMaxAmmo;

    public int pistolSpareAmmo;
    public int pistolMaxAmmo;

    public int grenadesSpare;
    public int grenadesMax;

    //+++++++++++++++++++++++     Unchangables    +++++++++++++++++++++++++++++
    public float playerSpeed = 12f;
    public float gravity = -12f;
    public float jumpHeight = .0001f;

    public static float xAxis;
    public static float zAxis;

    public static float spreadMultiplier;
    float jumpingSpread;

    //+++++++++++++++++++++++     Obj References    +++++++++++++++++++++++++++++

    public CharacterController controller;
    Vector3 playerVelocity;
    public Transform groundCheck;
    public float groundCheckSize = 0.5f;
    public LayerMask groundMask;

    public Animator viewCamAnim;
    public GameObject deathScreen;

    //+++++++++++++++++++++++     Checkbools    +++++++++++++++++++++++++++++

    public bool isGrounded;
    public bool hasDied;



    private void Start()
    {

    }

    void Update()
    {
        //Debug.Log("SpreadMultiplier is " + spreadMultiplier);

        if (currentHealth > maxHealth) currentHealth = maxHealth;

        if (!hasDied)
        {
            GetInput();
            CalculateSpreadMP();
            FPSMovement();
        }
    }

    void GetInput()
    {
        xAxis = Input.GetAxis("Horizontal");
        zAxis = Input.GetAxis("Vertical");
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


        //setting up the movement with the playerspeed and correcting for executiontime
        controller.Move(move * playerSpeed * Time.deltaTime);


        //gravity
        playerVelocity.y += gravity * Time.deltaTime;


        //moving
        controller.Move(playerVelocity * Time.deltaTime);

    }

    public void TakeDamage(int damageAmount)
    {
        currentHealth -= damageAmount;
        if (currentHealth <= 0)
        {
            hasDied = true;
            deathScreen.SetActive(true);
        }
    }

    public void GetHealth(int healAmount)
    {
        currentHealth += healAmount;
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
    }

    public void ShotgunAmmoCheck()
    {
        if (shotgunSpareAmmo > shotgunMaxAmmo)
        {
            Debug.Log("Shotgunammo has been balanced");

            shotgunSpareAmmo = shotgunMaxAmmo;
        }
    }
    public void PistolAmmoCheck()
    {
        if (pistolSpareAmmo > pistolMaxAmmo)
        {
            Debug.Log("PistolAmmo has been balanced");

            pistolSpareAmmo = pistolMaxAmmo;
        }
    }
}


