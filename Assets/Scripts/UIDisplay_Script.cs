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
    [Header("Text References")]
    public TextMeshProUGUI ammoMagText, ammoSpareText, healthText;

    [Header("Object References")]
    public GameObject grenades;
    public GameObject shotgun;
    public GameObject pistol;
    public GameObject player;
    public Transform bulletIndicatorImageStart;
    public GameObject _ammoUI;
    public Transform _ammoUIContainer;
    [SerializeField] private GameObject weaponHolder;

    [Header("Ammo arrays")]
    [SerializeField] private GameObject[] _pistolBulletImages;
    [SerializeField] private GameObject[] _shellImages;
    [SerializeField] private GameObject[] _grenadeImages;

    [Header("Ammo background vars")]
    [SerializeField] private int _maxPistolBullets;
    [SerializeField] private int _maxShells;
    [SerializeField] private int _maxGrenades;

    [Header("Script References")]
    [SerializeField] private PlayerController_Script playerScript;
    [SerializeField] private WeaponSwitching_Script switchingScript;

    // Start is called before the first frame update
    void Start()
    {
        weaponHolder = GameObject.Find("WeaponHolder");
        switchingScript = weaponHolder.GetComponent<WeaponSwitching_Script>();

        player = GameObject.FindGameObjectWithTag("Player");
        playerScript = player.GetComponent<PlayerController_Script>();

        _pistolBulletImages = new GameObject[_maxPistolBullets];
        //shells
        //grenades
    }

    // trying to use an Ammocounter like: https://mike-87852.medium.com/create-a-visual-ammo-counter-in-unity-63e640675ea2
    void SetAmmoUI (int magAmount)
    {
        int total = 0;
        float imageOffset = 1.5f;
        foreach (var image in _pistolBulletImages)
        {
            Vector3 newPos = bulletIndicatorImageStart.position;
            newPos.x += total * imageOffset;
            GameObject newImage = Instantiate(_ammoUI, newPos, Quaternion.identity, transform.parent);
        }
    }

    // Update is called once per frame
    void Update()
    {
        healthText.text = PlayerController_Script.currentHealth.ToString();

        //if (WeaponSwitching.selectedWeapon == 1) equippedWeapon = shotgun.GetComponent<sg_Script>();
        //if (WeaponSwitching.selectedWeapon == 0) equippedWeapon = pistol.GetComponent<PistolScript>();

        if (switchingScript.selectedWeapon == 0) //pistol
        {
            ammoMagText.text = pistol.GetComponent<Pistol_Script>().bulletsInMag.ToString();
            ammoSpareText.text = playerScript.pistolSpareAmmo.ToString();
        }
        if (switchingScript.selectedWeapon == 1) //shotgun
        {
            ammoMagText.text = shotgun.GetComponent<Shotgun_Script>().bulletsInMag.ToString();
            ammoSpareText.text = playerScript.shotgunSpareAmmo.ToString();
        }

    }
}
