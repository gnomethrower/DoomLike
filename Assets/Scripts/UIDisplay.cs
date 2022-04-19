using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using TMPro;

//+++++++++++++++++++++++++++++++++++


//TODO Find a way to make the Variables used in UI be pushed by the scripts, so I don't need to update them every frame.


//+++++++++++++++++++++++++++++++++++

public class UIDisplay : MonoBehaviour
{
    public TextMeshProUGUI ammoMagText, ammoSpareText, healthText;
    public GameObject shotgun;
    public GameObject pistol;
    GameObject equippedWeapon;


    // Start is called before the first frame update
    void Start()
    {
        equippedWeapon = pistol;
    }

    // Update is called once per frame
    void Update()
    {
        healthText.text = PlayerController.currentHealth.ToString();

        //if (WeaponSwitching.selectedWeapon == 1) equippedWeapon = shotgun.GetComponent<sg_Script>();
        //if (WeaponSwitching.selectedWeapon == 0) equippedWeapon = pistol.GetComponent<PistolScript>();

        if (WeaponSwitching.selectedWeapon == 0)
        {
            ammoMagText.text = pistol.GetComponent<PistolScript>().bulletsInMag.ToString();
            ammoSpareText.text = pistol.GetComponent<PistolScript>().ammoSpare.ToString();
        }
        if (WeaponSwitching.selectedWeapon == 1)
        {
            ammoMagText.text = shotgun.GetComponent<sg_Script>().bulletsInMag.ToString();
            ammoSpareText.text = shotgun.GetComponent<sg_Script>().ammoSpare.ToString();
        }

    }
}
