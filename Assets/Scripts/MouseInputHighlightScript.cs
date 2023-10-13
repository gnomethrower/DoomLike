using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MouseInputHighlightScript : MonoBehaviour
{
    private Image lmbImage, rmbImage, mouseBodyImage;
    private TextMeshProUGUI debuggerText;
    private bool debugModeEnabled = false;

    private void Awake()
    {
        Initialize();
    }

    void Start()
    {
        
    }

    void Update()
    {
        MouseButtonColorChange();
    }

    private void Initialize()
    {
        //Objects
        mouseBodyImage = GameObject.Find("Mouse Body DebugImage").GetComponent<Image>();
        lmbImage = GameObject.Find("LMB DebugImage").GetComponent<Image>();
        rmbImage = GameObject.Find("RMB DebugImage").GetComponent<Image>();
        debuggerText = GameObject.Find("MouseDebuggerText").GetComponent<TextMeshProUGUI>();
       
        //Variable Values and States
        mouseBodyImage.enabled = false;
        lmbImage.enabled = false;
        rmbImage.enabled = false;
        debuggerText.enabled = false;


        //Event subscriptions
        EventManagerMaster.Action_ToggleDebugMode += OnDebugModeToggle;
    }

    private void MouseButtonColorChange()
    {
        if (Input.GetButton("Fire1"))
        {
            lmbImage.color = new Color32(255, 0, 0, 255);
        }
        else lmbImage.color = new Color32(255, 255, 255, 255);

        if (Input.GetButton("Fire2"))
        {
            rmbImage.color = new Color32(255, 0, 0, 255);
        }
        else rmbImage.color = new Color32(255, 255, 255, 255);
    }

    private void OnDebugModeToggle()
    {
        debugModeEnabled = !debugModeEnabled;

        if (debugModeEnabled)
        {
            mouseBodyImage.enabled = true;
            lmbImage.enabled = true;
            rmbImage.enabled = true;
            debuggerText.enabled = true;
        }

        if (!debugModeEnabled)
        {
            mouseBodyImage.enabled = false;
            lmbImage.enabled = false;
            rmbImage.enabled = false;
            debuggerText.enabled = false;
        }
    }
}
