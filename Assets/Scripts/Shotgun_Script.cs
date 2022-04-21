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
    public int pelletNumber;
    public float pelletSpread;
    public float pelletDamage;
    public int range;

    public float shotDelay;
    public float pumpDuration;
    public float reloadShellTime;

    public Image chamberIndicator;

    public bool isFiring = false;
    public bool chamberedBullet = true;
    public bool spentShellChambered = false;
    public bool canFire = true;
    public bool isPumping = false;
    public bool isReloading = false;

    // Object references

    public Camera playerCam;
    public Animator shotgunAnimator;
    public ShakeRecoil_Script camShakeRecoil;
    public AudioController_Script audioInstance;

    [SerializeField] private GameObject _bulletHolePrefab;

    PlayerController_Script playerScript;



    private void Start()
    {
        GameObject PlayerController = GameObject.FindGameObjectWithTag("Player");
        playerScript = PlayerController.GetComponent<PlayerController_Script>();
    }

    private void OnEnable()
    {
        audioInstance.PlaySgReady();
    }

    void Update()
    {
        //Input
        if (Input.GetButtonDown("Fire1") && canFire && !isReloading)
        {
            Fire();
        }

        if (Input.GetButtonUp("Fire1") && !isPumping && !isReloading && !canFire)
        {
            Pump();
        }

        StartCoroutine(Reload());
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
        }

        Invoke("Pump", shotDelay);
    }


    void LiveShot()
    {
        isFiring = true;
        chamberedBullet = false;
        spentShellChambered = true;

        shotgunAnimator.Play("UI_Shotgun_SimpleShot");
        AudioController_Script.audioInstance.PlaySgShoot();

        StartCoroutine(camShakeRecoil.Shaking(.15f, .5f));
        camShakeRecoil.Recoil(recoilX, recoilY, recoilZ);


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
                    Debug.Log("you hit " + mortalObj.name);
                }

            }
        }

        isFiring = false;
    }


    void EmptyShot()
    {
        if (Input.GetButtonDown("Fire1") && !chamberedBullet)                // if chamber is empty
        {
            AudioController_Script.audioInstance.PlayGunEmpty();
        }
    }


    void Pump()
    {
        if (!Input.GetButton("Fire1") && !isPumping)
        {
            isPumping = true;
            //Debug.Log("Pumping!");
            AudioController_Script.audioInstance.PlaySgPumping();
            Invoke("CyclingAction", pumpDuration);
            shotgunAnimator.Play("UI_Shotgun_Pump");
        }
    }


    public void CyclingAction()
    {
        if (spentShellChambered == true)
        {
            //EJECT SHELL HERE +++ Particlesystemmagix4tehwinzors
            spentShellChambered = false;
        }

        if (bulletsInMag >= 1)
        {
            bulletsInMag--;
            chamberedBullet = true;
            chamberIndicator.enabled = true;

        }
        canFire = true;
        isPumping = false;
    }


    IEnumerator Reload()
    {
        if (Input.GetKeyDown(KeyCode.R) && bulletsInMag < magSize && playerScript.shotgunSpareAmmo > 0 && !isReloading && !isPumping)
        {
            isReloading = true;

            while (bulletsInMag < magSize && playerScript.shotgunSpareAmmo > 0)
            {
                shotgunAnimator.Play("UI_Shotgun_Reload");
                AudioController_Script.audioInstance.PlaySgLoadShell();
                yield return new WaitForSeconds(reloadShellTime);
                playerScript.shotgunSpareAmmo--;
                bulletsInMag++;
            }

            if (!chamberedBullet) Pump();
            isReloading = false;
        }
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