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

    public bool hasDeathPrefab;
    public GameObject deathPrefab;

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
        //Instantiating the deathPrefab, if the object has such a thing.
        if (hasDeathPrefab)
        {
            Instantiate(deathPrefab, transform.position, transform.rotation);
        }
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
