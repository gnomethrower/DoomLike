using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponSwitching_Script : MonoBehaviour
{

    public int selectedWeapon;
    //[SerializeField] private int lastWeaponSelected;
    string weaponEnum;

    public static GameObject shotgunInit;
    GameObject player;
    PlayerController_Script playerScript;
    public Image uiShell;
    public Image uiBullet;
    public Image uiGrenade;



    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerScript = player.GetComponent<PlayerController_Script>();
        weaponEnum = playerScript.startingGun.ToString();

        StartingGun();
        shotgunInit = GameObject.FindGameObjectWithTag("Shotgun");
        SelectWeapon();
    }

    // Update is called once per frame
    void Update()
    {
        GetInput();
    }

    //Only used in Start method.
    public void StartingGun()
    {
        if (weaponEnum == "Pistol")
        {
            selectedWeapon = 0;
        }

        if (weaponEnum == "Shotgun")
        {
            selectedWeapon = 1;
        }

        if (weaponEnum == "Grenade" && playerScript.grenadesSpare > 0)
        {
            selectedWeapon = 2;
        }
    }

    void GetInput()
    {
        if (playerScript.grenadesSpare == 0)
        {
            if(selectedWeapon == 2) selectedWeapon -= 1;
            SelectWeapon();
        }

        if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            //scrolled Up
            selectedWeapon++;
            if (selectedWeapon > 2) selectedWeapon = 0;
            SelectWeapon();
        }
        if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            //scrolled Down
            selectedWeapon--;
            if (selectedWeapon < 0) selectedWeapon = 2;
            SelectWeapon();
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            selectedWeapon = 0;
            SelectWeapon();
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            selectedWeapon = 1;
            SelectWeapon();
        }
        if (Input.GetKeyDown(KeyCode.Alpha3) && playerScript.grenadesSpare != 0)
        {
            selectedWeapon = 2;
            SelectWeapon();
        }
    }

    void SelectWeapon()
    {

        int i = 0;
        foreach (Transform weapon in transform)
        {
            if (i == selectedWeapon)
                weapon.gameObject.SetActive(true);
            else
                weapon.gameObject.SetActive(false);
            i++;
        }

        if (selectedWeapon == 0)
        {
            uiBullet.enabled = true;
            uiShell.enabled = false;
            uiGrenade.enabled = false;
        }

        if (selectedWeapon == 1)
        {
            uiBullet.enabled = false;
            uiShell.enabled = true;
            uiGrenade.enabled = false;
        }

        if (selectedWeapon == 2)
        {
            uiBullet.enabled = false;
            uiShell.enabled = false;
            uiGrenade.enabled = true;
        }
    }

}
