using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "IdleState", menuName ="FiniteStateMachine/States/Idle", order = 1)]
public class IdleState : AbstractFSMState
{
    public override bool EnterState()
    {
        base.EnterState();
        Debug.Log("ENTERED IDLE STATE");

        EnteredState = true;
        return EnteredState;
    }

    public override void UpdateState()
    {
        Debug.Log("UPDATING IDLE STATE");
    }

    public override bool ExitState()
    {
        base.ExitState();
        Debug.Log("EXITING IDLE STATE");
        return true;
    }
}