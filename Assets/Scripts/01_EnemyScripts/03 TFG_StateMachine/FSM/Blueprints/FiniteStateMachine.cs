using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FiniteStateMachine : MonoBehaviour
{
    [SerializeField]
    AbstractFSMState _startingState; //For experimentation purposes, we want to expose this field, in order to drag and drop a starting state into this field.

    AbstractFSMState _currentState;

    public void Awake()
    {
        _currentState = null; // To be safe, set the _currentState to null, in order to avoid any interferences that might be caused.
    }

    public void Start()
    {
        if(_startingState != null)
        {
            EnterState(_startingState);
        }
    }

    public void Update()
    {
        if (_currentState != null)
        {
            _currentState.UpdateState();
        }
    } 


    #region STATE MANAGEMENT

    public void EnterState(AbstractFSMState nextState)
    {
        if(_startingState == null)
        {
            return;
        }

        _currentState = nextState;
        _currentState.EnterState();
    }

    #endregion
}
