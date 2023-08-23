using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ChaseRadiusScript : MonoBehaviour
{
    public bool insideRadius;

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "Player")
        {
            Debug.Log(other.gameObject.name + " has entered trigger.");
            insideRadius = true;
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.gameObject.name == "Player")
        {
            Debug.Log(other.gameObject.name + " has exited trigger.");
            insideRadius = false;
        }
    }

}
