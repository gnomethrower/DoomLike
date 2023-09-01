using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AttackRadiusScript : MonoBehaviour
{
    public bool insideAttackRadius;

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "Player")
        {
            //Debug.Log("Entered attack Radius of " + transform.parent.transform.parent.transform.parent.name + "!");
            insideAttackRadius = true;
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.gameObject.name == "Player")
        {
            //Debug.Log("Left attack Radius");
            insideAttackRadius = false;
        }
    }

}
