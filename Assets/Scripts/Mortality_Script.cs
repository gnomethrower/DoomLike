using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mortality_Script : MonoBehaviour
{
    public bool respawningTarget = true;
    public float targetRespawntime;

    public float maxHealth = 100f;
    public float health;

    MeshRenderer targetRenderer;
    SphereCollider targetCollider;

    public bool hasBeenAttacked = false;

    private void Start()
    {
        GetMaxHealth();
        targetRenderer = GetComponent<MeshRenderer>();
        targetCollider = GetComponent<SphereCollider>();
    }

    void GetMaxHealth()
    {
        health = maxHealth;
    }

    public void TakeDamage(float amount)
    {
        hasBeenAttacked = true;
        health -= amount;
        if (health <= 0f)
        {
            if (respawningTarget) SetInactive();
            else Die();
        }
    }

    void Die()
    {
        //Die gets called, this happens:
        Destroy(gameObject);

    }
    void Reactivate()
    {
        //RespawnParticleEffect
        GetMaxHealth();
        targetRenderer.enabled = true;
        targetCollider.enabled = true;
    }

    void SetInactive()
    {
        targetRenderer.enabled = false;
        targetCollider.enabled = false;

        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        if (respawningTarget == true) Invoke("Reactivate", targetRespawntime);
    }
}
