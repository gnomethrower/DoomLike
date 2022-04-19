using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PistolScript : MonoBehaviour
{

    //Gun stat variables
    public int magSize = 15;
    public int bulletsInMag = 15;
    public int ammoSpare = 60;
    public int maxSpareAmmo = 60;


    public int bulletDamage;
    public float shotCooldown, timeBetweenShots, spread, spreadMultiplierMoving, range, reloadTime;
    public int bulletsPerTap;


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
    public AudioController audioInstance;
    public Animator animator;

    [SerializeField] private GameObject _bulletHolePrefab;


    private void Update()
    {
        GetInput();
    }

    private void Awake()
    {
        chamberedRound = true;
    }

    void GetInput()
    {
        if (!rapidFire) isShooting = Input.GetButtonDown("Fire1");
        if (rapidFire) isShooting = Input.GetButton("Fire1");

        if (chamberedRound && isShooting && !isReloading)
        {
            AudioController.audioInstance.PlayPistolShoot();
            Shoot();
        }

        if (Input.GetButtonDown("Reload") && ammoSpare > 0 && bulletsInMag < magSize && !isShooting)
        {
            Debug.Log("Reload pressed!");
            animator.SetTrigger("PistolReloading");
            Invoke("Reload", reloadTime);
        }
    }

    void Shoot()
    {
        chamberedRound = false;
        
        //spread
        
        float x = Random.Range(-spread, spread) * PlayerController.spreadMultiplier;
        float y = Random.Range(-spread, spread) * PlayerController.spreadMultiplier;

        EjectCasing();
        //Debug.Log(bulletsInMag + " shots left!");

        animator.SetTrigger("PistolShot");

        for (int i = 0; i < bulletsPerTap; i++)
        {
            Debug.Log("RAYCAST");
            RaycastHit hit;
            //Raycast and Decal production
            if (Physics.Raycast(playerCam.transform.position, playerCam.transform.forward, out hit, range, target | ground))
            {
                Debug.Log("You hit " + hit.transform.name);
                Mortal mortalObj = hit.transform.GetComponent<Mortal>(); // we create a new variable "mortalObj" of the class Mortal, which we define as what the raycasthit "hit" has found.

                GameObject obj = Instantiate(_bulletHolePrefab, hit.point, Quaternion.LookRotation(hit.normal));
                obj.transform.parent = hit.transform;
                obj.transform.position += obj.transform.forward / 1000;

                if (mortalObj != null) // if the mortalObj should not be of type
                {
                    mortalObj.TakeDamage(bulletDamage);
                    //Bodyhitdecals/blood particle system at the hit location.
                    Debug.Log("you hit " + mortalObj.name);
                }

            }
        }

        //play sound;
        Invoke("ChamberRound", shotCooldown);
    }

    void ChamberRound()
    {
        if (bulletsInMag > 0)
        {
            bulletsInMag--;
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
        if (ammoSpare > 0)
        {
            if (ammoSpare < magSize)
            {
                Debug.Log("Ammo left is smaller than MagSize");
                bulletsInMag = ammoSpare;
                ammoSpare = 0;
            }

            if (ammoSpare >= magSize)
            {
                Debug.Log("Enough ammo for a full mag reload");
                int bulletsLeftMag = magSize - bulletsInMag;
                ammoSpare = ammoSpare - bulletsLeftMag;
                bulletsInMag = magSize;
            }
            if(!chamberedRound) Invoke("ChamberRound", 1f);
        }
    }

}
