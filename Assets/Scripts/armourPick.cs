using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class armourPick : MonoBehaviour
{
    public float amount = 80;
    private void OnTriggerEnter(Collider other1)
    {
        PlayerMovement armour = other1.GetComponent<PlayerMovement>();
        if (armour)
        {
            armour.Armo(amount);
            Destroy(gameObject);
        }
    }

}
