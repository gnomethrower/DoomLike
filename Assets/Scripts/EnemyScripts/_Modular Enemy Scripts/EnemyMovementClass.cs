using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovementClass : MonoBehaviour
{

    //Class which contains methods pertaining to enemy movement.

    // Defining the transform of the object this script is attached to.
    Transform enemy;
    Vector3 spawnPos;
    LayerMask groundLayer;

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
        groundLayer = LayerMask.GetMask("Ground");
    }

    private void Update()
    {
        if (Input.GetButtonDown("Submit"))
        {
            Debug.Log("Enter is pressed");
            PatrolMovement(10f, 5f, 5f, groundLayer);
        }
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

    public void PatrolMovement(float patrolRange, float listenRange, float aggroRange, LayerMask layermaskCollider)
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
        Vector3 startingPoint = spawnPos;


        //Getting a new point by using a random number between -patrolRange and patrolRange for x and z of the new patrolPoint. for Y, we declare the same height as current position.
        float x = startingPoint.x + Random.Range(-patrolRange, patrolRange);
        float z = startingPoint.z + Random.Range(-patrolRange, patrolRange);
        Vector3 patrolPoint = new Vector3(x, enemy.position.y, z);
        Debug.Log(patrolPoint);

        /** A short excursion into vector calculation!
        *  1) This is the formula of calculating the distance between two points:PQ→=(Xq−Xp,Yq−Yp,Zq−Zp)
        *  2) If we calculate the magnitude/length of the difference, we get the distance between the two vectors.
        *  3) If we normalize the length, we get the direction.  
        */

        // 1) Calculating distance between startingPoint and patrolPoint:

        Vector3 distance = patrolPoint - startingPoint;
        Vector3 direction = distance.normalized;
        RaycastHit hit;
        if (Physics.Raycast(startingPoint, direction, out hit, patrolRange, layermaskCollider))
        {
            Debug.Log(hit.transform.name);
        }
        Debug.DrawLine(spawnPos, patrolPoint, Color.red, 5f);

        // TODO: If a wall is hit, Calculate a length of 2f back towards the startingpoint and let the enemy stop there.
    }
}
