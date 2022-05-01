using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Carl_State_Script : MonoBehaviour
{
    //3 zones:
    //1. Aggro Zone
    //2. Wary Zone
    //3. Hearing Range

    //3 States:
    //0 Peaceful Idle
    //1 Wary
    //2 Aggressive

    //Peaceful Idle: rotates around, gazing, paying you no further attention.
    //when you enter Carl's Wary Zone, he becomes wary.
    //If you fire while in his Hearing Range he becomes wary.

    //Wary: You get too close, he locks onto you, maybe making a hissing sound to warn you off!
    // if he is wary and you are not in his wary zone for 5s, he becomes idle again.

    //Aggressive: He runs at you, trying to kill you.
    //if you shoot him, he becomes aggressive.
    //if you stay inside the wary range for 1s he will hiss again, after 2s he will growl after 3s he will become aggressive.
    //if you enter Aggro Zone, he gets aggressive immediately.
    //he will never exit Aggressive state, unless player dies.


    //Variables to fiddle with

    public int startingState = 0;
    public int state;
    [Header("Variables for Peaceful State")]

    [Header("Rotation Angles")]
    [SerializeField] float currentAngle;
    [SerializeField] float startingAngle;
    [SerializeField] float targetAngle;

    public float minCoolDown, maxCooldown;
    float coolDown;
    float coolDownControl;
    public float maxGazingVariation;

    public float turnSpeed;

    Quaternion _targetRotation;
    public float rotatingAngle;
    [SerializeField] private float turnTime;
    [SerializeField] private float timeControl;

    //controlbools
    public bool rotationFinished;
    public bool isCooldownFinished;

    [Header("Variables for Wary State")]
    public float waryToPeaceful;

    //objects
    GameObject player;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        startingAngle = transform.rotation.y;
        SetRotationTarget(0);
        int state = startingState;
    }

    private void Update()
    {
        if (state == 0)
        {
            Peaceful();
        }
        if (state == 1)
        {
            Wary();
        }
    }

    void Peaceful()
    {
        transform.rotation = Quaternion.RotateTowards(transform.rotation, _targetRotation, turnSpeed * Time.deltaTime);

        if (transform.rotation == _targetRotation && !rotationFinished) rotationFinished = true;

        if (rotationFinished && coolDownControl > 0f) coolDownControl -= Time.deltaTime;

        if (rotationFinished && !isCooldownFinished && coolDownControl <= 0f)
        {
            isCooldownFinished = true;
            float newIdleAngle = Random.Range(-maxGazingVariation, maxGazingVariation);
            SetRotationTarget(newIdleAngle);
        }

        //Create checksphere.
        //OnTriggerStay cooldown.

    }

    void SetRotationTarget(float newRotAngle)
    {
        rotationFinished = false;
        isCooldownFinished = false;

        coolDown = Random.Range(minCoolDown, maxCooldown);
        coolDownControl = coolDown;

        startingAngle = transform.eulerAngles.y;
        currentAngle = startingAngle;

        _targetRotation = Quaternion.Euler(0, newRotAngle, 0);
    }

    void Wary()
    {
        transform.LookAt(player.transform, player.transform.up);
    }

    void aggressive()
    {

    }

}
