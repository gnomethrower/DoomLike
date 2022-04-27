using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using TMPro;

public class OLD_VisualAmmoCounter : MonoBehaviour
{
    [Header("Object References")]
    public GameObject grenades;
    public GameObject shotgun;
    public GameObject pistol;
    public GameObject player;
    public Transform bulletIndicatorImageStart;
    public GameObject _bulletImage;
    public GameObject _shellImage;
    public GameObject _ammoUIImage;
    public Transform _ammoUIContainer;

    [Header("Ammo arrays")]
    [SerializeField] private GameObject[] _currentAmmoImages;
    [SerializeField] private GameObject[] _pistolBulletImages;
    [SerializeField] private GameObject[] _shellImages;
    [SerializeField] private GameObject[] _grenadeImages;

    [Header("Background vars")]
    [SerializeField] private int _maxPistolBullets;
    [SerializeField] private int _maxShells;
    [SerializeField] private int _maxGrenades;
    [SerializeField] private int equippedWeapon;
    [SerializeField] private int currentAmmo;

    [Header("Script References")]
    private PlayerController_Script playerScript;
    private Pistol_Script pistolScript;
    private Shotgun_Script shotgunScript;
    //[SerializeField] private Grenade_Script grenadeScript;
    private WeaponSwitching_Script switchingScript;

    // trying to use an Ammocounter like: https://mike-87852.medium.com/create-a-visual-ammo-counter-in-unity-63e640675ea2

    void Start()
    {
        //initializing 
        player = GameObject.FindGameObjectWithTag("Player");

        //Initializing Scripts
        playerScript = player.GetComponent<PlayerController_Script>();
        switchingScript = GameObject.FindGameObjectWithTag("SwitchingScript").GetComponent<WeaponSwitching_Script>();

        pistolScript = GameObject.FindGameObjectWithTag("pistol").GetComponent<Pistol_Script>();
        shotgunScript = GameObject.FindGameObjectWithTag("shotgun").GetComponent<Shotgun_Script>();
        //grenadeScript = GameObject.FindGameObjectWithTag("grenade").GetComponent<Grenade_Script>();

        //Initializing Variables
        equippedWeapon = switchingScript.selectedWeapon;
        _maxPistolBullets = pistolScript.magSize;
        _maxShells = shotgunScript.magSize;
        //_maxGrenades = grenadeScript.maxGrenades;

        currentAmmo = pistolScript.bulletsInMag;

    }

    private void Update()
    {
        SetAmmoUI(currentAmmo);
    }

    void CheckWeapon()
    {
        
        if (equippedWeapon == 0) _ammoUIImage = _bulletImage; currentAmmo = pistolScript.bulletsInMag;
        if (equippedWeapon == 1) _ammoUIImage = _shellImage; currentAmmo = shotgunScript.bulletsInMag;
        //if (equippedWeapon == 2) _currentAmmoImages = _grenadeImages; currentAmmo = grenadeScript.nadesRemaining;
    }

    public void SetAmmoUI(int magAmount) 
    {
        CheckWeapon();
        _currentAmmoImages = _pistolBulletImages;

        int total = 0;
        //int horizCount = 0;
        //int vertCount = 0;

        float imageOffset = 1.5f;
        foreach (var image in _currentAmmoImages)
        {
            Vector3 newPos = bulletIndicatorImageStart.position;
            newPos.x += total * imageOffset;

            GameObject newImage = Instantiate(_ammoUIImage, newPos, Quaternion.identity, transform.parent);
            _currentAmmoImages[total] = newImage;
            total++;
        }
    }
}
