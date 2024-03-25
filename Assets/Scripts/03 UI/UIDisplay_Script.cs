using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;
//using UnityEditor.UIElements;
using BehaviorDesigner.Runtime.Tasks.Unity.UnityGameObject;

public class UIDisplay_Script : MonoBehaviour
{
    [Header("Text References")]
    public TextMeshProUGUI ammoMagText, ammoSpareText, staminaText, staminaTimerText, healthBarText, ammoDividerLine;
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
    private bool staminaBarActive;

    private GameObject healthBarSliderObject;
    private Slider healthBarSlider;
    private GameObject healthBarFill;
    private Image healthBarFillImage;
    private Text healthBarTextObject;
    private bool debugModeEnabled;

    [SerializeField] private GameObject StaminaExhaustionThresholdObject;
    [SerializeField] private Color freshStaminaBarColor;
    [SerializeField] private Color exhaustedStaminaBarColor;
    [SerializeField] private Color freshHealthBarColor;
    [SerializeField] private Color criticalHealthColor;
    [SerializeField] private float alphaColorValueTransparent;
    [SerializeField] private float alphaColorValueSolid;

    public Transform ammoIconStartLocationParent;
    public Transform ammoIconStartLocation;
    private GameObject weaponHolder;

    [Header("Script References")]
    public PlayerController_Script playerScript;
    public WeaponSwitching_Script weapSwitchScript;
    public Pistol_Script pistolScript;
    public Shotgun_Script shotgunScript;
    public UI_Grenade_Script grenadeScript;
    //private UI_Grenade_Script grenadeScript;

    void Start()
    {
        #region Object Initialization
        //HealthObjects
        healthBarText = GameObject.Find("healthBarText").GetComponent<TextMeshProUGUI>();
        healthBarSliderObject = GameObject.Find("HealthBar");
        healthBarFill = GameObject.Find("HealthFill");
        healthBarSlider = healthBarSliderObject.GetComponent<Slider>();
        healthBarFillImage = healthBarFill.GetComponent<Image>();
        healthBarSlider = healthBarSliderObject.GetComponent<Slider>();

        //HealthValues
        healthBarSlider.maxValue = playerScript.maxHealth;
        healthBarSlider.value = playerScript.maxHealth;

        //StaminaObjects
        staminaTimerText = GameObject.Find("StamTimerObject").GetComponent<TextMeshProUGUI>();
        staminaBarSliderObject = GameObject.Find("StaminaBar");
        staminaBarFill = GameObject.Find("StaminaFill");
        staminaBarSlider = staminaBarSliderObject.GetComponent<Slider>();
        staminaBarFillImage = staminaBarFill.GetComponent<Image>();
        StaminaExhaustionThresholdObject = GameObject.Find("StaminaExhaustionThreshold");

        StaminaExhaustionThresholdObject.SetActive(false);
        staminaTimerText.enabled = false;

        //StaminaValues
        staminaBarSlider.maxValue = playerScript.maxStamina;
        staminaBarSlider.value = playerScript.maxStamina;

        //AmmoValues
        ammoMagText = GameObject.Find("ammoMagText").GetComponent<TextMeshProUGUI>();
        ammoSpareText = GameObject.Find("ammoSpareText").GetComponent<TextMeshProUGUI>();
        ammoDividerLine = GameObject.Find("Divider").GetComponent<TextMeshProUGUI>();

        //Colors
        exhaustedStaminaBarColor = new Color(.6f, .5f, .5f, 1f);
        freshStaminaBarColor = new Color(.9f, .9f, .9f, 1f);

        criticalHealthColor = new Color(1f, .35f, 0f, 1f);
        freshHealthBarColor = new Color(1f, 1f, 1f, 1f);
        #endregion

        #region event subscription
        PlayerController_Script.Action_PlayerStaminaExhausted += OnStaminaExhaustion;
        PlayerController_Script.Action_PlayerStaminaRecovered += OnStaminaRecovery;
        PlayerController_Script.Action_PlayerHealthCritical += OnHealthCritical;
        PlayerController_Script.Action_PlayerHealthRecoveredFromCritical += OnHealthRecovered;

        EventManagerMaster.Action_ToggleDebugMode += OnDebugModeToggle;
        #endregion

        #region checkfornulls
        if (healthBarSliderObject == null) { Debug.Log("No HealthBarSliderObject found!"); }
        if (healthBarFill == null) { Debug.Log("No healthBarFill found!"); }
        if (healthBarFillImage == null) { Debug.Log("No healthBarFillImage found!"); }
        #endregion
        
        //UIAmmoIconCreation();
    }

    void Update()
    {
        UIValuesUpdate();
        CheckSelectedWeapon();
        CheckForStaminaBarVisibility();
    }

    void CheckSelectedWeapon()
    {
        if (weapSwitchScript.selectedWeapon == 0) //pistol
        {
            CheckForTransparency();

            ammoInMag = pistolScript.bulletsInMag;
            ammoSpare = playerScript.pistolSpareAmmo;
        }
        if (weapSwitchScript.selectedWeapon == 1) //shotgun
        {
            CheckForTransparency();

            ammoInMag = shotgunScript.bulletsInMag;
            ammoSpare = playerScript.shotgunSpareAmmo;
        }
        if (weapSwitchScript.selectedWeapon == 2) //grenade
        {
            ammoMagText.alpha = 0;
            ammoDividerLine.alpha = 0;
            ammoSpare = playerScript.grenadesSpare;
        }
    }

    private void CheckForTransparency()
    {
        if (ammoDividerLine.alpha == 0) ammoDividerLine.alpha = 1;
        if (ammoMagText.alpha == 0) ammoMagText.alpha = 1;
    }

    private void UIValuesUpdate()
    {
        staminaBarSlider.value = playerScript.currentStamina;
        healthBarSlider.value = playerScript.currentHealth;

        ammoMagText.text = ammoInMag.ToString();
        ammoSpareText.text = ammoSpare.ToString();

        healthBarText.text = playerScript.currentHealth.ToString();
        if (staminaTimerText == enabled) staminaTimerText.text = playerScript.staminaTimerSec.ToString();
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

    private void CheckForStaminaBarVisibility()
    {
        if (playerScript.currentStamina <= 0 && staminaBarActive)
        {
            staminaBarFill.SetActive(false);
            staminaBarActive = false;
        }
        if (playerScript.currentStamina > 0)
        {
            staminaBarFill.SetActive(true);
            staminaBarActive = true;
        }
    }

    private void OnStaminaRecovery()
    {
        //Turn the stamina bar white if it is red
        if (staminaExhaustedColor)
        {
            if (staminaBarFillImage != null)
            {
                staminaExhaustedColor = false;
                StaminaExhaustionThresholdObject.SetActive(false);
                staminaBarFillImage.color = freshStaminaBarColor;
            }
        }
    }

    private void OnDebugModeToggle()
    {
        staminaTimerText.enabled = !staminaTimerText.enabled;
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