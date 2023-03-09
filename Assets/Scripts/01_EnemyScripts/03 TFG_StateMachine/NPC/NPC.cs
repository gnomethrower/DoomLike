using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent), typeof(FiniteStateMachine))] // automatically adds the components required in the typeof brackets

public class NPC : MonoBehaviour
{

    NavMeshAgent _navMeshAgent;
    FiniteStateMachine _finiteStateMachine; //instantiates a copy of the FiniteStateMachine class

    // We need to store 2 variables: Reference to NavMeshAgent and to the FiniteStateMachine
    public void Awake()
    {
        _navMeshAgent = this.GetComponent<NavMeshAgent>();
        _finiteStateMachine = this.GetComponent<FiniteStateMachine>();
    }

    public void Start()
    {
        
    }

    public void Update()
    {
        
    }
}
