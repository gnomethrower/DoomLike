using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class GameplayUtilities : MonoBehaviour
{
    #region variables

    #endregion

    public bool IsSightlineToPlayer(Vector3 playerPos, Vector3 enemyPos, float sightLineDistance) // returns true, if the player is hit.
    {
        int layerMask = 1 << 3;
        RaycastHit hit;
        if (Physics.Raycast(enemyPos, (playerPos - enemyPos).normalized, out hit, sightLineDistance, layerMask, QueryTriggerInteraction.Ignore))
        {
            return true;
        }
        return false;
    }
   
}
