using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovementClass : MonoBehaviour
{

    //Class which contains methods pertaining to enemy movement.

    // Defining the transform of the object this script is attached to.
    Transform enemy;
    Vector3 spawnPos;

    // Public Movement Variables
    public float moveSpeed;
    public float enemyHeight;
    public bool canFly;

    void Start()
    {
        // Define the Transform of the object this script is attached to.
        enemy = gameObject.transform;
        //Defining the current location as the spawnPosition. We will need this for Patrol().
        spawnPos = transform.position;
    }

    public void NoMove()
    {
        //Standing still, facing one direction.
    }

    public void IdleTurning(float rotationDegree) //rotationDegree is halved.
    {
        /* Standing still but turning back and forth in degrees, like a stationary turret.
         * Startingrotation is the middle of the rotation cycle. Starts rotating until rotationDegree is reached
         * Then proceeds until -rotationDegree is reached. Repeat until enemy enters LoS.
         */
    }

    public void AggroMovement(float aggroSpeedMod, float meleeRange)
    {
        float aggroSpeed = moveSpeed * aggroSpeedMod;
        // The movement needed to close distance to the enemy player, according to the meleeRange.
    }

    public void PatrolMovement(float patrolRange, float listenRange, float aggroRange)
    {
        /*
         * Simple Patrolling behaviour, where entity patrols around their spawn spot, "searching" for the player.
         * Enter Conditions:
         * 1. Player shoots within listeningRange
         * 2. Entity has lost sight of player for 5 seconds while aggro.
         * 
         * Exit Conditions:
         * 1. Player damages the entity
         * 2. Player enters direct aggro range (brownie points: not through walls)
         * 
         * Algorithm:
         * 1. Take the spawnposition.
         * 2. Take a random number x betwen -patrolrange and patrolrange. Do the same with y.
         * 3. Offset the new Vector 3 movePoint from the spawnPos.
         * 4. Move entity to movePoint.
         * 5. Restart.
         * 
         */
    }
}
