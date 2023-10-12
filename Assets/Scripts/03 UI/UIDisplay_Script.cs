using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.UIElements;
using BehaviorDesigner.Runtime.Tasks.Unity.UnityGameObject;
//+++++++++++++++++++++++++++++++++++
//TODO Find a way to make the Variables used in UI be pushed by the scripts, so I don't need to update them every frame.
//+++++++++++++++++++++++++++++++++++
public class UIDisplay_Script : MonoBehaviour
{
    [Header("Text References")]
    public TextMeshProUGUI ammoMagText, ammoSpareText, healthText, staminaText, staminaTimerObject;
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
    private GameObject staminaBarSliderObject;
    private Slider staminaBarSlider;
    private GameObject staminaBarFill;
    private Image staminaBarFillImage;
    private bool staminaExhaustedColor = false;

    private GameObject healthBarSliderObject;
    private Slider healthBarSlider;
    private GameObject healthBarFill;
    private Image healthBarFillImage;
    private bool healthCriticalColor = false;

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
        staminaTimerObject = GameObject.Find("StamTimerObject").GetComponent<TextMeshProUGUI>();
        staminaBarSliderObject = GameObject.Find("StaminaBar");
        staminaBarFill = GameObject.Find("StaminaFill");
        staminaBarSlider = staminaBarSliderObject.GetComponent<Slider>();
        staminaBarFillImage = staminaBarFill.GetComponent<Image>();
        staminaBarSlider.maxValue = playerScript.maxStamina;
        staminaBarSlider.value = playerScript.maxStamina;
        StaminaExhaustionThresholdObject = GameObject.Find("StaminaExhaustionThreshold");

        healthBarSliderObject = GameObject.Find("HealthBar");
        healthBarFill = GameObject.Find("HealthFill");
        healthBarSlider = healthBarSliderObject.GetComponent<Slider>();
        healthBarFillImage = healthBarFill.GetComponent<Image>();

        if (healthBarSliderObject == null) { Debug.Log("No HealthBarSliderObject found!"); }
        if (healthBarFill == null) { Debug.Log("No healthBarFill found!"); }
        if (healthBarFillImage == null) { Debug.Log("No healthBarFillImage found!"); }

        healthBarSlider.maxValue = playerScript.maxHealth;
        healthBarSlider.value = playerScript.maxHealth;


        healthBarSlider = healthBarSliderObject.GetComponent<Slider>();

        healthBarSlider.maxValue = playerScript.maxHealth;
        healthBarSlider.value = playerScript.maxHealth;

        StaminaExhaustionThresholdObject.SetActive(false);

        PlayerController_Script.OnPlayerStaminaExhaustion += OnStaminaExhaustion;
        PlayerController_Script.OnPlayerStaminaRecovery += OnStaminaRecovery;
        PlayerController_Script.OnPlayerHealthCritical += OnHealthCritical;
        PlayerController_Script.OnPlayerHealthRecovered += OnHealthRecovered;

        exhaustedStaminaBarColor = new Color(.6f, .5f, .5f, 1f);
        freshStaminaBarColor = new Color(.9f, .9f, .9f, 1f);

        criticalHealthColor = new Color(1f, .35f, 0f, 1f);
        freshHealthBarColor = new Color(1f, .35f, 0f, 1f);

        /* Old Init Code
        weaponHolder = GameObject.Find("WeaponHolder");
        player = GameObject.FindGameObjectWithTag("Player");
        pistol = GameObject.FindGameObjectWithTag("Pistol");
        shotgun = GameObject.FindGameObjectWithTag("Shotgun");
        //grenade = GameObject.FindGameObjectWithTag("Grenade");
        Debug.Log(pistol);
        weapSwitchScript = weaponHolder.GetComponent<WeaponSwitching_Script>();
        playerScript = player.GetComponent<PlayerController_Script>();
        pistolScript = pistol.GetComponent<Pistol_Script>(); // +++++++++ nullref here at initialization ! +++++++++ $$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$
        shotgunScript = shotgun.GetComponent<Shotgun_Script>();
        //grenadeScript = grenade.GetComponent<Grenade_Script>();
        */
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
        staminaBarSlider.value = playerScript.currentStamina;
        healthBarSlider.value = playerScript.currentHealth;
        ammoMagText.text = ammoInMag.ToString();
        ammoSpareText.text = ammoSpare.ToString();
        healthText.text = playerScript.currentHealth.ToString();
        staminaTimerObject.text = playerScript.staminaTimerSec.ToString();
        CheckSelectedWeapon();
    }

    private void OnHealthCritical()
    {
        healthBarFillImage.color = criticalHealthColor;
        
        //Low-Health Vignette?
    }

    private void OnHealthRecovered()
    {
        healthBarFillImage.color = freshHealthBarColor;
        //Recovery Effect?
    }

    private void OnStaminaExhaustion()
    {
        //Turn stamina bar red if it is white
        if (!staminaExhaustedColor)
        {
            if (staminaBarFillImage != null)
            {
                //Debug.Log("Set red!");
                StaminaExhaustionThresholdObject.SetActive(true);
                staminaExhaustedColor = true;
                staminaBarFillImage.color = exhaustedStaminaBarColor;
            }
        }
    }

    private void OnStaminaRecovery()
    {
        //Turn the stamina bar white if it is red
        if (staminaExhaustedColor)
        {
            if (staminaBarFillImage != null)
            {
                //Debug.Log("Set white!");
                staminaExhaustedColor = false;
                StaminaExhaustionThresholdObject.SetActive(false);
                staminaBarFillImage.color = freshStaminaBarColor;
            }
        }
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
    //    //f�r jede patrone/granate im Magazin, soll ein prefab startend ab ammIconStartLocation instanziert werden, mit dem vorgegebenen iconOffset.
    //    for (int interruptIterator = 0; interruptIterator < ammoInMag; interruptIterator++)
    //    {
    //        GameObject _newAmmoIcon = Instantiate(ammoIcon,
    //                                              ammoIconStartLocation.position + (new Vector3(iconOffset, 0f, 0f) * interruptIterator),
    //                                              ammoIconStartLocation.rotation,
    //                                              ammoIconStartLocationParent);
    //    }
    //}
}