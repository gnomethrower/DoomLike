using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.Port;
using Random = UnityEngine.Random;
//using static UnityEditorInternal.VersionControl.ListControl;

public class LiveGrenade_Script : MonoBehaviour
{
    [SerializeField] private GameObject explosionParticles;
    GameObject grenadeWeaponObject;
    GameObject grenadeScript;
    UI_Grenade_Script grenadeWeaponScript;
    [SerializeField] private GameObject explosionRadiusDebugSpherePrefab;

    private LayerMask explosionLayers; // Layers to damage in explosion

    //[SerializeField] private enum SplinterCalculationType { ray, parabola };
    //[SerializeField] private SplinterCalculationType selectedCalculationType;
    [SerializeField] private float timer = 0;
    [SerializeField] private int numberOfFragSplinters = 10;
    [SerializeField] private int damagePerFragment;
    [SerializeField] private float explosionDamage;
    [SerializeField] private float explosionRadius;
    [SerializeField] private float explosionForce;
    [SerializeField] private float explosionForceFallOffDistance;
    [SerializeField] private float splinterCastRange = 10f;
    [SerializeField] private float destroyObjectAfterSeconds;
    [SerializeField] private bool castDebugRays = true;
    [SerializeField] private float debugRayCastDurationSecondsGround = .5f;
    [SerializeField] private float debugRayCastDurationSecondsEnemy = 10f;
    [SerializeField] private float debugRayCastDurationSecondsAir = .5f;
    [SerializeField] private bool castDebugSphere = true;
    [SerializeField] private float debugSphereDurationSeconds = 2f;
    [SerializeField] private float debugSphereOpacity = .5f;

    private float fuseTimeSeconds;
    //[SerializeField] private float initialSpeed = 15f; // Initial speed of the fragments
    //[SerializeField] private float fragmentLifetime = 2f; // Lifetime of the fragments before they disappear
    //[SerializeField] private float gravityMultiplier = 1.0f; // Multiplier to adjust the effect of gravity on fragments

    private int state = 0; // 0 = Cooking Timer, 1 = Explosion
    private int currentState;
    private int lastState;
    private int nextState;

    private int layerMaskGround = 1 << 6;
    private int layerMaskEnemy = 1 << 7;

    // This script is only used for the grenade prefab after it is primed. The force will be generated upon instatiating in the grenade weapon script attached to the player.

    private void Start()
    {
        explosionLayers = LayerMask.GetMask("Enemy");

        grenadeWeaponScript = GameObject.Find("UI_Grenade").GetComponent<UI_Grenade_Script>();
        fuseTimeSeconds = grenadeWeaponScript.fuseTimer;
    }

    private void Update()
    {
        if (state == 0)
        {
            CookingState();
        }

        if(state == 1)
        {
            ExplosionState();
        }
    }    

