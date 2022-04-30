using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook_Script : MonoBehaviour
{
    public float mouseSens = 100f;
    public Transform PlayerBody;
    public float xRotation;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        GetInput();
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
