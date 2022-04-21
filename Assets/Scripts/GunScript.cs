using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using TMPro;


//+++++++++++++++++++++++++++++++++++


//TODO Reload canceling(by shooting, or select methods) -ADD SOUNDS! ADD PISTOL


//+++++++++++++++++++++++++++++++++++


public class GunScript : MonoBehaviour
{
    public int magSize = 5;
    public int bulletsInMag = 5;
    public int ammoSpare = 25;

    public float damage;
    public float range;
    public float gunCoolDown = 1;
    public float reloadTimePerShell = 0.5f;
    public float reloadDelay = 0f;

    public Animator shotgunAnim;

    private float nextTimeToFire;

    //private bool bulletInChamber;

    public bool readyToShoot = true;
    public bool canReload = true;
    public bool isReloading = false;
    public Camera shootCameraOrigin;

    // Update is called once per frame
    void Update()
    {
        if (Time.time > nextTimeToFire)
        {
            if (Input.GetButtonDown("Fire1"))
            {
                StartCoroutine(Shoot());
                nextTimeToFire = Time.time + gunCoolDown;
                print("Fired! Cooldown Starts!");

            }
        }
        StartCoroutine(ReloadGun());
    }

    IEnumerator Shoot()
    {

        if (bulletsInMag != 0 && readyToShoot == true)
        {
            //Cast a ray from the origin of the camera into a direction, until we either impact something, or if we don't, the range is exhausted.
            bulletsInMag -= 1;
            shotgunAnim.SetTrigger("ShotgunShoot");
            AudioController_Script.audioInstance.PlaySgShoot();


            RaycastHit hit; //the RaycastHit variable gives back info on what the raycast has hit.
            if (Physics.Raycast(shootCameraOrigin.transform.position, shootCameraOrigin.transform.forward, out hit, range)) // IF statement only gets called if it hits anything in range.
            {
                //This gets called if anything has been hit with the ray

                Mortality_Script mortal = hit.transform.GetComponent<Mortality_Script>();
                if (mortal != null)
                {
                    mortal.TakeDamage(damage);
                }
            }
            canReload = false;
            yield return new WaitForSeconds(0.35f);
            AudioController_Script.audioInstance.PlaySgPumping();
            canReload = true;
        }
        else

        {
            if (readyToShoot == true)
            {
                AudioController_Script.audioInstance.PlayGunEmpty();
            }
        }
    }

    IEnumerator ReloadGun()
    {

        //++++++++++++++++++++++++++++ TODO Reload canceling (by shooting, or select methods) - ADD SOUNDS! ADD MORE GUNS
        //if your bulletCount is less than magazine size and you have more than zero spare Bullets, reload.
        if (Input.GetButtonDown("Reload") && ammoSpare != 0 && bulletsInMag < magSize && isReloading == false & canReload == true)
        {
            //play sound Reload initiation
            Debug.Log("Initiating Reload");
            readyToShoot = false;
            isReloading = true;

            while (ammoSpare >= 1 && bulletsInMag < magSize)
            {
                shotgunAnim.SetTrigger("ShotgunReload");
                AudioController_Script.audioInstance.PlaySgLoadShell();
                yield return new WaitForSeconds(reloadTimePerShell);
                ammoSpare--;
                bulletsInMag++;
            }

            readyToShoot = true;
            isReloading = false;

            AudioController_Script.audioInstance.PlaySgPumping();
        }

    }

}
