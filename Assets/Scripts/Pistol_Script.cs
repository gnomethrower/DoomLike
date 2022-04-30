using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Pistol_Script : MonoBehaviour
{

    //Gun stat variables
    public int magSize;
    public int bulletsInMag;

    public float recoilX, recoilY, recoilZ;
    public float adsMultiplier = .5f;
    float recoilADS = 1f;

    public int bulletDamage;
    public float shotCooldown, timeBetweenShots, range, reloadTime;
    public int bulletsPerTap = 1;


    public bool rapidFire;

    // control bools
    public bool isShooting, isReloading, canReload, chamberedRound, isMoving, isADS, isSwitchingFireMode;
    float movingX;
    float movingZ;

    //referencing Objects
    public Camera playerCam;
    public Transform attackPoint;
    public RaycastHit hit;
    public LayerMask target;
    public LayerMask ground;
    public AudioController_Script audioInstance;
    public Animator animator;
    public ShakeRecoil_Script camShakeRecoil;
    public ParticleSystem shellParticle;
    public Image chamberIndicator;

    GameObject reticle;
    Image reticleImage;

    [SerializeField] private GameObject _bulletHolePrefab;

    PlayerController_Script playerScript;



    private void Start()
    {
        chamberedRound = true;

        GameObject PlayerController = GameObject.FindGameObjectWithTag("Player");
        playerScript = PlayerController.GetComponent<PlayerController_Script>();

        GameObject AudioController = GameObject.FindGameObjectWithTag("AudioController");
        audioInstance = AudioController.GetComponent<AudioController_Script>();

        reticle = GameObject.FindWithTag("Reticle");
        reticleImage = reticle.GetComponent<Image>();
    }

    private void OnEnable()
    {
        isShooting = false;
        isReloading = false;
        isADS = false;
        isSwitchingFireMode = false;

        audioInstance.PlayPistolRackSlide();

        if (!chamberedRound) ChamberRound();
    }

    private void Update()
    {
        GetInput();
    }

    void GetInput()
    {
        //if (Input.GetButtonDown("Fire1")) Debug.Log("Clicked Mouse1");
        //if (Input.GetButton("Fire1")) Debug.Log("Holding Mouse1");

        if (!rapidFire) isShooting = Input.GetButtonDown("Fire1");
        if (rapidFire) isShooting = Input.GetButton("Fire1");

        if (chamberedRound && isShooting && !isReloading && !isSwitchingFireMode)
        {
            Shoot();
        }

        if (Input.GetButtonDown("Reload") && playerScript.pistolSpareAmmo > 0 && bulletsInMag < magSize && !isShooting && !isSwitchingFireMode)
        {
            Reload();
        }

        if (Input.GetKeyDown(KeyCode.B) && !isShooting && !isReloading && !isSwitchingFireMode)
        {
            ToggleFireMode();
        }

        if (Input.GetButtonDown("Fire2") && !isShooting && !isReloading && !isSwitchingFireMode)
        {
            ChangeADSMode();
        }

    }

    void ToggleFireMode()
    {
        isSwitchingFireMode = !isSwitchingFireMode;
        animator.SetTrigger("PistolFireMode");
        audioInstance.PlayGunEmpty();
        if (rapidFire) rapidFire = false;
        else rapidFire = true;
        isSwitchingFireMode = !isSwitchingFireMode;
    }

    void Shoot()
    {
        chamberedRound = false;
        chamberIndicator.enabled = false;

        EjectCasing();

        audioInstance.PlayPistolShoot();

        animator.SetTrigger("PistolShot");
        camShakeRecoil.Recoil(recoilX, recoilY, recoilZ, recoilADS);
        for (int i = 0; bulletsPerTap > i; i++)
        {

            //Raycast and Decal production
            if (Physics.Raycast(playerCam.transform.position, playerCam.transform.forward, out RaycastHit hit, range, target | ground))
            {

                //Debug.Log("You hit " + hit.transform.name);

                Mortality_Script mortalObj = hit.transform.GetComponent<Mortality_Script>(); // we create a new variable "mortalObj" of the class Mortal, which we define as what the raycasthit "hit" has found.

                GameObject obj = Instantiate(_bulletHolePrefab, hit.point, Quaternion.LookRotation(hit.normal));

                obj.transform.parent = hit.transform;
                obj.transform.position += obj.transform.forward / 1000;

                if (mortalObj != null) // if the mortalObj should not be of type
                {
                    mortalObj.TakeDamage(bulletDamage);
                    //Bodyhitdecals/blood particle system at the hit location.
                    //Debug.Log("you hit " + mortalObj.name);
                }

            }
        }

        Invoke("ChamberRound", shotCooldown);
    }

    void ChamberRound()
    {
        if (bulletsInMag > 0)
        {
            bulletsInMag -= bulletsPerTap;
            chamberedRound = true;
            chamberIndicator.enabled = true;
        }
        //else //Set the Idle animation to emptyGun Idle, if available.
    }

    void EjectCasing()
    {
        shellParticle.Emit(1);
    }

    void Reload() // +++++++++++++++++++++++++ GOT TO MAKE IT SO THE SPARE AMMO IS ALWAYS CORRECTLY SUBTRACTED, ACCORDING TO MAG SIZE AND FILL.+++++++++++++++++++++++++++++++++++
    {
        isReloading = true;
        animator.SetTrigger("PistolReloading");
        audioInstance.PlayPistolFullReload();
        Invoke("ReloadAmmoRefresh", reloadTime);
    }

    void ReloadAmmoRefresh()
    {

        int bulletsMissing = magSize - bulletsInMag;
        int bulletsToReload = Mathf.Min(playerScript.pistolSpareAmmo, bulletsMissing);

        bulletsInMag += bulletsToReload;
        playerScript.pistolSpareAmmo -= bulletsToReload;

        if (!chamberedRound)
        {
            ChamberRound();
        }

        isReloading = false;
    }

    void ChangeADSMode()
    {
        if (!isADS)
        {
            ADSon();
        }
        else if (isADS)
        {
            ADSoff();
        }
    }

    void ADSon()
    {
        animator.SetBool("PistolADS", true);
        animator.SetTrigger("PistolAiming");
        isADS = true;
        recoilADS = adsMultiplier;
        Invoke("ReticleToggle", .2f);
        Debug.Log("Aiming down Sights is + " + isADS);
    }

    void ADSoff()
    {
        animator.SetBool("PistolADS", false);
        animator.SetTrigger("PistolAiming");
        isADS = false;
        recoilADS = 1f;
        Invoke("ReticleToggle", .05f);
        Debug.Log("Aiming down Sights is + " + isADS);
    }


    void ReticleToggle()
    {
        if (isADS) reticleImage.enabled = false;
        else reticleImage.enabled = true;
    }

    public void ToggleIsSwitchingFireMode() //only called in Firemode Toggle Anim Event.
    {
        isSwitchingFireMode = !isSwitchingFireMode;
    }

    public void PistolIdleAnimation()
    {
        animator.Play("UI_Pistol_Idle");
    }

}
