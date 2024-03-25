using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveData_Script : MonoBehaviour
{
    public Vector3 playerPosition;
    public int shotgunAmmoMag, shotgunAmmoSpare, pistolAmmoMag, pistolAmmoSpare, grenadesSpare;
    public int playerHealth;
    public int playerStamina;
    PlayerPrefs playerPrefs;
}

//private bool Quicksave => Input.GetKeyDown(KeyCode.F5);
//private bool Quickload => Input.GetKeyDown(KeyCode.F9);

//public GameObject playerObject;
//public int shotgunAmmoMag, shotgunAmmoSpare, pistolAmmoMag, pistolAmmoSpare, grenadesSpare;

//private void Awake()
//{
//    playerObject = GameObject.Find("Player");
//    PlayerController_Script playerControllerScript = playerObject.GetComponent<PlayerController_Script>();
//}

//public void QuickSave()
//{

//}