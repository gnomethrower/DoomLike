using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// Taken from Table Flip Games Tutorial
// This abstract class is a blueprint to create all subsequent states based on this FSM architecture.

public enum ExecutionState
{
    NONE,
    ACTIVE,
    COMPLETED,
    TERMINATED, // Failure of executing the state
};

public abstract class AbstractFSMState : ScriptableObject
{

    protected NavMeshAgent _navMeshAgent;
    protected NPC _npc;

    public ExecutionState ExecutionState { get; protected set; } // This declares a property of the enumerator ExecutionState with the accessors get and set.
    public bool EnteredState { get; protected set; } 

    public virtual void OnEnable()
    {
        ExecutionState = ExecutionState.NONE;
    }

    public virtual bool EnterState() // Whenever we execute a state in the FSM, we enter the state successfully and maybe executes some logic.
    {
        bool successNavMesh = true;
        bool successNPC = true;

        ExecutionState = ExecutionState.ACTIVE;

        // Returns true or fals depending on: Does the navMeshAgent exist?
        successNavMesh = (_navMeshAgent != null);

        // Returns true or false depending on: Does the executing agent exist?
        successNPC = (_npc != null);

        return successNavMesh;
    }

    public abstract void UpdateState(); // Update the state on every frame of the game.


    public virtual bool ExitState() // Opposite of EnterState. A virtual method can be overridden 
    {
        ExecutionState = ExecutionState.COMPLETED;
        return true;
    }

    public virtual void SetNavmeshAgent(NavMeshAgent navMeshAgent)
    {
        if(_navMeshAgent != null)
        {
            _navMeshAgent = navMeshAgent;
        }
    }

    public virtual void SetExecutingNPC(NPC npc)
    {
        if(_npc != null)
        {
            _npc = npc;
        }
    }
}
