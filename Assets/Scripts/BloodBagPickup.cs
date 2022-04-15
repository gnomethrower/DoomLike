using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodBagPickup : MonoBehaviour
{

    public int healthAmount = 25;
    public static bool pickupActive = true;
    float respawnTime = 5f;
    public bool respawnable;

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

        if (respawning && respawnable)
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
        if (other.tag == "Player" && PlayerController.currentHealth < PlayerController.maxHealth)
        {
            mySCollider.enabled = false;
            mySpriteRenderer.enabled = false;

            PlayerController.currentHealth += healthAmount;

            AudioController.audioInstance.PlayBloodBagPickup();
            Debug.Log("Picked up some " + gameObject.name);

            respawning = true;
        }
    }

}
