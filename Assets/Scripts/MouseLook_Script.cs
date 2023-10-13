using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook_Script : MonoBehaviour
{
    public float mouseSens = 100f;
    public Transform PlayerBody;
    public float xRotation;
    bool isPlayerDead;

    private void Start()
    {
        PlayerController_Script.Action_PlayerDeath += HandleOnPlayerDeath;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void HandleOnPlayerDeath()
    {
        isPlayerDead = true;
    }

    private void Update()
    {
        if (!isPlayerDead) GetInput();
    }

    void GetInput()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSens;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSens;

        xRotation -= mouseY; // you subtract the mouseY input from the xRotation variable.
        xRotation = Mathf.Clamp(xRotation, -89.5f, 89.5f); // Clamping so we don't over-rotate our camera.

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        PlayerBody.Rotate(Vector3.up * mouseX);
    }
}
