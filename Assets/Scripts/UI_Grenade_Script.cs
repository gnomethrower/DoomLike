using BehaviorDesigner.Runtime.Tasks.Unity.UnityAnimator;
using BehaviorDesigner.Runtime.Tasks.Unity.UnityRigidbody;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditorInternal.VersionControl.ListControl;

public class UI_Grenade_Script : MonoBehaviour
{

    /**
     * 0. On weaponselect, play equipanim.
     * 1. When player left clicks and there are still grenades in inventory, start grenade throw animation.
     * 2. The grenade throw animation will call the animation event that spawns a grenade with a preset velocity.
     * 3. if no grenades remain in inventory, switch to the previously equipped weapon.
     * **/

    [SerializeField] private Animator grenadeAnimator;
    [SerializeField] private GameObject liveGrenade;
    private PlayerController_Script playerControllerScript;
    private AnimationEvent grenadeRelease;
    private GameObject refSphere;
    private Rigidbody liveGrenadeRB;

    [SerializeField] public float fuseTimer;
    [SerializeField] private float throwForce = 5f;
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
        }
    }
    #endregion
}

// Old Code
//private bool FireButtonPressed => UnityEngine.Input.GetButtonDown("Fire1");
//private bool FireButtonHeld => UnityEngine.Input.GetButton("Fire1");
//private bool FireButtonReleased => UnityEngine.Input.GetButtonUp("Fire1");


//[SerializeField] private Animator grenadeAnimator;
//private AnimationEvent grenadeRelease;
//[SerializeField] private GameObject liveGrenade;
//[SerializeField] bool releaseGrenade = false;

//private GameObject refSphere;
//private Rigidbody liveGrenadeRB;

////[SerializeField] private bool grenadeReady = true;
//[SerializeField] private float throwForce = 5f;
//[SerializeField] private float torque = 5f;

//public float fuseTimer;
//private float grenadeCooldownSeconds;

//private int state = 0; // 0 = Ready to throw grenade; 1 = Playing Anim; 2 = Pausing Animation; 3 = throwing grenade;
//private int lastState;
//private int currentState;

//private void Awake()
//{
//    grenadeAnimator = this.GetComponent<Animator>();
//    refSphere = GameObject.Find("RefSphere");
//    Debug.Log("Awake is called");
//}

//private void Update()
//{
//        Debug.Log(state);
//        StateMachine();
//}

//void StateMachine()
//{
//    if(state == 0)
//    {
//        Debug.Log("Ready State");
//        if (FireButtonPressed)
//        {
//            SetState(1);
//        }
//    }

//    if(state == 1) // Start Throw Anim State
//    {
//        Debug.Log("throwing");
//    }

//    if(state == 2) // HoldState
//    {
//        Debug.Log("Holding Grenade");
//        grenadeAnimator.speed = 0;

//        if (!grenadeAnimator.GetBool("isHeld"))
//        {
//            SetState(3);
//        }
//    }

//    if(state == 3) // Release State
//    {

//    }
//}

//void HoldNadeState() // state 2
//{

//}

//void ReleaseNadeState() // state 3
//{
//    if (grenadeAnimator.speed == 0) grenadeAnimator.speed = 1;
//}
//void SetState(int nextState)
//{
//    Debug.Log(nextState);
//    if (nextState == currentState)
//    {
//        return;
//    }

//    ExitState(currentState);
//    lastState = currentState;
//    currentState = nextState;

//    InitializingNextState(currentState);
//}
//void InitializingNextState(int initState)
//{
//    switch (initState)
//    {
//        case 0:
//            break;
//        case 1:
//            break;
//        case 2:
//            break;
//        case 3:
//            break;
//        default:
//            Debug.LogWarning("Unexpected state: " + initState);
//            break;
//    }
//}
//void ExitState(int lastState)
//{
//    switch (lastState)
//    {
//        case 0:
//            break;
//        case 1:
//            break;
//        case 2:
//            break;
//        case 3:
//            break;
//        default:
//            Debug.LogWarning("Unexpected state: " + lastState);
//            break;
//    }
//}
