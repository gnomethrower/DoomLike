using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Shotgun_Script : MonoBehaviour
{
    public static Shotgun_Script instance;

    public int bulletsInMag;

    //Settings
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


    public bool isShooting = false;
    public bool chamberedBullet = true;
    public bool spentShellChambered = false;
    public bool canFire = true;
    public bool isPumping = false;
    public bool isReloading = false;
    bool isADS = false;

    // Object references
    public Image chamberIndicator;
    public Camera playerCam;
    public Animator animator;
    public ShakeRecoil_Script camShakeRecoil;
    public AudioController_Script audioInstance;
    public AnimationEvent shotgunAnimSounds;
    public ParticleSystem shellParticle;
    public GameObject reticle;
    Image reticleImage;


    [SerializeField] private GameObject _bulletHolePrefab;

    PlayerController_Script playerScript;



    private void Start()
    {
        GameObject PlayerController = GameObject.FindGameObjectWithTag("Player");
        playerScript = PlayerController.GetComponent<PlayerController_Script>();

        GameObject AudioController = GameObject.FindGameObjectWithTag("AudioController");
        audioInstance = AudioController.GetComponent<AudioController_Script>();

        reticle = GameObject.FindWithTag("Reticle");
        reticleImage = reticle.GetComponent<Image>();
    }


    private void OnEnable()
    {
        isADS = false;
        isReloading = false;
        isPumping = false;
        isShooting = false;

        audioInstance.PlaySgReady();

        if (!chamberedBullet && bulletsInMag > 0) Pump();
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

        if (chamberedBullet)                                                           // if a bullet is in the Chamber
        {
            LiveShot();
            chamberIndicator.enabled = false;
            //Debug.Log("Shots fired!");
        }
        else
        {
            EmptyShot();
            Debug.Log("sad click");
        }

        Invoke("Pump", shotDelay);
    }


    void LiveShot()
    {
        isShooting = true;
        chamberedBullet = false;
        spentShellChambered = true;

        animator.SetTrigger("ShotgunShoot");
        audioInstance.PlaySgShoot();

        StartCoroutine(camShakeRecoil.Shaking(.15f, .5f));
        camShakeRecoil.Recoil(recoilX, recoilY, recoilZ, recoilADS);


        LayerMask enemyMask = LayerMask.GetMask("Enemy");
        LayerMask groundMask = LayerMask.GetMask("Ground");

        for (int i = 0; i < pelletNumber; i++)
        {
            RaycastHit hit;
            //Raycast and Decal production
            if (Physics.Raycast(playerCam.transform.position, GetShootingDirection(), out hit, range, enemyMask | groundMask))
            {
                Mortality_Script mortalObj = hit.transform.GetComponent<Mortality_Script>(); // we create a new variable "mortalObj" of the class Mortal, which we define as what the raycasthit "hit" has found.

                GameObject obj = Instantiate(_bulletHolePrefab, hit.point, Quaternion.LookRotation(hit.normal));
                obj.transform.parent = hit.transform;
                obj.transform.position += obj.transform.forward / 1000;

                if (mortalObj != null) // if the mortalObj should not be of type
                {
                    mortalObj.TakeDamage(pelletDamage);
                    //Bodyhitdecals/blood particle system at the hit location.
                    //Debug.Log("you hit " + mortalObj.name);
                }

            }
        }

        isShooting = false;
    }


    void EmptyShot()
    {
        if (Input.GetButtonDown("Fire1") && !chamberedBullet)                // if chamber is empty
        {
            audioInstance.PlayGunEmpty();
        }
    }


    void Pump()
    {
        if (!isPumping && !Input.GetButton("Fire1"))
        {
            isPumping = true;
            //Debug.Log("Pumping!");
            animator.SetTrigger("ShotgunPump");
        }
    }


    IEnumerator Reload()
    {
        if (Input.GetKeyDown(KeyCode.R) && bulletsInMag < magSize && playerScript.shotgunSpareAmmo > 0 && !isReloading && !isPumping)
        {
            isReloading = true;

            while (bulletsInMag < magSize && playerScript.shotgunSpareAmmo > 0)
            {
                animator.SetTrigger("ShotgunReload");
                audioInstance.PlaySgLoadShell();
                yield return new WaitForSeconds(reloadShellTime);
                playerScript.shotgunSpareAmmo--;
                bulletsInMag++;
            }

            Debug.Log("Bullet in chamber after reload = " + chamberedBullet);

            if (chamberedBullet) ReloadFinished();
            if (!chamberedBullet)
            {
                animator.SetTrigger("ShotgunPump");
            }

        }
    }


    void ReloadFinished()
    {
        animator.SetTrigger("ReloadDone");
        Debug.Log("ReloadFinished");
        isReloading = false;
    }


    void ChangeADSMode()
    {
        if (Input.GetButtonDown("Fire2") && !isReloading && !isShooting && !isPumping)
        {
            if (!isADS)
            {
                animator.SetBool("ShotgunAiming", true);
                animator.SetTrigger("ShotgunADS");
                isADS = true;
                recoilADS = adsMultiplier;
                Invoke("ReticleToggle", .2f);
                //Debug.Log("Aiming down Sights is + " + isADS);
            }
            else if (isADS)
            {
                animator.SetBool("ShotgunAiming", false);
                animator.SetTrigger("ShotgunADS");
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
            shellParticle.Emit(1);
            spentShellChambered = false;
        }
    }


    public void CyclingAction() // called in pump animation event
    {

        if (bulletsInMag >= 1)
        {
            bulletsInMag--;
            chamberedBullet = true;
            chamberIndicator.enabled = true;
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



