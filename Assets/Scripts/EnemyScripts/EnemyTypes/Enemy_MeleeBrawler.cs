using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Enemy_MeleeBrawler : MonoBehaviour
{
    //Enemy type MeleeBrawler
    EnemyHealthClass myHealthClass;
    EnemyMovementClass myMovementClass;
    EnemyAttackClass myAttackClass;

    float meleeBrawlerHealth = 100;

    private void Start()
    {
        myHealthClass = gameObject.AddComponent<EnemyHealthClass>() as EnemyHealthClass;
        myHealthClass.Init(meleeBrawlerHealth);
    }

    private void Update()
    {

    }
}
