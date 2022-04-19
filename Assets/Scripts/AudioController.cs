using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour
{
    public static AudioController audioInstance;
    public AudioSource ammoPickup, bloodBagPickup, sgShoot, sgLoadShell, sgPumping, sgReady, gunEmpty, pistolShoot;

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
    // Shotgun
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

    public void PlayPistolShoot()
    {
        pistolShoot.Stop();
        pistolShoot.Play();
    }
}
