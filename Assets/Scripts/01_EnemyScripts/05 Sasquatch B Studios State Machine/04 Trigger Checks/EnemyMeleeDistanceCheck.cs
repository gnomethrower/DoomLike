using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMeleeDistanceCheck : MonoBehaviour
{
    public GameObject PlayerTarget { get; set; }
    private Enemy _enemy;

    private void Awake()
    {
        PlayerTarget = GameObject.FindGameObjectWithTag("Player");
        _enemy = GetComponentInParent<Enemy>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == PlayerTarget)
        {
            Debug.Log("hello from the melee collider");
            _enemy.SetMeleeDistanceBool(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == PlayerTarget)
        {
            _enemy.SetMeleeDistanceBool(false);
        }
    }
}
