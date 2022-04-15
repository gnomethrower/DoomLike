using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamShake : MonoBehaviour
{

    public IEnumerator Shaking(float duration, float strength)
    {
        Vector3 originalPositon = transform.localPosition;
        float timeSinceStart = 0.0f;
        float shakeFadeTime = strength / duration;
        float shakeStrengthFade = strength;

        while (timeSinceStart < duration)
        {
            float x = Random.Range(-.25f, .25f) * shakeStrengthFade;
            float y = Random.Range(-.25f, .25f) * shakeStrengthFade;

            transform.localPosition = new Vector3(x, y, 0f);

            timeSinceStart += Time.deltaTime;

            shakeStrengthFade = Mathf.MoveTowards(shakeStrengthFade, 0f, shakeFadeTime * Time.deltaTime);

            yield return null; //before the while routine continues, we let the next frame render
        }

        transform.localPosition = originalPositon;

    }
}
