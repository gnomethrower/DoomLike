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

    //Object Reference
    PlayerController_Script playerScript;

    //Settings
    [SerializeField] private float snappiness;
    [SerializeField] private float returnSpeed;

    //debug vars
    Vector3 currentPosition;
    Vector3 lastPosition;

    //Script courtesy of this guy: -https://www.youtube.com/watch?v=geieixA4Mqc

    private void Start()
    {
        playerScript = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController_Script>();
    }

    private void Update()
    {

        targetRotation = Vector3.Lerp(targetRotation, Vector3.zero, returnSpeed * Time.deltaTime);
        currentRotation = Vector3.Slerp(currentRotation, targetRotation, snappiness * Time.deltaTime);
        transform.localRotation = Quaternion.Euler(currentRotation);
    }

    public void Recoil(float recoilX, float recoilY, float recoilZ, float adsMP)
    {
        //Debug.Log("Recoil!");
        targetRotation += new Vector3(Random.Range((-.75f * recoilX * PlayerController_Script.spreadMultiplier * adsMP), (-1.25f * recoilX * PlayerController_Script.spreadMultiplier) * adsMP),
                                      Random.Range((0.25f * recoilY * PlayerController_Script.spreadMultiplier * adsMP), recoilY * PlayerController_Script.spreadMultiplier * adsMP),
                                      Random.Range(-recoilZ * (PlayerController_Script.spreadMultiplier / 2) * adsMP, (recoilZ * PlayerController_Script.spreadMultiplier / 2)) * adsMP);
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

        Vector3 originalPosition = transform.localPosition;
        float timeSinceStart = 0.0f;

        while (timeSinceStart < duration)
        {
            float x = Random.Range(-1f, 1f) * strength;
            float y = Random.Range(-1f, 1f) * strength;

            // Apply recoil as an offset in the local position
            transform.localPosition += new Vector3(x, y, 0f);

            timeSinceStart += Time.deltaTime;

            yield return null;
        }

        // Reset the camera's local position to the original position
        transform.localPosition = originalPosition;



        //Vector3 originalPositon = transform.localPosition;
        ////Debug.Log(playerBody.transform.position);
        ////Debug.Log(originalPositon);

        //float timeSinceStart = 0.0f;
        //float shakeFadeTime = strength / duration;
        //float shakeStrengthFade = strength;

        //while (timeSinceStart < duration)
        //{
        //    float x = Random.Range(-.25f, .25f) * shakeStrengthFade;
        //    float y = Random.Range(-.25f, .25f) * shakeStrengthFade;

        //    transform.localPosition = new Vector3(x, y, 1f);

        //    timeSinceStart += Time.deltaTime;

        //    shakeStrengthFade = Mathf.MoveTowards(shakeStrengthFade, 0f, shakeFadeTime * Time.deltaTime);

        //    yield return null; //before the while routine continues, we let the next frame render
        //}

        //transform.localPosition = originalPositon;

    }

}
