using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gunScript : MonoBehaviour
{

    //Gun stat variables
    public int damage;
    public float timeBetweenShooting, timeBetweenShots, spread, range, reloadTime;
    public int magSize, magFill, bulletsPerTap;
    public bool allowButtonHold;

    // control bools
    bool isShooting, isReloading, readyToShoot;

    //referencing Objects
    public Camera playerCam;
    public Transform attackPoint;
    public RaycastHit hit;
    public LayerMask target;

    private void Update()
    {
        GetInput();
    }

    void GetInput()
    {
        if (!allowButtonHold) isShooting = Input.GetKeyDown("Fire1");
        if (allowButtonHold) isShooting = Input.GetKey("Fire1");
    }

}
