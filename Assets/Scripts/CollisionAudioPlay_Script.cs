using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionAudioPlay_Script : MonoBehaviour
{
    public AudioClip ShellCasingFall;

    ////int numCollisionEvents = part.GetCollisionEvents(other, collisionEvents);

    //And iterate over the collisions for getting the intersection

    // Vector3 pos = collisionEvents[i].intersection;
    //int numCollisionEvents = part.GetCollisionEvents(other, collisionEvents);

    //And iterate over the collisions for getting the intersection

    // Vector3 pos = collisionEvents[i].intersection;

    private void OnParticleCollision(GameObject other)
    {
        AudioSource.PlayClipAtPoint(ShellCasingFall, other.transform.position);
    }

}



