using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnerScript : MonoBehaviour
{
    #region Variables

    #region object references
    [SerializeField] private GameObject _spawnObject;
    #endregion

    #region Spawner Variables
    [SerializeField] int maxEnemySpawnNumber;
    int spawnedEnemyNumber = 1;
    [SerializeField] float spawnTimer;
    [Tooltip("If you select this, spawnTimer will be ignored in favour of spawnTimerMin and spawnTimerMax")]
    [SerializeField] bool randomSpawnTimerRange;
    [SerializeField] bool infiniteSpawner;
    [SerializeField] float spawnTimerMin;
    [SerializeField] float spawnTimerMax;
    float timeSinceLastSpawn = 0;
    bool spawningFinished;

    #endregion

    #endregion


    private void Awake()
    {
        if (randomSpawnTimerRange) { spawnTimer = Random.Range(spawnTimerMin, spawnTimerMax); }
    }

    private void Update()
    {
        if (spawningFinished) { return; }
        if (SpawnTimerDone() && CanSpawnEnemy())
        {
            SpawnGameObject(_spawnObject);
        }

    }

    private bool CanSpawnEnemy()
    {
        if (infiniteSpawner) { return true; }

        //check if this has spawned more enemies than allowed.
        if (spawnedEnemyNumber < maxEnemySpawnNumber + 1)
        {
            //Debug.Log("I have spawned enemy no.:" + spawnedEnemyNumber);
            spawnedEnemyNumber++;
            return true;
        }
        else
        {
            //Debug.Log("All enemies have been spawned");
            KillSpawnerObject();
            return false;
        }
    }

    private bool SpawnTimerDone()
    {
        if (randomSpawnTimerRange)
        {
            spawnTimer = Random.Range(spawnTimerMin, spawnTimerMax);
        }
        if (timeSinceLastSpawn < spawnTimer) //if the timer is not done.
        {
            timeSinceLastSpawn += Time.deltaTime;
            return false;
        }

        else if (timeSinceLastSpawn >= spawnTimer) //if the timer is done
        {
            return true;
        }
        return false;
    }

    void SpawnGameObject(GameObject objectToSpawn)
    {
        timeSinceLastSpawn = 0;
        if (objectToSpawn == null) { Debug.Log("No Enemy has been set for this spawner"); return; }
        else
        {
            Instantiate(objectToSpawn, transform.position, Quaternion.identity);
        }
    }

    void KillSpawnerObject()
    {
        //Play animation of the Portal closing?++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        Destroy(transform.root.gameObject);
    }

}
