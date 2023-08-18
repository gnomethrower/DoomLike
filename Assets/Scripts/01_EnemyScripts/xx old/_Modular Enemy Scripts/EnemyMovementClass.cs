using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMovementClass : MonoBehaviour
{

    //Class which contains methods pertaining to _spawnObject movement.

    // Defining the transform of the object this script is attached to.
    Transform enemy;
    Vector3 spawnPos;
    LayerMask groundLayer;
    NavMeshAgent agent;
    GameObject playerObjRef;
    Vector3 playerPos;

    // Public Movement Variables
    public float enemySpeed;
    public float patrolRange;

    void Start()
    {
        // Define the Transform of the object this script is attached to.
        enemy = gameObject.transform;
        //Defining the current location as the spawnPosition. We will need this for Patrol().
        spawnPos = transform.position;
        //offset the positon on y by 1 so the raycast doesnt intersect with the floor.
        spawnPos.y += 1;
        //defining what we mean by groundlayer
        groundLayer = LayerMask.GetMask("Ground");
        //defining the agent variable by making it look for the navmeshagent component on this object.
        agent = GetComponent<NavMeshAgent>();
        //defining a set reference to the player
        playerObjRef = GameObject.FindGameObjectWithTag("Player");
    }

    private void Update()
    {
        
    }

    public void NoMove()
    {
        agent.SetDestination(transform.position);
    }

    public void IdleTurning(float rotationDegree) //rotationDegree is halved.
    {
        /* Standing still but turning back and forth in degrees, like a stationary turret.
         * Startingrotation is the middle of the rotation cycle. Starts rotating until rotationDegree is reached
         * Then proceeds until -rotationDegree is reached. Repeat until _spawnObject enters LoS.
         */
    }

    public void AggroMovement(float aggroSpeed, float meleeRange, Vector3 playerPos)
    {
        enemySpeed = aggroSpeed;
        agent.speed = enemySpeed;
        agent.SetDestination(playerPos);
        // The movement needed to close distance to the _spawnObject player, according to the meleeRange.
    }

    public void GetNewPatrolPoint(float patrolRange, float patrolSpeed, LayerMask layermaskCollider)
    {
        patrolSpeed = 5f;
        enemySpeed = patrolSpeed;
        agent.speed = enemySpeed;
        
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
         * 1. Take the spawnPos.
         * 2. Take a random number x betwen -patrolrange and patrolrange and subtract it from the current spawnPosition. Do the same with y.
         * 3. Offset the new Vector 3 patrolPoint from the spawnPos.
         * 4. Move entity to movePoint.
         * 5. Restart.
         * 
         */


        //1.) & 2.) Getting a new point by using a random number between -patrolRange and patrolRange for x and z of the new patrolPoint. for Y, we declare the same height as current position.
        float x = spawnPos.x + Random.Range(-patrolRange, patrolRange);
        float z = spawnPos.z + Random.Range(-patrolRange, patrolRange);
        Vector3 patrolPoint = new Vector3(x, spawnPos.y, z);

        /** A short excursion into vector calculation!
        *  1) This is the formula of calculating the distance between two points P-Starting Point, Q-Endpoint :PQ→=(Xq−Xp,Yq−Yp,Zq−Zp)
        *  2) If we calculate the magnitude/length of the difference, we get the distance between the two vectors.
        *  3) If we normalize the length, we get the direction.  
        */

        Vector3 distance = patrolPoint - spawnPos;
        Vector3 direction = distance.normalized;

        //Checking whether or not the patrolPoint is behind a wall - if yes, correcting the patrol point by shortening the vector distance.
        RaycastHit hit;
        if (Physics.Raycast(spawnPos, direction, out hit, patrolRange, layermaskCollider))
        {
            // took me way too long: http://answers.unity.com/comments/1632403/view.html
            Vector3 spawnToHitPoint = hit.point - spawnPos;
            Vector3 correctedPatrolPoint = hit.point - (spawnToHitPoint.normalized*.9f);

            patrolPoint = correctedPatrolPoint;
        } Debug.DrawLine(spawnPos, patrolPoint, Color.green, 5f);

        agent.SetDestination(patrolPoint);

    }


}
