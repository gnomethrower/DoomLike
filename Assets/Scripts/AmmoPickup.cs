using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoPickup : MonoBehaviour
{

    public int shellAmount = 5;
    public static bool pickupActive = true;
    float respawnTime = 5f;

    private SpriteRenderer mySpriteRenderer;
    private SphereCollider mySCollider;
    public bool respawning;

    GameObject shotgun;

    private void Start()
    {
        mySpriteRenderer = GetComponent<SpriteRenderer>();
        mySCollider = GetComponent<SphereCollider>();
    }

    void Update()
    {
    }

    void Reactivate()
    {
        //fancy particles
        mySpriteRenderer.enabled = true;
        mySCollider.enabled = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        shotgun = WeaponSwitching.shotgunInit;

        if (other.tag == "Player" && shotgun.GetComponent<sg_Script>().ammoSpare < shotgun.GetComponent<sg_Script>().maxSpareAmmo)
        {
            mySCollider.enabled = false;
            mySpriteRenderer.enabled = false;

            shotgun.GetComponent<sg_Script>().ammoSpare += shellAmount;

            AudioController.audioInstance.PlayAmmoPickup();
            Debug.Log("Picked up some " + gameObject.name);

            if (respawning) Invoke("Reactivate", respawnTime);


        }
    }
}


