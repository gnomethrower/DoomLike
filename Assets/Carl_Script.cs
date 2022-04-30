using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Carl_Script : MonoBehaviour
{

    // Carl attacks you when you enter his special personal sphere collider (he's very private like that).
    // Upon entering, you instantly get attacked for n=damage hp. He can only attack after the atkCooldown has passed.

    public int damage = 30;
    public bool canAttack = true;
    public float atkCoolDown;

    public SphereCollider personalSpace;
    public PlayerController_Script playerScript;

    private void Start()
    {

    }

    private void OnTriggerStay(Collider other)
    {
        Attack(other.gameObject);
        //Start Cooldown
    }

    void Attack(GameObject attackee)
    {
        if (attackee.CompareTag("Player") && canAttack)
        {
            playerScript.currentHealth -= damage;
            if (playerScript.currentHealth <= 0) playerScript.Death();
            canAttack = false;
            Invoke("ResetCooldown", atkCoolDown);
        }

    }

    void ResetCooldown()
    {
        canAttack = true;
    }
}
