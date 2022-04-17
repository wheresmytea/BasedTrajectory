using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class stormDamage : MonoBehaviour
{
    public float amount = 30;

    private void OnTriggerEnter(Collider other3)
    {
        PlayerMovement storm = other3.GetComponent<PlayerMovement>();
        if (storm)
        {
            storm.Stormy(amount);
            Destroy(gameObject);
        }
    
    }
}
