using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mortality_Script : MonoBehaviour
{
    public bool respawningTarget = true;
    public float targetRespawntime;

    public float maxHealth = 100f;
    public float currentHealth;
    public float painDuration;
    public bool canBleed;

    MeshRenderer targetRenderer;
    Collider targetCollider;

    public bool gotHurt = false;
    public bool hasDeathPrefab;
    public GameObject deathPrefab;

    private void Start()
    {
        GetMaxHealth();
        targetRenderer = GetComponent<MeshRenderer>();
        targetCollider = GetComponent<Collider>();
        if (targetCollider == null)
        {
            targetCollider = GetComponentInChildren<Collider>();
        }
        if (targetCollider == null)
        {
            Debug.Log("Collider is null");
        }
    }

    void GetMaxHealth()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        gotHurt = true;

        if (currentHealth <= 0f)
        {
            if (respawningTarget) SetInactive();
            else Die();
        }
    }

    void Die()
    {
        if (hasDeathPrefab)
        {
            Instantiate(deathPrefab, transform.position, transform.rotation);
        }

        Destroy(transform.root.gameObject);
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
