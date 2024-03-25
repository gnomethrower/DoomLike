using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BillboardingAxesScript : MonoBehaviour
{
    Vector3 mainCamDirection;


    void Update()
    {
        mainCamDirection = Camera.main.transform.forward;
        mainCamDirection.y = 0f;

        transform.rotation = Quaternion.LookRotation(mainCamDirection);
    }
}
