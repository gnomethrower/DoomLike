using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodBagPickup_Script : MonoBehaviour
{

    public int healthAmount = 25;
    public static bool pickupActive = true;
    public float respawnTime;
    public bool respawnable;

    private SpriteRenderer mySpriteRenderer;
    private SphereCollider mySCollider;
    public AudioController_Script audioInstance;
    public PlayerController_Script playerScript;
    public GameObject player;

    private void Start()
    {
        mySpriteRenderer = GetComponent<SpriteRenderer>();
        mySCollider = GetComponent<SphereCollider>();

        GameObject AudioController = GameObject.FindGameObjectWithTag("AudioController");
        audioInstance = AudioController.GetComponent<AudioController_Script>();

        player = GameObject.FindGameObjectWithTag("Player");
        playerScript = player.GetComponent<PlayerController_Script>();
    }


    void Reactivate()
    {
        //fancy particles
        mySpriteRenderer.enabled = true;
        mySCollider.enabled = true;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player" && playerScript.currentHealth < playerScript.maxHealth)
        {
            mySCollider.enabled = false;
            mySpriteRenderer.enabled = false;

            playerScript.currentHealth = Mathf.Min(playerScript.currentHealth + healthAmount, playerScript.maxHealth);

            audioInstance.PlayBloodBagPickup();

            if (respawnable) Invoke("Reactivate", respawnTime);
        }
    }

}

