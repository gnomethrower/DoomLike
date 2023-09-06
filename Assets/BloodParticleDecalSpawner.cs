using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.ProBuilder;

public class BloodParticleDecalSpawner : MonoBehaviour
{
    public ParticleSystem part;
    public List<ParticleCollisionEvent> collisionEvents;
    public GameObject[] bloodSplatter;

    void Start()
    {
        part = GetComponent<ParticleSystem>();
        collisionEvents = new List<ParticleCollisionEvent>();
    }



    void OnParticleCollision(GameObject other)
    {
        //Debug.Log("Collision happened");
        int numCollisionEvents = part.GetCollisionEvents(other, collisionEvents);

        Collider collider = other.GetComponent<Collider>();
        int i = 0;

        while (i < numCollisionEvents)
        {
            if (collider)
            {
                Vector3 pos = collisionEvents[i].intersection;

                int p = (Random.Range(1, 100));
                if (p >= 80)
                {
                    Instantiate(bloodSplatter[(Random.Range(0, 1))], pos, Quaternion.LookRotation(collisionEvents[i].normal));
                }

                //DebugIndicator.DrawDebugIndicator(pos, Color.red);
            }
            i++;
        }
    }
}