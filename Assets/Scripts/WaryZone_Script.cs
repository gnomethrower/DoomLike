using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaryZone_Script : MonoBehaviour
{
    public GameObject player;
    public PlayerController_Script playerScript;
    public Carl_State_Script carlStates;
    public SphereCollider warySphere;

    public float calmingCountdown;
    public float timeWhenCalmed;
    public float aggroCountdown;
    public float timeWhenAggro;
    public float currentTime;

    public bool calmingDown = false;
    public bool aggroing = false;

    private void Start()
    {
        calmingCountdown = carlStates.waryToPeacefulTime;
        aggroCountdown = carlStates.waryToAggroTime;
    }

    private void Update()
    {
        if (calmingDown) CalmingCountdown();
        if (aggroing) WaryToAggroCountdown();
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && carlStates.state == 0)
        {
            Debug.Log("I'm wary now!");
            carlStates.state = 3;
            calmingDown = false;
            aggroing = true;

            currentTime = Time.deltaTime;
            timeWhenAggro = currentTime + aggroCountdown;
        }
    }

    void WaryToAggroCountdown()
    {
        currentTime += Time.deltaTime;

        if (timeWhenAggro <= currentTime)
        {
            calmingDown = false;
            aggroing = false;
            carlStates.state = 4;
            Debug.Log("I'm aggro now!");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && carlStates.state == 1)
        {
            calmingDown = true;
            aggroing = false;
            Debug.Log("I'm starting to calm down!");
            currentTime = Time.deltaTime;
            timeWhenCalmed = currentTime + calmingCountdown;
        }
    }

    void CalmingCountdown()
    {
        currentTime += Time.deltaTime;

        if (timeWhenCalmed <= currentTime)
        {
            carlStates.state = 5;
            calmingDown = false;
            Debug.Log("I've calmed down!");
        }
    }

}
