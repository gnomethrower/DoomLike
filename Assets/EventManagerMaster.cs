using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EventManagerMaster : MonoBehaviour
{
    public static Action Action_ToggleDebugMode;
    private bool debugModeEnabled = false;

    private void Start()
    {
        //Action_EnableDebugMode += EnableDebugMode;
        //Action_DisableDebugMode += DisableDebugMode;
        Action_ToggleDebugMode += OnToggleDebugMode;
    }

    private void Update()
    {
        if (UnityEngine.Input.GetKeyDown(KeyCode.F11))
        {
            Action_ToggleDebugMode?.Invoke();
        }
    }

    private void OnToggleDebugMode()
    {
        debugModeEnabled = !debugModeEnabled;
    }
}
