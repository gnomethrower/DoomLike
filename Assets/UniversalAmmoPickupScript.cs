using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UniversalAmmoPickupScript : MonoBehaviour
{
    private GameObject player;
    private Collider playerCollider;
    private PlayerController_Script playerControllerScript;

    [SerializeField] private PickupType pickupType;
    [Tooltip("Small currently has no separate sprites")][SerializeField] private PickupSize pickupSize;
    [SerializeField] private enum PickupType { shotgunAmmo, pistolAmmo, grenadeAmmo};
    [SerializeField] private enum PickupSize { small, large};


    [SerializeField] private int shotgunPickupSmall;
    [SerializeField] private int shotgunPickupLarge;

    [SerializeField] private int pistolPickupSmall;
    [SerializeField] private int pistolPickupLarge;
    
    [SerializeField] private int grenadePickupSmall;
    [SerializeField] private int grenadePickupLarge;

    private int ammo;
    private SpriteRenderer spriteRenderer;
    [SerializeField] private Sprite[] ammoSprite;
    

    private void Start()
    {
        player = GameObject.Find("Player");
        playerCollider = player.GetComponent<Collider>();
        playerControllerScript = player.GetComponent<PlayerController_Script>();
        spriteRenderer = gameObject.AddComponent<SpriteRenderer>();

        AmmoSetup();
    }

    void AmmoSetup()
    {
        if (pickupType == PickupType.pistolAmmo)
        {
            spriteRenderer.sprite = ammoSprite[0];
            //Small Pistol
            if (pickupSize == PickupSize.small)
            {
                ammo = pistolPickupSmall;
            }
            //Large Pistol
            if (pickupSize == PickupSize.large)
            {
                ammo = pistolPickupLarge;
            }
        }

        if (pickupType == PickupType.shotgunAmmo)
        {
            spriteRenderer.sprite = ammoSprite[1];

            //Small Shotgun
            if (pickupSize == PickupSize.small)
                {
                    ammo = shotgunPickupSmall;
                }
            //Large Shotgun
            if (pickupSize == PickupSize.large)
                {
                    ammo = shotgunPickupLarge;
                }
        }
 
           if(pickupType == PickupType.grenadeAmmo)
        {
            spriteRenderer.sprite = ammoSprite[2];

            //Small Grenade
            if (pickupSize == PickupSize.small)
                {
                    ammo = grenadePickupSmall;
                }
            //Large Grenade
            if (pickupSize == PickupSize.large)
                {
                    ammo = grenadePickupLarge;
                }
        }
            
    }

    private void OnTriggerStay(Collider other)
    {
        if(other == playerCollider)
        {
            if(pickupType == PickupType.shotgunAmmo)
            {
                if(playerControllerScript.shotgunSpareAmmo < playerControllerScript.shotgunMaxAmmo)
                {
                    playerControllerScript.shotgunSpareAmmo += ammo;
                    if (playerControllerScript.shotgunSpareAmmo > playerControllerScript.shotgunMaxAmmo) playerControllerScript.shotgunSpareAmmo = playerControllerScript.shotgunMaxAmmo;
                }
            }

            if (pickupType == PickupType.pistolAmmo)
            {
                if (playerControllerScript.pistolSpareAmmo < playerControllerScript.pistolMaxAmmo)
                {
                    playerControllerScript.pistolSpareAmmo += ammo;
                    if (playerControllerScript.pistolSpareAmmo > playerControllerScript.pistolMaxAmmo) playerControllerScript.pistolSpareAmmo = playerControllerScript.pistolMaxAmmo;
                }                
            }

            if (pickupType == PickupType.grenadeAmmo)
            {
                if (playerControllerScript.grenadesSpare < playerControllerScript.grenadesMax)
                {
                    playerControllerScript.grenadesSpare += ammo;
                    if (playerControllerScript.grenadesSpare > playerControllerScript.grenadesMax) playerControllerScript.grenadesSpare = playerControllerScript.grenadesMax;
                }
            }

            Destroy(this.transform.gameObject);
        }
    }
}
