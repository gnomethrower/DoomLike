using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaryZone_Script : MonoBehaviour
{
    public GameObject player;
    public PlayerController_Script playerScript;
    public Carl_State_Script carlStates;
    public SphereCollider warySphere;

    public float calmingDuration;
    public float timeWhenCalmed;
    public float currentTime;
    public bool calmingDown = false;

    private void Start()
    {
        calmingDuration = carlStates.waryToPeaceful;
    }

    private void Update()
    {
        if (calmingDown) CalmingProcess();
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && carlStates.state == 0)
        {
            Debug.Log("I'm wary now!");
            carlStates.state = 1;
            calmingDown = false;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && carlStates.state == 1)
        {
            calmingDown = true;
            Debug.Log("I'm starting to calm down!");
            currentTime = Time.deltaTime;
            timeWhenCalmed = currentTime + calmingDuration;

        }
    }

    void CalmingProcess()
    {
        currentTime += Time.deltaTime;

        if (timeWhenCalmed <= currentTime)
        {
            carlStates.state = 0;
            calmingDown = false;
            Debug.Log("I've calmed down!");
        }
    }

}
