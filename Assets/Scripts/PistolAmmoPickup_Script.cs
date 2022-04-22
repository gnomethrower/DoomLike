using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PistolAmmoPickup_Script : MonoBehaviour

{

    public int shellAmount = 5;
    public static bool pickupActive = true;
    public float respawnTime = 5f;

    private SpriteRenderer myRenderer;
    private SphereCollider myCollider;
    public bool respawning;

    GameObject player;
    public PlayerController_Script playerScript;
    public AudioController_Script audioInstance;


    private void Start()
    {
        myRenderer = GetComponent<SpriteRenderer>();
        myCollider = GetComponent<SphereCollider>();

        player = GameObject.FindGameObjectWithTag("Player");
        playerScript = player.GetComponent<PlayerController_Script>();

        GameObject AudioController = GameObject.FindGameObjectWithTag("AudioController");
        audioInstance = AudioController.GetComponent<AudioController_Script>();
    }


    void Reactivate()
    {
        //fancy particles
        myRenderer.enabled = true;
        myCollider.enabled = true;
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" && playerScript.pistolMaxAmmo > playerScript.pistolSpareAmmo)
        {
            playerScript.pistolSpareAmmo += shellAmount;
            myCollider.enabled = false;
            myRenderer.enabled = false;

            audioInstance.PlayAmmoPickup();
            Debug.Log("Picked up some " + gameObject.name);

            if (respawning) Invoke("Reactivate", respawnTime);
        }

        playerScript.ShotgunAmmoCheck();
    }



}


