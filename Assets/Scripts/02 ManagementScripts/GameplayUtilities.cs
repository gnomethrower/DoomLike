using UnityEngine;

public static class GameplayUtilities
{
    public static bool IsSightlineToPlayer(Vector3 playerPos, Vector3 enemyPos, float sightLineDistance)
    {
        int layerMask = 1 << 3;
        RaycastHit hit;
        if (Physics.Raycast(enemyPos, (playerPos - enemyPos).normalized, out hit, sightLineDistance, layerMask, QueryTriggerInteraction.Ignore))
        {
            return true;
        }
        return false;
    }

    public static bool IsInSightlineOf(Vector3 initialPos, Vector3 targetPos, float sightLineDistance, LayerMask targetLayermaskInt)
    {
        int layerMask = 1 << targetLayermaskInt;
        RaycastHit hit;
        if (Physics.Raycast(targetPos, (initialPos - targetPos).normalized, out hit, sightLineDistance, layerMask, QueryTriggerInteraction.Ignore))
        {
            return true;
        }
        return false;
    }

    public static void SpawnDebugSphere(Vector3 spawnPoint, float sphereRadius, float sphereOpacity, float duration, Color color)
    {
        GameObject debugSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        debugSphere.transform.position = spawnPoint;

        // Set color and transparency
        Renderer renderer = debugSphere.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material.color = new Color(1f, 0f, 0f, sphereOpacity); // Red with specified opacity
        }
        else Debug.Log(renderer.gameObject + " renderer is null");

        float sphereDiameter = sphereRadius * 2f;
        debugSphere.transform.localScale = new Vector3(sphereDiameter, sphereDiameter, sphereDiameter);

        // Destroy the sphere after the specified duration
        UnityEngine.Object.Destroy(debugSphere, duration);
    }
}



//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.Threading;
//using Unity.VisualScripting;
//using UnityEngine;
//using UnityEngine.AI;
//using static UnityEditor.Experimental.GraphView.Port;


//public class GameplayUtilities : MonoBehaviour
//{
//    private float sphereOpacity;
//    private object duration;
//    #region variables

//    #endregion

//    private void Start()
//    {

//    }

//    public bool IsSightlineToPlayer(Vector3 playerPos, Vector3 enemyPos, float sightLineDistance) // returns true, if the player is hit.
//    {
//        int layerMask = 1 << 3;
//        RaycastHit hit;
//        if (Physics.Raycast(enemyPos, (playerPos - enemyPos).normalized, out hit, sightLineDistance, layerMask, QueryTriggerInteraction.Ignore))
//        {
//            return true;
//        }
//        return false;
//    }

//    public bool IsInSightlineOf(Vector3 initialPos, Vector3 targetPos, float sightLineDistance, LayerMask targetLayermaskInt) // returns true, if the player is hit.
//    {
//        int layerMask = 1 << targetLayermaskInt; // 
//        RaycastHit hit;
//        if (Physics.Raycast(targetPos, (initialPos - targetPos).normalized, out hit, sightLineDistance, layerMask, QueryTriggerInteraction.Ignore))
//        {
//            return true;
//        }
//        return false;
//    }

//    public void SpawnDebugSphere(Vector3 spawnPoint, float sphereOpacity, Color color)
//    {
//        GameObject debugSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
//        debugSphere.transform.position = spawnPoint;

//        // Set red color and transparency
//        Renderer renderer = debugSphere.GetComponent<Renderer>();
//        if (renderer != null)
//        {
//            renderer.material.color = new Color(1f, 0f, 0f, sphereOpacity); // Red with specified opacity
//        }

//        // Destroy the sphere after the specified duration
//        Destroy(debugSphere, duration);
//    }

//    private void Destroy(GameObject redSphere, object duration)
//    {
//        throw new NotImplementedException();
//    }
//}
