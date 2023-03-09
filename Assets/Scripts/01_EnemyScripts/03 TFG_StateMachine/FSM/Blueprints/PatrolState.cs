using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static MantisEnemyAI;

[CreateAssetMenu(fileName = "PatrolState", menuName = "FiniteStateMachine/States/Patrol", order = 2)]
public class PatrolState : AbstractFSMState
{
    Vector3 nextPatrolPoint;

    [SerializeField]
    float patrolRadius = 5f;
    float pauseDurationMin = 1;
    float pauseDurationMax = 5;

    public bool pauseTimerFinished;
    public bool patrolPointIsSet;

    private bool hasReachedPatrolPoint;
    private int animationStateInteger;
    private float minimumDistanceToDestination;
    private int previousBehaviorStateInt;
    private int behaviorStateInteger;
    private int previousAnimStateInt;

    public override void OnEnable()
    {
        base.OnEnable();
    }

    public override bool EnterState()
    {
        if (base.EnterState())
        {
            Debug.Log("Started Patrol State");
            SetNewPatrolPoint();
        }

        EnteredState = true;
        return EnteredState;
    }

    public override bool ExitState()
    {
        base.ExitState();
        return true;
    }

    public override void UpdateState()
    {
        //Only executes every frame, if we successfully entered the state
        if (EnteredState)
        {
            
        }

    }

    #region PatrolPoint Functionality

    void Patrol()
    {
        if (!patrolPointIsSet && hasReachedPatrolPoint && pauseTimerFinished)
        {
            hasReachedPatrolPoint = false;
            pauseTimerFinished = false;

            PreviousAnimationStateUpdate();
            PreviousBehaviorStateUpdate();

            Debug.Log("2: Setting Walking Anim State at: " + Time.realtimeSinceStartup);
            animationStateInteger = ((int)MantisAnimationStates.WALKING);

            SetNewPatrolPoint();
        }

        if (_navMeshAgent.remainingDistance < minimumDistanceToDestination && !pauseTimerFinished && !hasReachedPatrolPoint)
        {
            PreviousAnimationStateUpdate();
            PreviousBehaviorStateUpdate();

            animationStateInteger = ((int)MantisAnimationStates.IDLING);

            hasReachedPatrolPoint = true;
            //Debug.Log("We've reached the Point, now pausing!");
            //StartCoroutine("PatrolPauseTimer"); TIMER DOESN'T WORK ANYMORE BECAUSE ITS NOT MONOBEHAVIOR
        }
    }

    void PreviousBehaviorStateUpdate()
    {
        if (previousBehaviorStateInt != behaviorStateInteger)
        {
            //Debug.Log("Updating previous Behavior State");
            previousBehaviorStateInt = behaviorStateInteger;
        }
    }
    void PreviousAnimationStateUpdate()
    {
        if (previousAnimStateInt != animationStateInteger)
        {
            //Debug.Log("Updating previous Anim State");
            previousAnimStateInt = animationStateInteger;
        }
    }

    private void SetNewPatrolPoint()
    {
        NextPatrolPoint();

        //Initializing movement towards patrol point
        _navMeshAgent.SetDestination(nextPatrolPoint);
        patrolPointIsSet = true;
    }

    private void NextPatrolPoint()
    {
        // Finds the next patrol point.
        // Does not check whether the patrol point is on the same floor.

        //This variable is needed to approximate a point near the Navmesh.
        Vector3 patrolPointApproximation;

        Vector3 patrolPositionOrigin;

        // This NavMeshHit point is needed to find the exact point on the Navmesh.
        NavMeshHit navHitPoint;

        patrolPositionOrigin = _navMeshAgent.transform.position;
        patrolPointApproximation = patrolPositionOrigin + Random.insideUnitSphere * patrolRadius;

        //Visualizing the original approximationpoint
        Debug.DrawLine(patrolPositionOrigin, patrolPointApproximation, Color.white, 1f);

        if (NavMesh.SamplePosition(patrolPointApproximation, out navHitPoint, patrolRadius, NavMesh.GetAreaFromName("Ground")))
        {
            nextPatrolPoint = navHitPoint.position;
        }
    }

    IEnumerator PatrolPauseTimer()
    {
        //Debug.Log("Pause timer started");
        float pauseTimer = Random.Range(pauseDurationMin, pauseDurationMax);
        yield return new WaitForSeconds(pauseTimer);

        pauseTimerFinished = true;
        patrolPointIsSet = false;
        //Debug.Log("Timer finished after " + pausePatrolForSeconds + " seconds.");
    }

    #endregion
}
