using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ammoPick : MonoBehaviour
{
    public float amount = 0;
    private void OnTriggerEnter(Collider other2)
    {
        PlayerMovement ammo = other2.GetComponent<PlayerMovement>();
        if (ammo)
        {
            Destroy(gameObject);
        }
    }
}
