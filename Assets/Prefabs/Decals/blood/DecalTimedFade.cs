using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

public class DecalTimedFade : MonoBehaviour
{
    [Tooltip("After how many seconds the fade is supposed to start")]
    [SerializeField] private float startToFadeAfterSeconds;
    [Tooltip("How long the fade is supposed to take")]
    [SerializeField] private float fadingTimeInSeconds;

    private int fadingState;
    private float timer;
    private DecalProjector decalProjector;

    private void Start()
    {
        decalProjector = GetComponent<DecalProjector>();
        if (decalProjector == null) Debug.Log("No projector");
        timer = 0;
    }

    private void Update()
    {
        DestroyAfterFade(startToFadeAfterSeconds);
    }

    private void DestroyAfterFade(float timerDuration)
    {
        switch (fadingState)
        {
            case 0:

                if (timer < startToFadeAfterSeconds)
                {
                    timer += Time.deltaTime;
                }
                else if (timer >= startToFadeAfterSeconds)
                {
                    fadingState++;
                    timer = 0;
                }
                break;

            case 1:
                decalProjector.fadeFactor -= Time.deltaTime / fadingTimeInSeconds;
                if (decalProjector.fadeFactor <= 0)
                {
                    fadingState++;
                }
                break;

            case 2:
                Destroy(gameObject);
                break;
        }


    }
}

