using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSwitching_Script : MonoBehaviour
{

    public static int selectedWeapon;
    string weaponEnum;
    public static GameObject shotgunInit;
    GameObject player;
    PlayerController_Script playerScript;


    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerScript = player.GetComponent<PlayerController_Script>();
        weaponEnum = playerScript.startingGun.ToString();

        StartingGun();
        shotgunInit = GameObject.FindGameObjectWithTag("shotgun");
        SelectWeapon();
    }

    // Update is called once per frame
    void Update()
    {
        GetInput();
    }

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
    }

    void GetInput()
    {
        if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            //scrolled Up
            selectedWeapon--;
            if (selectedWeapon < 0) selectedWeapon = 1;
            SelectWeapon();
        }
        if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            //scrolled Down
            selectedWeapon--;
            if (selectedWeapon < 0) selectedWeapon = 1;
            SelectWeapon();
        }
    }
}
