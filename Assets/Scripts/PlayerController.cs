using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{


    //+++++++++++++++++++++++     Changing variables throughout play    +++++++++++++++++++++++++++++

    public static int maxHealth = 100;
    public static int currentHealth = 50;

    //+++++++++++++++++++++++     Unchangables    +++++++++++++++++++++++++++++
    public float playerSpeed = 12f;
    public float gravity = -12f;
    public float jumpHeight = .0001f;

    private float xAxis;
    private float zAxis;

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


    void Update()
    {
        if (currentHealth > maxHealth) currentHealth = maxHealth;

        if (!hasDied)
        {
            GetInput();
            FPSMovement();
        }
    }

    void GetInput()
    {
        xAxis = Input.GetAxis("Horizontal");
        zAxis = Input.GetAxis("Vertical");
    }



    void FPSMovement()
    {
        // jumping with groundcheck
        isGrounded = Physics.CheckBox(groundCheck.position, new Vector3(groundCheckSize, groundCheckSize, groundCheckSize), groundCheck.transform.rotation, groundMask);
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
}

