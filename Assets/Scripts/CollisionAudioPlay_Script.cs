using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionAudioPlay_Script : MonoBehaviour
{
    public AudioController_Script audioInstance;
    public ParticleSystem shotShell;

    public List<ParticleCollisionEvent> collisionEvents;

    void Start()
    {
        shotShell = GetComponent<ParticleSystem>();
        collisionEvents = new List<ParticleCollisionEvent>();

        GameObject AudioController = GameObject.FindGameObjectWithTag("AudioController");
        audioInstance = AudioController.GetComponent<AudioController_Script>();
    }

    void OnParticleCollision(GameObject other)
    {
        if (gameObject.name == "Particle_9mmCasing") audioInstance.PlayCasingCollision();
        if (gameObject.name == "Particle_ShotgunShell") audioInstance.PlayShellCollision();
    }
}




