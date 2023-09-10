using EightDirectionalSpriteSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mortality_Script : MonoBehaviour
{


    public float maxHealth = 100f;
    [HideInInspector] public float currentHealth;
    public float painDuration;

    public bool canBleed;

    private float deathAnimDuration;

    [SerializeField] private bool hasDeathAnimation;
    [SerializeField] private GameObject deathPrefab;
    private BasicMantisClass basicMantisClass;
    private SimplestMantisActor simplestMantisActor;

    [Tooltip("If this entity is a simple TargetPractice Object, set this to true")]
    public bool respawningTarget = false;
    public float targetRespawntime;

    MeshRenderer targetRenderer;
    Collider targetCollider;

    [HideInInspector] public bool gotHurt = false;

    private void Awake()
    {
        if (hasDeathAnimation)
        {
            basicMantisClass = this.GetComponent<BasicMantisClass>();
            simplestMantisActor = this.GetComponent<SimplestMantisActor>();
            deathAnimDuration = simplestMantisActor.dieAnim.FrameCount * simplestMantisActor.frameDuration;
        }
    }

    private void Start()
    {
        GetMaxHealth();
        targetRenderer = this.GetComponent<MeshRenderer>();
        targetCollider = this.GetComponent<Collider>();
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
            else
            {
                StartCoroutine(Death(deathAnimDuration));
            }
        }
    }

    private IEnumerator Death(float delayInSeconds)
    {
        /* 1. Play deathanim
         * 2. wait for deathanimation secondsUntilRestartPrompt
         * 3. Instantiate DeathPrefabs
         * 4. Destroy object.
        */

        if (hasDeathAnimation)
        {
            basicMantisClass.CallingAnimation(SimplestMantisActor.State.DIE);
            yield return new WaitForSeconds(delayInSeconds);
        }


        if (deathPrefab != null)
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
