using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAggroCheck : MonoBehaviour
{

    public GameObject PlayerTarget { get; set; }
    private Enemy _enemy;
    [SerializeField] private float eyeOffset;

    private void Awake()
    {
        PlayerTarget = GameObject.FindGameObjectWithTag("Player");
        _enemy = GetComponentInParent<Enemy>();

        PlayerTarget = GameObject.FindGameObjectWithTag("Player");

        #region nullref check

        if (PlayerTarget == null)
        {
            Debug.LogError("Player target not found!");
        }
        _enemy = GetComponentInParent<Enemy>();
        if (_enemy == null)
        {
            Debug.LogError("Enemy component not found in parent GameObject!");
        }
        #endregion

    }

    private void OnTriggerEnter(Collider other)
    {

        #region nullref check
        if (PlayerTarget == null || _enemy == null)
        {
            return;
        }
        #endregion

        if (CanSeePlayer())
        {
            if (other.gameObject == PlayerTarget)
            {
                _enemy.SetAggroStatus(true);
            }
        }
        else return;
    }

    private void OnTriggerExit(Collider other)
    {
        #region nullref check
        if (PlayerTarget == null || _enemy == null)
        {
            return;
        }
        #endregion

        if (other.gameObject == PlayerTarget)
        {
            _enemy.SetAggroStatus(false);
        }
    }

    private bool CanSeePlayer()
    {
        // check if there is any level geometry between the enemy and the player.
        int layerMask = 1 << 3;
        RaycastHit hit;
        Vector3 enemyEyeHeight = _enemy.transform.position + new Vector3(0f, eyeOffset, 0f);

        if(Physics.Raycast(enemyEyeHeight, (PlayerTarget.transform.position - enemyEyeHeight).normalized, out hit, 100f, layerMask, QueryTriggerInteraction.Ignore))
        {
            Debug.Log(_enemy + " can see the player");
            Debug.DrawLine(enemyEyeHeight, hit.point, Color.red, 10f);
            return true;
        }
        else
        {
            Debug.Log("No player in sightline");
            return false;
        }
    }
}
