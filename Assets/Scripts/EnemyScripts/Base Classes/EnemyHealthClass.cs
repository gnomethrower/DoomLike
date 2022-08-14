using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealthClass : MonoBehaviour
{
    //Declaring everything to do with health and the change thereof.
    float healthPointMax;
    float healthPoints;

    public void Init(float healthMax)
    {
        healthPointMax = healthMax;
        healthPoints = healthPointMax;
    }

    public void Death()
    {
        //Deathanimation & Sound
        //Spawn Dead Body
        //possible Loot?
    }

    public void TakeDamage(float damage)
    {
        healthPoints -= damage;
        if (healthPoints <= 0)
        {
            Death();
        }
    }
}
