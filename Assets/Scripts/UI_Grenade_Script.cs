using BehaviorDesigner.Runtime.Tasks.Unity.UnityAnimator;
using BehaviorDesigner.Runtime.Tasks.Unity.UnityQuaternion;
using BehaviorDesigner.Runtime.Tasks.Unity.UnityRigidbody;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditorInternal.VersionControl.ListControl;

public class UI_Grenade_Script : MonoBehaviour
{
    [SerializeField] private Animator grenadeAnimator;
    [SerializeField] private GameObject liveGrenade;
    [SerializeField] private GameObject grenadeSpoon;
    [SerializeField] private GameObject grenadePin;

    private PlayerController_Script playerControllerScript;
    private GameObject player;
    private AnimationEvent grenadeRelease;
    private GameObject refSphere;
    private Vector3 spoonSpawnpointOffset;
    private Rigidbody liveGrenadeRB;

    [SerializeField] public float fuseTimer;
    [SerializeField] private float throwForce = 5f;
    [SerializeField] private float throwForceMP = 5f;
    [SerializeField] private float torque = 5f;
    [SerializeField] private bool grenadeIsHeld;

    private bool shouldPauseAnim = false;
    private float grenadeCooldownSeconds;
    
    

    private void OnEnable()
    {
        grenadeAnimator.speed = 1f;
    }

    private void Start()
    {
        player = GameObject.Find("Player");
        playerControllerScript = GameObject.Find("Player").GetComponent<PlayerController_Script>();
        grenadeAnimator = GetComponent<Animator>();
        refSphere = GameObject.Find("RefSphere");
    }

    private void Update()
    {
        if (Input.GetButton("Fire1"))
        {
            grenadeIsHeld = true;
        }
        else grenadeIsHeld = false;

        //Starting Animation on click, if grenade is ready.
        if (Input.GetButtonDown("Fire1"))
        {
            grenadeAnimator.Play("UI_Grenade_Throw");
        }

        if (shouldPauseAnim)
        {
            if(grenadeIsHeld)
            {
                grenadeAnimator.speed = 0;
            } 
            else
            {
                shouldPauseAnim = false;
                grenadeAnimator.speed = 1;
            }
        }
    }

    #region Animation event functions
    void CheckForPauseNadeAnim() 
    {
        if (grenadeIsHeld)
        {
            shouldPauseAnim = true;
        }
    }

    void InstantiateThrownGrenade()
    {
        if (playerControllerScript.grenadesSpare > 0)
        {
            playerControllerScript.grenadesSpare -= 1;
            GameObject thrownNade = Instantiate(liveGrenade, refSphere.transform.position + (2f * refSphere.transform.forward), Quaternion.identity);
            thrownNade.GetComponent<Rigidbody>().AddForce(refSphere.transform.forward * throwForce);
            thrownNade.GetComponent<Rigidbody>().AddTorque(refSphere.transform.forward * torque);
            
            GameObject discardedPin = Instantiate(grenadePin, player.transform.position + (1f * player.transform.forward), Quaternion.FromToRotation(player.transform.position, player.transform.forward));

            spoonSpawnpointOffset = refSphere.transform.position + (.35f * refSphere.transform.right) + (.15f * refSphere.transform.up) + (1.5f * refSphere.transform.forward);

            GameObject discardedSpoon = Instantiate(grenadeSpoon, spoonSpawnpointOffset, Quaternion.FromToRotation(refSphere.transform.position, refSphere.transform.forward));
            discardedSpoon.GetComponent<Rigidbody>().AddForce(refSphere.transform.right * throwForce * throwForceMP);
            discardedSpoon.GetComponent<Rigidbody>().AddTorque(refSphere.transform.right * torque);
        }
    }
    #endregion
}
