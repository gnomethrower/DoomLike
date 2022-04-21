using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using TMPro;

//+++++++++++++++++++++++++++++++++++


//TODO Find a way to make the Variables used in UI be pushed by the scripts, so I don't need to update them every frame.


//+++++++++++++++++++++++++++++++++++

public class UIDisplay_Script : MonoBehaviour
{
    public TextMeshProUGUI ammoMagText, ammoSpareText, healthText;
    public GameObject shotgun;
    public GameObject pistol;
    public GameObject player;
    GameObject equippedWeapon;
    PlayerController_Script playerScript;


    // Start is called before the first frame update
    void Start()
    {
        equippedWeapon = pistol;
        player = GameObject.FindGameObjectWithTag("Player");
        playerScript = player.GetComponent<PlayerController_Script>();

    }

    // Update is called once per frame
    void Update()
    {
        healthText.text = PlayerController_Script.currentHealth.ToString();

        //if (WeaponSwitching.selectedWeapon == 1) equippedWeapon = shotgun.GetComponent<sg_Script>();
        //if (WeaponSwitching.selectedWeapon == 0) equippedWeapon = pistol.GetComponent<PistolScript>();

        if (WeaponSwitching_Script.selectedWeapon == 0) //pistol
        {
            ammoMagText.text = pistol.GetComponent<Pistol_Script>().bulletsInMag.ToString();
            ammoSpareText.text = playerScript.pistolSpareAmmo.ToString();
        }
        if (WeaponSwitching_Script.selectedWeapon == 1) //shotgun
        {
            ammoMagText.text = shotgun.GetComponent<Shotgun_Script>().bulletsInMag.ToString();
            ammoSpareText.text = playerScript.shotgunSpareAmmo.ToString();
        }

    }
}
