using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class _EnemyBase : MonoBehaviour
{

    /* This class is supposed to bind together the movement, currentHealth and attack classes into a single _spawnObject behavior.
     * The _spawnObject should be controlled with a Finite State Machine.
     * 1) If State 0 is active, the _spawnObject is supposed to be idle and not do anything.
     * 2) If State 1 is active, the _spawnObject is supposed to patrol around their spawnpoint.
     * 3) If State 2 is active, the _spawnObject is supposed to follow and attack the player.
     */

    int state;

    public float baseEnemySpeed;
    public float baseEnemyPatrolRange;

    private void Start()
    {
        state = 0; //Idle is the default state.

        //In order to be able to use the movement methods from EnemyMovementClass, we need to instantiate a moveScript object according to EnemyMovementClass.
        EnemyMovementClass enemyMove1 = new EnemyMovementClass();
        enemyMove1.enemySpeed = baseEnemySpeed;
        enemyMove1.patrolRange = baseEnemyPatrolRange;
    }

    private void Update()
    {
        if (state == 0)
        {
            
        } else if (state == 1)
        {
            // Patrol
        } else if (state == 2)
        {
            // Aggro
        }
    }
}
