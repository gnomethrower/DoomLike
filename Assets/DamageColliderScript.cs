using UnityEngine;

public class DamageColliderScript : MonoBehaviour
{
    public bool insideDamageCollider = false;

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "Player")
        {
            insideDamageCollider = true;
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.gameObject.name == "Player")
        {
            insideDamageCollider = false;
        }
    }

}


