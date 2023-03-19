using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotgunAmmoPickup_Script : MonoBehaviour
{

    public int shellAmount = 5;
    public static bool pickupActive = true;
    public float respawnTime = 5f;

    private SpriteRenderer mySpriteRenderer;
    private SphereCollider mySCollider;
    public bool respawning;

    GameObject player;
    public PlayerController_Script playerScript;
    public AudioController_Script audioInstance;

    private void Start()
    {
        mySpriteRenderer = GetComponent<SpriteRenderer>();
        mySCollider = GetComponent<SphereCollider>();

        player = GameObject.FindGameObjectWithTag("Player");
        playerScript = player.GetComponent<PlayerController_Script>();

        GameObject AudioController = GameObject.FindGameObjectWithTag("AudioController");
        audioInstance = AudioController.GetComponent<AudioController_Script>();
    }


    void Reactivate()
    {
        //fancy particles
        mySpriteRenderer.enabled = true;
        mySCollider.enabled = true;
    }

    public void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player" && playerScript.shotgunMaxAmmo > playerScript.shotgunSpareAmmo)
        {
            playerScript.shotgunSpareAmmo = Mathf.Min(playerScript.shotgunSpareAmmo + shellAmount, playerScript.shotgunMaxAmmo);

            mySCollider.enabled = false;
            mySpriteRenderer.enabled = false;

            audioInstance.PlayAmmoPickup();
            //Debug.Log("Picked up some " + gameObject.name);

            if (respawning) Invoke("Reactivate", respawnTime);
        }
    }



}


