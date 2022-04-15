using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using TMPro;

//+++++++++++++++++++++++++++++++++++


//TODO Find a way to make the Variables used in UI be pushed by the scripts, so I don't need to update them every frame.


//+++++++++++++++++++++++++++++++++++

public class UIDisplay : MonoBehaviour
{
    public TextMeshProUGUI ammoMagText, ammoSpareText, healthText;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        healthText.text = PlayerController.currentHealth.ToString();
        ammoMagText.text = sg_Script.bulletsInMag.ToString();
        ammoSpareText.text = sg_Script.spareBullets.ToString();
    }
}
