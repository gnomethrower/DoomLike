using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;


public class SetWalkpointTask : Action
{
    public float walkPointRange, debugLineDuration;
    public Vector3 nextWalkPoint;
    public Vector3 currentPos;
    LayerMask whatIsGround;    

    public override TaskStatus OnUpdate()
    {
        whatIsGround = LayerMask.GetMask("Ground");

        Debug.Log("Current Pos is " + currentPos);
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        nextWalkPoint = new Vector3(currentPos.x + randomX, currentPos.y, currentPos.z + randomZ);

        RaycastHit hitCollider;
        if (Physics.Raycast(currentPos, nextWalkPoint - currentPos, out hitCollider, 2f * walkPointRange, whatIsGround))
        {
            Vector3 walkingVector = nextWalkPoint - currentPos;
            //Debug.Log(walkingVector);
            //walkingVector.y = 0f;
            nextWalkPoint = hitCollider.point - walkingVector.normalized;
            Debug.DrawLine(currentPos, nextWalkPoint, Color.red, debugLineDuration);
        }

        RaycastHit hitGroundCheck;
        if (Physics.Raycast(nextWalkPoint, -Vector3.up, out hitGroundCheck, 2f, whatIsGround))
        {
            Debug.DrawLine(nextWalkPoint, hitGroundCheck.point, Color.green, debugLineDuration);
            
            return TaskStatus.Success;

        } else return TaskStatus.Failure;

    }

}
