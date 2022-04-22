using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfterTime_Script : MonoBehaviour
{
    public float destroyDelay;
    public float timeAtDestruction;

    // Start is called before the first frame update
    void Start()
    {
        Invoke("Destroy", destroyDelay);
    }

    // Update is called once per frame
    void Destroy()
    {
        Destroy(gameObject);
    }
}
