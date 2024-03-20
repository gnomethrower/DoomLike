using BehaviorDesigner.Runtime.Tasks.Unity.UnityGameObject;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Shotgun_Script : MonoBehaviour
{
    //Lambda bools
    private bool PullTrigger => UnityEngine.Input.GetButtonDown("Fire1");
    private bool ReleaseTrigger => UnityEngine.Input.GetButtonUp("Fire1");

    [Header("Ammo")]
    public int bulletsInMag;
    public int magSize;

    [Header("Recoil")]
    public float recoilX, recoilY, recoilZ;
    public float adsMultiplier = .5f;
    float recoilADS = 1f;

    [Header("Damage, Spread and Projectiles")]
    public int pelletNumber;
    public float pelletSpread;
    public float pelletDamage;
    public int range;

    [Header("Timing")]
    public float shotDelay;
    public float pumpDuration;
    public float reloadShellTime;

    [Header("Checkbools")]
    public bool isShooting = false;
    public bool chamberedRound = true;
    public bool spentShellChambered = false;
    public bool canFire = true;
    public bool isPumping = false;
    public bool isReloading = false;
    [SerializeField] private bool interruptReload;

    bool isADS = false;

    [Header("Script References")]
    public static Shotgun_Script instance;
    public ShakeRecoil_Script camShakeRecoil;
    public AudioController_Script audioInstance;
    PlayerController_Script playerScript;

    [Header("Object References")]
    public Image uiShell;
    public Camera playerCam;
    public Animator sgAnimator;
    public Animator sgUIShell;
    public AnimationEvent shotgunAnimSounds;
    public ParticleSystem shellParticle;
    [SerializeField] private GameObject _bloodSplatterPrefab;
    [SerializeField] private GameObject _bulletHolePrefab;
    public GameObject reticle;
    Image reticleImage;
    public Transform gunMuzzle;
    private GameObject muzzleFlashPositionObject;
    private Vector3 muzzleSmokePosADS;
    private Vector3 muzzleSmokePosHipfire;
    private Vector3 muzzleFlashPositionADS;
    private Vector3 muzzleFlashPosHipfire;
    private GameObject muzzleLightObject;
    private ParticleSystem muzzleLightParticleSystem;
    private GameObject muzzleSmokeObject;
    private ParticleSystem muzzleSmokeParticleSystem;
    public LayerMask ground, enemy;

    private void Awake()
    {
        gunMuzzle = GameObject.Find("GunMuzzle").GetComponent<Transform>();
        GameObject PlayerController = GameObject.FindGameObjectWithTag("Player");
        playerScript = PlayerController.GetComponent<PlayerController_Script>();

        GameObject AudioController = GameObject.FindGameObjectWithTag("AudioController");
        audioInstance = AudioController.GetComponent<AudioController_Script>();

        reticle = GameObject.FindWithTag("Reticle");
        reticleImage = reticle.GetComponent<Image>();

        ground = LayerMask.GetMask("Ground");
        enemy = LayerMask.GetMask("Enemy");

        InitializeMuzzleEffects();
    }

    private void Start()
    {

    }

    private void OnEnable()
    {
        isADS = false;
        isReloading = false;
        isPumping = false;
        isShooting = false;

        audioInstance.PlaySgReady();

        uiShell.enabled = true;

        if (chamberedRound) sgUIShell.SetBool("FreshShellChambered", true);
        else sgUIShell.SetBool("FreshShellChambered", false);

        if (!chamberedRound && bulletsInMag > 0) Pump();
    }

    void Update()
    {
        #region debug msg

        #endregion

        GetInput();
        StartCoroutine(Reload());
    }

    void GetInput()
    {
        if (PullTrigger && canFire && !isReloading)
        {
            Fire();
        }

        if (PullTrigger && isReloading)
        {
            InterruptReload();
        }

        if (ReleaseTrigger && !isPumping && !isReloading && !canFire)
        {
            Pump();
        }

        ChangeADSMode();

        StartCoroutine("Reload");

    }

    void InterruptReload()
    {
        interruptReload = true;
    }

    public void Fire()
    {
        canFire = false;

        if (chamberedRound)                                                           // if a bullet is in the Chamber
        {
            LiveShot();
            sgUIShell.SetTrigger("Shoot");
            //Debug.Log("Shots fired!");
        }
        else
        {
            EmptyShot();
        }

        Invoke("Pump", shotDelay);
    }

    void LiveShot()
    {
        isShooting = true;
        chamberedRound = false;
        spentShellChambered = true;

        sgUIShell.SetBool("FreshShellChambered", false);
        sgUIShell.SetBool("SpentShellChambered", true);
        sgAnimator.SetTrigger("ShotgunShoot");
        audioInstance.PlaySgShoot();



        LayerMask enemyMask = LayerMask.GetMask("Enemy");
        LayerMask groundMask = LayerMask.GetMask("Ground");

        for (int i = 0; i < pelletNumber; i++)
        {
            RaycastHit hit;
            if (Physics.Raycast(gunMuzzle.transform.position, GetShootingDirection(), out hit, range, enemy | ground))
            {
                Debug.DrawRay(gunMuzzle.transform.position, hit.point, Color.red);
                Mortality_Script mortalObj = hit.transform.GetComponent<Mortality_Script>(); // we create a new variable "mortalObj" of the class Mortal, which we define as what the raycasthit "hit" has found.

                if (mortalObj != null)
                {
                    //Debug.Log(mortalObj.name);
                    mortalObj.TakeDamage(pelletDamage);
                    int bleedingChance = UnityEngine.Random.Range(1, 100);

                    if (mortalObj.canBleed && bleedingChance > 50)
                    {
                        Instantiate(_bloodSplatterPrefab, hit.transform.position, Quaternion.LookRotation(hit.normal));
                    }
                }


                if (Physics.Raycast(gunMuzzle.transform.position, GetShootingDirection(), out hit, range, ground))
                {
                    GameObject obj = Instantiate(_bulletHolePrefab, hit.point, Quaternion.LookRotation(hit.normal));
                    obj.transform.parent = hit.transform;
                    obj.transform.position += obj.transform.forward / 1000;
                }
            }

             //Debug.Log("Couldn't hit stuff!");

        }

        PlayMuzzleFlash();
        StartCoroutine(camShakeRecoil.Shaking(.15f, .25f));
        camShakeRecoil.Recoil(recoilX, recoilY, recoilZ, recoilADS);
        isShooting = false;
    }

    void EmptyShot()
    {
        //Debug.Log("sad empty gun noises");
        audioInstance.PlayGunEmpty();
    }

    void Pump()
    {
        if (!isPumping && !Input.GetButton("Fire1"))
        {
            isPumping = true;
            //Debug.Log("Pumping!");
            sgAnimator.SetTrigger("ShotgunPump");
        }
    }

    IEnumerator Reload()
    {
        if (Input.GetKeyDown(KeyCode.R) && bulletsInMag < magSize && playerScript.shotgunSpareAmmo > 0 && !isReloading && !isPumping)
        {
            isReloading = true;
            while (bulletsInMag < magSize && playerScript.shotgunSpareAmmo > 0 && !interruptReload)
            {
                sgAnimator.SetTrigger("ShotgunReload");
                audioInstance.PlaySgLoadShell();
                yield return new WaitForSeconds(reloadShellTime);
                playerScript.shotgunSpareAmmo--;
                bulletsInMag++;
            }
            if (interruptReload) ReloadFinished();
            if (chamberedRound) ReloadFinished();
            if (!chamberedRound)
            {
                sgAnimator.SetTrigger("ShotgunPump");
            }

        }
    }

    void ReloadFinished()
    {
        sgAnimator.SetTrigger("ReloadDone");
        isReloading = false;
        interruptReload = false;
    }

    void ChangeADSMode()
    {
        if (Input.GetButtonDown("Fire2") && !isReloading && !isShooting && !isPumping)
        {
            if (!isADS) // going into ADS
            {
                sgAnimator.SetBool("ShotgunAiming", true);
                sgAnimator.SetTrigger("ShotgunADS");
                isADS = true;
                recoilADS = adsMultiplier;
                Invoke("ReticleToggle", .2f);
                ToggleMuzzlePosition();
            }
            else if (isADS) // going into Hipfire
            {
                sgAnimator.SetBool("ShotgunAiming", false);
                sgAnimator.SetTrigger("ShotgunADS");
                isADS = false;
                recoilADS = 1f;
                Invoke("ReticleToggle", .05f);
                ToggleMuzzlePosition();
            }
        }
    }

    void ReticleToggle()
    {
        //if (isADS) reticleImage.enabled = false;
        //else reticleImage.enabled = true;
    }

    public void ShellEmit() // called in pump animation event
    {
        if (spentShellChambered == true)
        {
            sgUIShell.SetTrigger("EjectSpentShell");
            sgUIShell.SetBool("SpentShellChambered", false);
            shellParticle.Emit(1);
            spentShellChambered = false;
        }
    }

    public void CyclingAction() // called in pump animation event
    {

        if (bulletsInMag >= 1)
        {
            bulletsInMag--;
            sgUIShell.SetTrigger("ChambFreshRound");
            sgUIShell.SetBool("FreshShellChambered", true);
            chamberedRound = true;
            //shellIndicator.enabled = true;
        }

        if (isReloading) ReloadFinished();
        isPumping = false;
        canFire = true;
    }

    void AudioPumpSound() // called in pump animation event
    {
        audioInstance.PlaySgPumping();
    }

    private void PlayMuzzleFlash()
    {

        muzzleLightParticleSystem.Play();
        muzzleSmokeParticleSystem.Play();

    }

    private void InitializeMuzzleEffects()
    {
        muzzleLightObject = GameObject.Find("muzzleLight");
        muzzleLightParticleSystem = muzzleLightObject.GetComponent<ParticleSystem>();

        muzzleSmokeObject = GameObject.Find("muzzleSmoke");
        muzzleSmokeParticleSystem = muzzleSmokeObject.GetComponent<ParticleSystem>();

        muzzleFlashPositionObject = GameObject.Find("MuzzlePlacement");
    }

    void ToggleMuzzlePosition()
    {
        if (!isADS)
        {
            muzzleSmokeObject.transform.localPosition = muzzleSmokePosADS;
            muzzleLightObject.transform.localPosition = muzzleFlashPositionADS;
        }
        else if (isADS)
        {
            muzzleSmokeObject.transform.localPosition = muzzleSmokePosHipfire;
            muzzleLightObject.transform.localPosition = muzzleFlashPosHipfire;
        }
    }

    Vector3 GetShootingDirection() //gives us a direction which has a randomized targetposition.
    {
        Vector3 targetPos = gunMuzzle.transform.position + gunMuzzle.transform.forward * range;
        targetPos = new Vector3(
            targetPos.x + UnityEngine.Random.Range(-pelletSpread, pelletSpread),
            targetPos.y + UnityEngine.Random.Range(-pelletSpread, pelletSpread),
            targetPos.z + UnityEngine.Random.Range(-pelletSpread, pelletSpread)
            );
        Vector3 direction = targetPos - gunMuzzle.transform.position;
        Debug.DrawRay(gunMuzzle.transform.position, direction, Color.yellow, 5f, false);
        return direction.normalized;
    }

    private void OnToggleShotgunADS()
    {

    }

}



