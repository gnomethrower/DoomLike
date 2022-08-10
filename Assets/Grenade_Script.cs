using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade_Script : MonoBehaviour
{
    /**
     * 0. On weaponselect, play equipanim.
     * 1. When player left clicks and there are still grenades in inventory, start grenade throw animation.
     * 2. The grenade throw animation will call the animation event that spawns a grenade with a preset velocity.
     * 3. if no grenades remain in inventory, switch to the previously equipped weapon.
     * 
     * **/

    public Animator UI_Grenade;

    void OnEnable()
    {

    }
}
