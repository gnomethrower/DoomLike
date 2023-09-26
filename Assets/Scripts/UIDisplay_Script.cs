
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.UIElements;

//+++++++++++++++++++++++++++++++++++


//TODO Find a way to make the Variables used in UI be pushed by the scripts, so I don't need to update them every frame.


//+++++++++++++++++++++++++++++++++++

public class UIDisplay_Script : MonoBehaviour
{
    [Header("Text References")]
    public TextMeshProUGUI ammoMagText, ammoSpareText, healthText, staminaText;

    [Header("Ammo Icon prefabs")]
    public GameObject shellIcon;
    public GameObject bulletIcon;
    public GameObject grenadeIcon;

    //private float iconOffset;

    //[Header("Ammo background vars")]
    //private int _maxPistolBullets;
    //private int _maxShells;
    //private int _maxGrenades;

    private int ammoInMag;
    private int ammoSpare;
    //private int pistolMagFill;
    //private int shotgunMagFill;
    //private int nadeMagFill;

    [Header("Object References")]
    public GameObject grenade;
    public GameObject shotgun;
    public GameObject pistol;
    public GameObject player;


    private GameObject staminaBarObject;
    private GameObject healthBarObject;
    private Slider staminaBar;
    private GameObject staminaBarFill;
    private Image staminaBarFillImage;
    private Slider healthBar;

    [SerializeField] private GameObject StaminaExhaustionThresholdObject;

    [SerializeField] private Color freshStaminaBarColor;
    [SerializeField] private Color exhaustedStaminaBarColor;

    [SerializeField] private Color freshHealthBarColor;
    [SerializeField] private Color criticalHealthColor;



    public Transform ammoIconStartLocationParent;
    public Transform ammoIconStartLocation;

    private GameObject weaponHolder;

    //[Header("Script References")]
    public PlayerController_Script playerScript;
    public WeaponSwitching_Script weapSwitchScript;
    public Pistol_Script pistolScript;
    public Shotgun_Script shotgunScript;
    //private Grenade_Script grenadeScript;


    // Start is called before the first frame update
    void Start()
    {
        Initialize();
        //UIAmmoIconCreation();
    }

    void Initialize()
    {
        staminaBarObject = GameObject.Find("StaminaBar");
        staminaBar = staminaBarObject.GetComponent<Slider>();
        staminaBar.maxValue = playerScript.maxStamina;
        staminaBar.value = playerScript.maxStamina;
        staminaBarFill = GameObject.Find("Stamina Fill");
        staminaBarFillImage = staminaBarFill.GetComponent<Image>();

        healthBarObject = GameObject.Find("HealthBar");
        healthBar = healthBarObject.GetComponent<Slider>();
        healthBar.maxValue = playerScript.maxHealth;
        healthBar.value = playerScript.maxHealth;

        StaminaExhaustionThresholdObject = GameObject.Find("StaminaExhaustionThreshold");
        StaminaExhaustionThresholdObject.SetActive(false);

        PlayerController_Script.OnPlayerStaminaExhaustion += OnStaminaExhausted;
        PlayerController_Script.OnPlayerStaminaRecovery+= OnStaminaRecovery;

        //weaponHolder = GameObject.Find("WeaponHolder");
        //player = GameObject.FindGameObjectWithTag("Player");
        //pistol = GameObject.FindGameObjectWithTag("Pistol");
        //shotgun = GameObject.FindGameObjectWithTag("Shotgun");
        ////grenade = GameObject.FindGameObjectWithTag("Grenade");

        //Debug.Log(pistol);

        //weapSwitchScript = weaponHolder.GetComponent<WeaponSwitching_Script>();
        //playerScript = player.GetComponent<PlayerController_Script>();
        //pistolScript = pistol.GetComponent<Pistol_Script>(); // +++++++++ nullref here at initialization ! +++++++++ $$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$
        //shotgunScript = shotgun.GetComponent<Shotgun_Script>();
        ////grenadeScript = grenade.GetComponent<Grenade_Script>();
    }


    void CheckSelectedWeapon()
    {
        if (weapSwitchScript.selectedWeapon == 0) //pistol
        {
            ammoInMag = pistolScript.bulletsInMag;
            ammoSpare = playerScript.pistolSpareAmmo;
        }
        if (weapSwitchScript.selectedWeapon == 1) //shotgun
        {
            ammoInMag = shotgunScript.bulletsInMag;
            ammoSpare = playerScript.shotgunSpareAmmo;
        }
    }

    void Update()
    {
        staminaBar.value = playerScript.currentStamina;

        ammoMagText.text = ammoInMag.ToString();
        ammoSpareText.text = ammoSpare.ToString();

        healthText.text = playerScript.currentHealth.ToString();

        CheckSelectedWeapon();


        /*
        if (weapSwitchScript.selectedWeapon == 2) //nades
        {
            ammoInMag = grenadeScript.bulletsInMag;
            ammoSpare = playerScript.grenadeSpareAmmo;

        }

        if (Input.GetKeyDown(KeyCode.H)) UIAmmoIconCreation();
        */
    }

    private void OnStaminaExhausted()
    {
        Debug.Log("Stamina is exhausted!");
        if(staminaBarFillImage != null) staminaBarFillImage.color = exhaustedStaminaBarColor;
        //if(StaminaExhaustionThresholdObject != null) StaminaExhaustionThresholdObject.SetActive(true);
    }

    private void OnStaminaRecovery()
    {
        Debug.Log("Stamina is back!");
        if (staminaBarFillImage != null) staminaBarFillImage.color = freshStaminaBarColor;
        //if (StaminaExhaustionThresholdObject != null) StaminaExhaustionThresholdObject.SetActive(false);
    }

    //Ammocounter like https://www.youtube.com/watch?v=3uyolYVsiWc
    // I need the following ammoIconStartLocation, ammoIcon, iconOffset, shellIcon, bulletIcon
    //void UIAmmoIconCreation()
    //{
    //    if (weapSwitchScript.selectedWeapon == 0)
    //    {
    //        ammoIcon = bulletIcon;
    //        ammoInMag = pistolScript.bulletsInMag;
    //    }
    //    if (weapSwitchScript.selectedWeapon == 1)
    //    {
    //        ammoIcon = shellIcon;
    //        ammoInMag = shotgunScript.bulletsInMag;
    //    }
    //    //if (switchingScript.selectedWeapon == 2) ammoIcon = grenadeIcon;

    //    //für jede patrone/granate im Magazin, soll ein prefab startend ab ammIconStartLocation instanziert werden, mit dem vorgegebenen iconOffset.
    //    for (int interruptIterator = 0; interruptIterator < ammoInMag; interruptIterator++)
    //    {
    //        GameObject _newAmmoIcon = Instantiate(ammoIcon,
    //                                              ammoIconStartLocation.position + (new Vector3(iconOffset, 0f, 0f) * interruptIterator),
    //                                              ammoIconStartLocation.rotation,
    //                                              ammoIconStartLocationParent);
    //    }
    //}



}
