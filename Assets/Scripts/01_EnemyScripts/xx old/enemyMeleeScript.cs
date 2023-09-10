using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemyMeleeScript : MonoBehaviour
{

    // Carl attacks you when you enter his special personal sphere collider (he's very private like that).
    // Upon entering, you instantly get attacked for n=damage hp. He can only attack after the atkCooldown has passed.

    public int damage = 30;
    public bool canAttack = true;
    public float atkCoolDown;

    public SphereCollider personalSpace;
    public PlayerController_Script playerScript;
    public ShakeRecoil_Script shakingScript;
    public AudioController_Script audioInstance;
    private void Start()
    {
        audioInstance = GameObject.FindGameObjectWithTag("AudioController").GetComponent<AudioController_Script>();
    }

    private void OnTriggerStay(Collider other)
    {
        Attack(other.gameObject);
        //Start Cooldown
    }

    void Attack(GameObject attackee)
    {
        if (attackee.CompareTag("Player") && canAttack && !playerScript.hasDied)
        {
            playerScript.currentHealth -= damage;
            StartCoroutine(shakingScript.Shaking(.25f, 3f));

            audioInstance.PlayEnemyMelee();
            if (playerScript.currentHealth <= 0) playerScript.CheckForDeath();
            if (playerScript.hasDied) playerScript.currentHealth = 0;

            canAttack = false;
            Invoke("ResetCooldown", atkCoolDown);
        }

    }

    void ResetCooldown()
    {
        canAttack = true;
    }
}
