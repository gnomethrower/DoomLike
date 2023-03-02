using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealthClass : MonoBehaviour
{

    Transform enemy;

    //Class variables
    float healthMax;
    float health;


    void Start()
    {
        //initializing this gameobject's transform as enemy.
        enemy = gameObject.transform;
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        if(health <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        /* 1. Deactivate all behavior.
         * 2. Play death animation.
         * 3. Spawn corpse prefab. (and maybe loot)
         * 4. Instantly afterwards, destroy GameObject.
         */
    }
}
