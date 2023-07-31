using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyIdleState : EnemyState
{

    #region Variables

    private Vector3 _wanderDestination;
    private Vector3 _direction;
    private NavMeshAgent _agent;
    private bool destinationReached = false;
    #endregion

    public EnemyIdleState(Enemy enemy, EnemyStateMachine enemyStateMachine) : base(enemy, enemyStateMachine)
    {

    }

    public override void AnimationTriggerEvent(Enemy.AnimationTriggerType triggerType)
    {
        base.AnimationTriggerEvent(triggerType);
    }

    public override void EnterState()
    {
        base.EnterState();
        _wanderDestination = GetWanderDestination();
        enemy.SetEnemyDestination(_wanderDestination);
    }

    public override void ExitState()
    {
        base.ExitState();
    }

    public override void FrameUpdate()
    {
        base.FrameUpdate();

        if(destinationReached)
        {
            enemy.SetEnemyDestination(_wanderDestination);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        if (enemy.IsAggroed)
        {
            enemy.StateMachine.ChangeState(enemy.ChaseState);
        }
    }

    private Vector3 GetWanderDestination()
    {
        Vector3 randomPointInSphere = Random.insideUnitSphere * enemy.IdleWanderRange;
        DebugIndicator.DrawDebugIndicator(randomPointInSphere, Color.yellow, 20f);
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomPointInSphere, out hit, enemy.IdleWanderRange, NavMesh.AllAreas))
        {
            Debug.Log("Success! Next Wanderpoint is " + hit.position + "!");
            Vector3 wanderPoint = hit.position;
            destinationReached = false;
            return wanderPoint;
        }
        Debug.Log("Failed to find a valid wander point. Returning default position.");
        return Vector3.zero;
    }
}
