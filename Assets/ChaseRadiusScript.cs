using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ChaseRadiusScript : MonoBehaviour
{
    public bool insideChaseRadius;

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "Player")
        {
            insideChaseRadius = true;
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.gameObject.name == "Player")
        {
            insideChaseRadius = false;
        }
    }

}