    void CookingState()
    {
        timer += Time.deltaTime;
        //Debug.Log(timer);
        if (timer >= fuseTimeSeconds)
        {
            SetState(state = 1);
        }
    }
    void ExplosionState()
    {
        //Explosion Code in Init State method
        Invoke(nameof(DestroyAfterSeconds), destroyObjectAfterSeconds);
    }
    private void Fragmentation()
    {
        if (this.transform == null)
        {
            Debug.LogError("Cast Transform not assigned!");
            return;
        }

        //Raycast Fragmentation
        if (this.transform == null)
        {
            Debug.LogError("Cast Transform not assigned!");
            return;
        }

        for (int i = 0; i < numberOfFragSplinters; i++)
        {
            float theta = Random.Range(0f, Mathf.PI * 2); // Angle θ in radians (0 to 2π)
            float phi = Random.Range(0f, Mathf.PI);       // Angle φ in radians (0 to π)

            // Convert spherical coordinates to Cartesian coordinates
            float x = Mathf.Sin(phi) * Mathf.Cos(theta);
            float y = Mathf.Sin(phi) * Mathf.Sin(theta);
            float z = Mathf.Cos(phi);

            Vector3 direction = new Vector3(x, y, z);

            RaycastHit hit;
            if (Physics.Raycast(this.transform.position, direction, out hit, splinterCastRange))
            {
                if ((layerMaskGround & (1 << hit.collider.gameObject.layer)) != 0)
                {
                    if (castDebugRays) Debug.DrawRay(this.transform.position, direction * hit.distance, Color.blue, debugRayCastDurationSecondsGround);
                    // Additional code if ground layer is hit
                    //Debug.Log("Hit Ground!");
                }
                else if ((layerMaskEnemy & (1 << hit.collider.gameObject.layer)) != 0)
                {
                    if(castDebugRays) Debug.DrawRay(this.transform.position, direction * hit.distance, Color.red, debugRayCastDurationSecondsEnemy);
                    Mortality_Script mortalObj = hit.transform.GetComponent<Mortality_Script>(); // we create a new variable "mortalObj" of the class Mortal, which we define as what the raycasthit "hit" has found.
                    mortalObj.TakeDamage(damagePerFragment);
                    // Additional code if enemy layer is hit
                    //Debug.Log("Hit Enemy!");
                }
            }
            else
            {
                if (castDebugRays) Debug.DrawRay(this.transform.position, direction * splinterCastRange, Color.green, debugRayCastDurationSecondsAir);
            }
        }
               
    }

    void RadialExplosion(Vector3 center, float radius)
    {
        Collider[] colliders = Physics.OverlapSphere(center, radius, explosionLayers);

        if (explosionRadiusDebugSpherePrefab != null && castDebugSphere)
        {
            GameObject debugSphere = Instantiate(explosionRadiusDebugSpherePrefab, center, Quaternion.identity);

            // Set the scale of the debug sphere to match the explosion radius
            debugSphere.transform.localScale = new Vector3(radius * 2f, radius * 2f, radius * 2f);

            // Set the color and opacity of the debug sphere
            Renderer renderer = debugSphere.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material.color = new Color(1f, 0f, 0f, debugSphereOpacity);
            }

            // Destroy the debug sphere after the specified duration
            Destroy(debugSphere, debugSphereDurationSeconds);
        }

        ApplyExplosionForce(center, radius);

        foreach (Collider col in colliders)
        {
            if (GameplayUtilities.IsInSightlineOf(this.transform.position, col.transform.position, explosionRadius, 6))
            {
                Debug.Log("Object hit: " + col.gameObject.name);
                Mortality_Script mortalObj = col.GetComponent<Mortality_Script>();
                mortalObj.TakeDamage(explosionDamage);
            }
        }
    }

    private void ApplyExplosionForce(Vector3 center, float radius)
    {
        Collider[] colliders = Physics.OverlapSphere(center, radius);

        foreach (Collider col in colliders)
        {
            Rigidbody rb = col.GetComponent<Rigidbody>();
            if (rb != null)
            {
                Vector3 direction = col.transform.position - center;

                // Apply the force to the object without any falloff
                rb.AddForce(direction.normalized * explosionForce, ForceMode.Impulse);
            }
        }
    }

    void DestroyAfterSeconds()
    {
        //Debug.Log("Destroy LiveGrenade");
        Destroy(gameObject);
    }

    #region States
    void SetState(int nextState)
    {
        if (nextState == currentState)
        {
            return;
        }

        ExitState(currentState);
        lastState = currentState;
        currentState = nextState;

        InitializingNextState(currentState);
    }
    void InitializingNextState(int initState)
    {
        switch (initState)
        {
            case 0:
                //Debug.Log("Init Cooking Timer State");
                break;
            case 1:
                Fragmentation();
                RadialExplosion(this.transform.position, explosionRadius);
                Instantiate(explosionParticles, this.transform.position, new Quaternion(0f, 0f, 0f, 0f));
                break;
        }
    }
    void ExitState(int lastState)
    {
        switch (lastState)
        {
            case 0:
                break;

            case 1:
                timer = 0;
                break;
        }
    }
    #endregion

}

