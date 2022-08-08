using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;


public class SetWalkpointTask : Action
{
    public BehaviorTree enemyCore;
    public float walkPointRange, debugLineDuration;
    public Vector3 nextWalkPoint;
    public Vector3 walkingVector;
    public Vector3 currentPosition;
    public Vector3 verticalOffset;

    public SharedVector3 shared_nextWalkPoint;

    Vector3 offsetCurrentPos;
    Vector3 offsetNextWP;

    LayerMask whatIsGround;

    public void Start()
    {
        whatIsGround = LayerMask.GetMask("Ground");
        debugLineDuration = 10f;
        verticalOffset = new Vector3(0f, 0.5f, 0f);
    }

    public override TaskStatus OnUpdate()
    {
        //we make some object definitions:
        //we define layermask


        //we define the currentposition, offset it and print it for debugging purposes
        currentPosition = transform.position;
        offsetCurrentPos = currentPosition + verticalOffset;

        Debug.Log("Current Pos is " + currentPosition);

        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        nextWalkPoint = new Vector3(currentPosition.x + randomX, currentPosition.y, currentPosition.z + randomZ);
        offsetNextWP = nextWalkPoint + verticalOffset;
        Debug.Log("We found a new Walkpoint: " + nextWalkPoint);


        RaycastHit hitCollider;
        if (Physics.Raycast(offsetCurrentPos, offsetNextWP - offsetCurrentPos, out hitCollider, 2f * walkPointRange, whatIsGround))
        {
            Debug.Log("Raycast has hit something");
            Debug.DrawLine(offsetCurrentPos, offsetNextWP, Color.red, debugLineDuration);
            Debug.Log("Line to new WP has been drawn");
            /**walkingVector = nextWalkPoint - currentPosition;
            Debug.Log(walkingVector);
            walkingVector.y = 0f;
            nextWalkPoint = hitCollider.point - walkingVector.normalized;  **/
        }

        RaycastHit hitGroundCheck;
        if (Physics.Raycast(offsetNextWP, -Vector3.up, out hitGroundCheck, 2f, whatIsGround))
        {
            Debug.DrawLine(offsetNextWP, hitGroundCheck.point, Color.green, debugLineDuration);
            Debug.Log("We have hit the ground from new Walkpoint Down");
            return TaskStatus.Success;
        }

        else
        {
            Debug.Log("Walkpoint is not near ground!");
            return TaskStatus.Failure;
        }
    }

}
