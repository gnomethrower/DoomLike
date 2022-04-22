using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionAudioPlay_Script : MonoBehaviour
{
    public AudioClip ShellCasingFall;
    public ParticleSystem shotShell;


    public List<ParticleCollisionEvent> collisionEvents;

    void Start()
    {
        shotShell = GetComponent<ParticleSystem>();
        collisionEvents = new List<ParticleCollisionEvent>();
    }

    void OnParticleCollision(GameObject other)
    {
        int numCollisionEvents = shotShell.GetCollisionEvents(other, collisionEvents);

        MeshCollider mc = other.GetComponent<MeshCollider>();
        int i = 0;


        if (mc)
        {
            Debug.Log("Shell fell on " + mc.name);
        }

    }
}




