using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public interface IEnemyMovable
{
    #region Variables
    NavMeshAgent navAgent { get; set; }
    #endregion

    #region Methods
    void SetEnemyDestination(Vector3 destination);

    #endregion
}
