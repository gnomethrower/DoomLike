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

    private void Start()
    {
        mySpriteRenderer = GetComponent<SpriteRenderer>();
        mySCollider = GetComponent<SphereCollider>();
    }


    void Reactivate()
    {
        //fancy particles
        mySpriteRenderer.enabled = true;
        mySCollider.enabled = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" && PlayerController_Script.currentHealth < PlayerController_Script.maxHealth)
        {
            mySCollider.enabled = false;
            mySpriteRenderer.enabled = false;

            PlayerController_Script.currentHealth += healthAmount;

            AudioController_Script.audioInstance.PlayBloodBagPickup();
            Debug.Log("Picked up some " + gameObject.name);

            if (respawnable) Invoke("Reactivate", respawnTime);
        }
    }

}
