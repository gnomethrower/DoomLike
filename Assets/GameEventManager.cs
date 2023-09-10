using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameEventManager : MonoBehaviour
{

    //This script is to act as the a global event subscriber.

    /// <summary>
    /// 
    /// *****************************************************************
    /// 
    /// Next Up: Add a Counter that tracks all the enemies in the scene.
    /// 
    /// *****************************************************************
    /// 
    /// </summary>

    #region Inspector Variables
    [Tooltip("The amount of seconds from showing deathscreen until you can restart the game")]
    [SerializeField] private float secondsUntilRestartPrompt = 5;
    #endregion

    #region Background Variables
    public bool playerIsDead = false;
    private float timer = 0;
    private int timerState = 0;

    private int currentEnemyCount;
    #endregion

    #region References
    [SerializeField] private GameObject deathScreenPrefab;
    [SerializeField] private GameObject pressAnyKeyPrefab;

    private GameObject playerUICanvas;
    private GameObject aliveUIElements;


    #endregion

    public void Awake()
    {
        #region Player Death
        PlayerController_Script.OnPlayerDeath += HandlePlayerDeath;

        playerUICanvas = GameObject.Find("PlayerUICanvas");
        aliveUIElements = GameObject.Find("UI-Alive-Elements");

        if (deathScreenPrefab == null) Debug.Log("No Deathscreen found!");
        if (aliveUIElements == null) Debug.Log("No AliveUI found!");
        #endregion

        #region
        BasicMantisClass.OnMantisSpawn += HandleMantisSpawn;
        BasicMantisClass.OnMantisDeath += HandleMantisDeath;
        #endregion
    }

    private void Update()
    {
        PayerDeathUpdate();

        TrackingEnemyCount();
    }

    private void TrackingEnemyCount()
    {

    }

    #region Events

    // ++++++++++++++++++++ Player Death ++++++++++++++++++++
    private void HandlePlayerDeath()
    {

        Debug.Log("Player is dead!");
        aliveUIElements.SetActive(false);
        Instantiate(deathScreenPrefab, playerUICanvas.transform.position, Quaternion.identity, playerUICanvas.transform);
        playerIsDead = true;
        PlayerController_Script.OnPlayerDeath -= HandlePlayerDeath;
    }
    private void PayerDeathUpdate()
    {
        if (playerIsDead)
        {
            ShowDeathscreen();
        }
    }
    private void ShowDeathscreen()
    {
        switch (timerState)
        {
            case 0:
                if (timer < secondsUntilRestartPrompt)
                {
                    timer += Time.deltaTime;
                }
                else
                {
                    Instantiate(pressAnyKeyPrefab, playerUICanvas.transform.position + new Vector3(0, -100f, 0), Quaternion.identity, playerUICanvas.transform);
                    Debug.Log("Deathscreentimer done!");
                    timerState++;
                }
                break;

            case 1:
                if (Input.GetKey(KeyCode.Space))
                {
                    SceneManager.LoadScene("DC_Testmap");
                }
                break;
        }



    }

    // ++++++++++++++++++++ Enemy Handling ++++++++++++++++++++


    #endregion

}
