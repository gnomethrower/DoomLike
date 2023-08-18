using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Shotgun_Script : MonoBehaviour
{


    #region Settings
    public int bulletsInMag;
    public int magSize;

    public float recoilX, recoilY, recoilZ;
    public float adsMultiplier = .5f;
    float recoilADS = 1f;

    public int pelletNumber;
    public float pelletSpread;
    public float pelletDamage;
    public int range;

    public float shotDelay;
    public float pumpDuration;
    public float reloadShellTime;
    #endregion

    #region Control Bools
    public bool isShooting = false;
    public bool chamberedRound = true;
    public bool spentShellChambered = false;
    public bool canFire = true;
    public bool isPumping = false;
    public bool isReloading = false;
    bool isADS = false;
    #endregion

    #region References
    public static Shotgun_Script instance;
    public Image uiShell;
    public Camera playerCam;
    public Animator sgAnimator;
    public Animator sgUIShell;
    public ShakeRecoil_Script camShakeRecoil;
    public AudioController_Script audioInstance;
    public AnimationEvent shotgunAnimSounds;
    public ParticleSystem shellParticle;
    [SerializeField] private GameObject _bloodSplatterPrefab;
    [SerializeField] private GameObject _bulletHolePrefab;
    public GameObject reticle;

    Image reticleImage;
    #endregion



    PlayerController_Script playerScript;
    public LayerMask ground, enemy;

    private void Start()
    {
        GameObject PlayerController = GameObject.FindGameObjectWithTag("Player");
        playerScript = PlayerController.GetComponent<PlayerController_Script>();

        GameObject AudioController = GameObject.FindGameObjectWithTag("AudioController");
        audioInstance = AudioController.GetComponent<AudioController_Script>();

        reticle = GameObject.FindWithTag("Reticle");
        reticleImage = reticle.GetComponent<Image>();

        ground = LayerMask.GetMask("Ground");
        enemy = LayerMask.GetMask("Enemy");
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
        GetInput();
        StartCoroutine(Reload());
    }


    void GetInput()
    {
        if (Input.GetButtonDown("Fire1") && canFire && !isReloading)
        {
            Fire();
        }

        if (Input.GetButtonUp("Fire1") && !isPumping && !isReloading && !canFire)
        {
            Pump();
        }

        ChangeADSMode();

        StartCoroutine("Reload");

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

        StartCoroutine(camShakeRecoil.Shaking(.15f, .5f));
        camShakeRecoil.Recoil(recoilX, recoilY, recoilZ, recoilADS);


        LayerMask enemyMask = LayerMask.GetMask("Enemy");
        LayerMask groundMask = LayerMask.GetMask("Ground");

        for (int i = 0; i < pelletNumber; i++)
        {
            RaycastHit hit;
            //Raycast and Decal production
            if (Physics.Raycast(playerCam.transform.position, GetShootingDirection(), out hit, range, enemy | ground))
            {
                //Debug.DrawLine(playerCam.transform.position, hit.transform.position, Color.red, 1f);
                Mortality_Script mortalObj = hit.transform.GetComponent<Mortality_Script>(); // we create a new variable "mortalObj" of the class Mortal, which we define as what the raycasthit "hit" has found.
                //if (mortalObj != null) Debug.Log(mortalObj.transform.name);

                if (mortalObj != null) // if the mortalObj should not be of type
                {
                    mortalObj.TakeDamage(pelletDamage);
                    //Debug.Log("damage was done of " + pelletDamage + " amount");
                    if (mortalObj.canBleed) { Instantiate(_bloodSplatterPrefab, hit.transform.position, Quaternion.LookRotation(hit.normal)); }
                }

                if (Physics.Raycast(playerCam.transform.position, GetShootingDirection(), out hit, range, ground))
                {
                    GameObject obj = Instantiate(_bulletHolePrefab, hit.point, Quaternion.LookRotation(hit.normal));
                    obj.transform.parent = hit.transform;
                    obj.transform.position += obj.transform.forward / 1000;
                }
            }
        }

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

            while (bulletsInMag < magSize && playerScript.shotgunSpareAmmo > 0)
            {
                sgAnimator.SetTrigger("ShotgunReload");
                audioInstance.PlaySgLoadShell();
                yield return new WaitForSeconds(reloadShellTime);
                playerScript.shotgunSpareAmmo--;
                bulletsInMag++;
            }

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
    }

    void ChangeADSMode()
    {
        if (Input.GetButtonDown("Fire2") && !isReloading && !isShooting && !isPumping)
        {
            if (!isADS)
            {
                sgAnimator.SetBool("ShotgunAiming", true);
                sgAnimator.SetTrigger("ShotgunADS");
                isADS = true;
                recoilADS = adsMultiplier;
                Invoke("ReticleToggle", .2f);
                //Debug.Log("Aiming down Sights is + " + isADS);
            }
            else if (isADS)
            {
                sgAnimator.SetBool("ShotgunAiming", false);
                sgAnimator.SetTrigger("ShotgunADS");
                isADS = false;
                recoilADS = 1f;
                Invoke("ReticleToggle", .05f);
                //Debug.Log("Aiming down Sights is + " + isADS);
            }
        }
    }

    void ReticleToggle()
    {
        if (isADS) reticleImage.enabled = false;
        else reticleImage.enabled = true;
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

    Vector3 GetShootingDirection() //gives us a direction which has a randomized targetposition.
    {
        Vector3 targetPos = playerCam.transform.position + playerCam.transform.forward * range;
        targetPos = new Vector3(
            targetPos.x + UnityEngine.Random.Range(-pelletSpread, pelletSpread),
            targetPos.y + UnityEngine.Random.Range(-pelletSpread, pelletSpread),
            targetPos.z + UnityEngine.Random.Range(-pelletSpread, pelletSpread)
            );
        Vector3 direction = targetPos - playerCam.transform.position;
        return direction.normalized;
    }

}



