using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pistol_Script : MonoBehaviour
{

    //Gun stat variables
    public int magSize;
    public int bulletsInMag;

    public float recoilX, recoilY, recoilZ;

    public int bulletDamage;
    public float shotCooldown, timeBetweenShots, spread, spreadMultiplierMoving, range, reloadTime;
    public int bulletsPerTap = 1;


    public bool rapidFire;

    // control bools
    bool isShooting, isReloading, canReload, chamberedRound, isMoving;
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


    [SerializeField] private GameObject _bulletHolePrefab;

    PlayerController_Script playerScript;



    private void Start()
    {
        chamberedRound = true;

        GameObject PlayerController = GameObject.FindGameObjectWithTag("Player");
        playerScript = PlayerController.GetComponent<PlayerController_Script>();

        GameObject AudioController = GameObject.FindGameObjectWithTag("AudioController");
        audioInstance = AudioController.GetComponent<AudioController_Script>();
    }

    private void OnEnable()
    {
        audioInstance.PlayPistolRackSlide();
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

        if (chamberedRound && isShooting && !isReloading)
        {
            audioInstance.PlayPistolShoot();
            Shoot();
        }

        if (Input.GetButtonDown("Reload") && playerScript.pistolSpareAmmo > 0 && bulletsInMag < magSize && !isShooting)
        {
            animator.SetTrigger("PistolReloading");

            audioInstance.PlayPistolFullReload();

            Invoke("Reload", reloadTime);
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            animator.SetTrigger("PistolFireMode");
            Debug.Log("fire mode switched");
            audioInstance.PlayGunEmpty();
            if (rapidFire) rapidFire = false;
            else rapidFire = true;
        }


    }

    void Shoot()
    {
        chamberedRound = false;

        //spread
        float x = Random.Range(-spread, spread) * PlayerController_Script.spreadMultiplier;
        float y = Random.Range(-spread, spread) * PlayerController_Script.spreadMultiplier;

        //EjectCasing();

        animator.SetTrigger("PistolShot");
        camShakeRecoil.Recoil(recoilX, recoilY, recoilZ);
        shellParticle.Emit(1);

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
        }
        //else //Set the Idle animation to emptyGun Idle, if available.
    }

    void EjectCasing()
    {
        //eject a casing prefab
    }

    void Reload() // +++++++++++++++++++++++++ GOT TO MAKE IT SO THE SPARE AMMO IS ALWAYS CORRECTLY SUBTRACTED, ACCORDING TO MAG SIZE AND FILL.+++++++++++++++++++++++++++++++++++
    {
        if (playerScript.pistolSpareAmmo > 0)
        {
            if (playerScript.pistolSpareAmmo < magSize)
            {
                //Debug.Log("Ammo left is smaller than MagSize");
                bulletsInMag = playerScript.pistolSpareAmmo;
                playerScript.pistolSpareAmmo = 0;
            }

            if (playerScript.pistolSpareAmmo >= magSize)
            {
                //Debug.Log("Enough ammo for a full mag reload");
                int bulletsLeftMag = magSize - bulletsInMag;
                playerScript.pistolSpareAmmo = playerScript.pistolSpareAmmo - bulletsLeftMag;
                bulletsInMag = magSize;
            }


        }

        if (!chamberedRound)
        {
            ChamberRound();
        }
    }

}
