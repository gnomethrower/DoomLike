using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSwitching : MonoBehaviour
{

    public static int selectedWeapon = 0;
    public static GameObject shotgunInit;

    // Start is called before the first frame update
    void Start()
    {
        shotgunInit = GameObject.FindGameObjectWithTag("shotgun");
        SelectWeapon();
    }

    // Update is called once per frame
    void Update()
    {
        GetInput();
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
