using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShakeRecoil_Script : MonoBehaviour
{

    public Transform playerBody;
    public float xRotation;

    Vector3 currentRotation;
    Vector3 currentXRotation;
    Vector3 targetRotation;

    //hipfireRecoil


    //Settings
    [SerializeField] private float snappiness;
    [SerializeField] private float returnSpeed;

    //Script courtesy of this guy: -https://www.youtube.com/watch?v=geieixA4Mqc


    private void Update()
    {
        targetRotation = Vector3.Lerp(targetRotation, Vector3.zero, returnSpeed * Time.deltaTime);
        currentRotation = Vector3.Slerp(currentRotation, targetRotation, snappiness * Time.deltaTime);

        transform.localRotation = Quaternion.Euler(currentRotation);
    }

    public void Recoil(float recoilX, float recoilY, float recoilZ)
    {
        //Debug.Log("Recoil!");
        targetRotation += new Vector3(Random.Range((-.75f * recoilX), (-1.25f * recoilX)), Random.Range((0.25f * recoilY), recoilY), Random.Range(-recoilZ, recoilZ));
    }

    //public void Recoil(float recoilStrength)
    //{

    //    float recoilX = Random.Range(-recoilStrength * .5f, recoilStrength);
    //    float recoilY = Random.Range(-recoilStrength * .5f, recoilStrength * 3f);
    //    Debug.Log("recoil X =" + recoilX);
    //    Debug.Log("recoil Y =" + recoilY);

    //    //Upwards recoil
    //    xRotation -= recoilY;
    //    playerBody.rotation = Quaternion.Euler(xRotation, 0f, 0f);

    //    //Sideways recoil
    //    //playerBody.transform.Rotate(Vector3.up * recoilX);

    //}

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
