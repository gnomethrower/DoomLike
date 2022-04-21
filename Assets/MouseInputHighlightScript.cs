using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MouseInputHighlightScript : MonoBehaviour
{
    Image lmbImage;
    // Start is called before the first frame update
    void Start()
    {
        lmbImage = this.GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButton("Fire1"))
        {
            lmbImage.color = new Color32(255, 0, 0, 255);
        }
        else lmbImage.color = new Color32(255, 255, 255, 255);
    }
}
