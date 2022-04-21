using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController_Script : MonoBehaviour
{
    public static AudioController_Script audioInstance;
    public AudioSource ammoPickup, bloodBagPickup, sgShoot, sgLoadShell, sgPumping, sgReady, gunEmpty, pistolShoot, pistolSlideRelease, pistolRackSlide, pistolMagEject, pistolMagInsert;

    // Start is called before the first frame update
    void Start()
    {
        audioInstance = this;
    }

    // Update is called once per frame
    void Update()
    {

    }
    //++++++++++++++++++++Pickups
    public void PlayAmmoPickup()
    {
        ammoPickup.Stop();
        ammoPickup.Play();
    }
    public void PlayBloodBagPickup()
    {
        bloodBagPickup.Stop();
        bloodBagPickup.Play();
    }

    //++++++++++++++++++++Player

    //++++++++++++++++++++ GUNS

    // +++++++++++++++++ Shotgun
    public void PlaySgReady()
    {
        sgReady.Stop();
        sgReady.Play();
    }
    public void PlaySgShoot()
    {
        sgShoot.Stop();
        sgShoot.Play();
    }
    public void PlaySgLoadShell()
    {
        sgLoadShell.Stop();
        sgLoadShell.Play();
    }
    public void PlaySgPumping()
    {
        sgPumping.Stop();
        sgPumping.Play();
    }

    public void PlayGunEmpty()
    {
        gunEmpty.Stop();
        gunEmpty.Play();
    }

    // ++++++++++++++ Pistol

    public void PlayPistolShoot()
    {
        pistolShoot.Stop();
        pistolShoot.Play();
    }

    public void PlayPistolSlideRelease()
    {
        pistolSlideRelease.Stop();
        pistolSlideRelease.Play();
    }

    public void PlayPistolRackSlide()
    {
        pistolRackSlide.Stop();
        pistolRackSlide.Play();
    }

    public void PlayPistolMagEject()
    {
        pistolMagEject.Stop();
        pistolMagEject.Play();
    }
    public void PlayPistolMagInsert()
    {
        pistolMagInsert.Stop();
        pistolMagInsert.Play();
    }

    public void PlayPistolFullReload()
    {
        pistolMagEject.PlayDelayed(.2f);
        pistolMagInsert.PlayDelayed(.4f);
        pistolSlideRelease.PlayDelayed(1.35f);

    }

    public void PlayPistolPartialReload()
    {
        pistolMagEject.PlayDelayed(.2f);
        pistolMagInsert.PlayDelayed(.4f);
    }

}
