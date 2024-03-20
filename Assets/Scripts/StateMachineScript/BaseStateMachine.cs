using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using static UnityEditorInternal.VersionControl.ListControl;

public class BaseStateMachine : MonoBehaviour
{
    //THIS IS A COPY-ABLE STATE MACHINE BASE CLASS - NOT FOR INHERITANCE

    private int lastState;
    private int currentState;

    void SetState(int nextState)
    {
        if (nextState == currentState)
        {
            return;
        }

        ExitState(currentState);
        lastState = currentState;
        currentState = nextState;

        InitializingNextState(currentState);
    }
    void ExitState(int lastState)
    {
        switch (lastState)
        {
            case 0:
                break;
            case 1:
                break;
            case 2:
                break;
            case 3:
                break;
        }
    }
    void InitializingNextState(int initState)
    {
        switch (initState)
        {
            case 0:
                break;
            case 1:
                break;
            case 2:
                break;
            case 3:
                break;
        }
    }
    void StateFrameUpdate()
    {
        switch (currentState)
        {
            case 0:
                break;
            case 1:
                break;
            case 2:
                break;
            case 3:
                break;
        }
    }
}
