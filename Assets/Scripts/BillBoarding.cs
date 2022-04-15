using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BillBoarding : MonoBehaviour
{

    Vector3 mainCamDirection;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        mainCamDirection = Camera.main.transform.forward;
        mainCamDirection.y = 0f;

        transform.rotation = Quaternion.LookRotation(mainCamDirection);
    }
}
