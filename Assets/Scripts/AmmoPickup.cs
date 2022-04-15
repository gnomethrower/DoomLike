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
    bool respawning = false;
    private void Start()
    {
        mySpriteRenderer = GetComponent<SpriteRenderer>();
        mySCollider = GetComponent<SphereCollider>();
    }

    void Update()
    {

        if (respawning)
        {
            respawning = false;
            Invoke("Reactivate", respawnTime);
        }

    }

    void Reactivate()
    {
        //fancy particles
        mySpriteRenderer.enabled = true;
        mySCollider.enabled = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" && sg_Script.spareBullets < sg_Script.maxSpareAmmo)
        {
            mySCollider.enabled = false;
            mySpriteRenderer.enabled = false;

            sg_Script.spareBullets += shellAmount;

            AudioController.audioInstance.PlayAmmoPickup();
            Debug.Log("Picked up some " + gameObject.name);

            respawning = true;
        }
    }

}
