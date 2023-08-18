using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedDestroyer : MonoBehaviour
{
    public float timeToDestroy;
    float timer;

    private void Update()
    {
        if (TimerFinished())
        {
            Destroy(gameObject);
        }
    }

    public bool TimerFinished()
    {
        if (timer < timeToDestroy)
        {
            timer += Time.deltaTime;
            return false;
        }

        return true;
    }
}
