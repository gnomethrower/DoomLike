using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController_Script : MonoBehaviour
{
    public AudioController_Script audioInstance;

    [Header("Player")]
    public AudioSource playerDeath;
    public AudioSource playerHurt;
    public AudioSource playerHurtBad;
    public AudioSource bloodBagPickup;
    public AudioSource footStep;

    [Header("PistolSounds")]
    public AudioSource pistolShoot;
    public AudioSource pistolSlideRelease;
    public AudioSource pistolRackSlide;
    public AudioSource pistolMagEject;
    public AudioSource pistolMagInsert;

    [Header("ShotgunSounds")]
    public AudioSource sgShoot;
    public AudioSource sgLoadShell;
    public AudioSource sgPumping;
    public AudioSource sgReady;

    [Header("AmmoSounds")]
    public AudioSource ammoPickup;
    public AudioSource gunEmpty;
    public AudioSource shellCollision;
    public AudioSource casingCollision;

    [Header("Enemies")]
    public AudioSource enemyAggro;
    public AudioSource enemyMelee;
    public AudioSource enemyWary;
    public AudioSource enemyDeath;
    public AudioSource enemyPeace;

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

    public void PlayShellCollision()
    {
        shellCollision.Stop();
        shellCollision.Play();
    }

    public void PlayCasingCollision()
    {
        casingCollision.Stop();
        casingCollision.Play();
    }

    public void PlayEnemyMelee()
    {
        enemyMelee.Stop();
        enemyMelee.Play();
    }

    public void PlayPlayerDeath()
    {
        playerDeath.Stop();
        playerDeath.Play();
    }

    public void PlayEnemyAggro()
    {
        enemyAggro.Stop();
        enemyAggro.Play();
    }

    public void PlayEnemyPeace()
    {
        enemyPeace.Stop();
        enemyPeace.Play();
    }

    public void PlayEnemyWary()
    {
        enemyWary.Stop();
        enemyWary.Play();
    }

    public void PlayEnemyDeath()
    {
        enemyDeath.Stop();
        enemyDeath.Play();
    }

    public void PlayFootStep()
    {
        footStep.Stop();
        footStep.Play();
    }

    public void PlayPlayerHurt()
    {
        playerHurt.Stop();
        playerHurt.Play();
    }
    public void PlayPlayerHurtBad()
    {
        playerHurtBad.Stop();
        playerHurtBad.Play();
    }
}
