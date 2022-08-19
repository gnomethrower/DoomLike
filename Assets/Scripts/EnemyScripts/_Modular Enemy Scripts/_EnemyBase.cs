using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class _EnemyBase : MonoBehaviour
{

    public GameObject playerObj;
    public GameObject thisEnemyObj;

    EnemyMovementClass moveScript;

    int currentState = 0;
    int nextState = 0;
    private void Awake()
    {
        EnemyMovementClass moveScript = new EnemyMovementClass();
    }

    public int GetNextState(int nextState)
    {
        int enemyState;
        switch(currentState){
            case 0: enemyState = 0; //idle
                        break;

            case 1: enemyState = 1; //patrol
                        break;

            case 2:
                    enemyState = 2;     //aggro
                        break;

            default:
                    enemyState = 0;     //default:Idle
                        break;

        } 
        return enemyState;
    }

    private void Update()
    {
        if(currentState != nextState)
        {
            GetNextState(nextState);
        }

        if(currentState == 0)
        {
            //IdleAlgo
        }

        if(currentState == 1)
        {
            //PatrolAlgo
        }

        if (currentState == 2)
        {
            //AggroAlgo
        }
    }    
}
