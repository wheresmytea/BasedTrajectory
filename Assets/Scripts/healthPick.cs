using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class healthPick : MonoBehaviour
{
    public float amount = 30;
    private void OnTriggerEnter(Collider other)
    {
        PlayerMovement health = other.GetComponent<PlayerMovement>();
        if (health)
        {
            health.Heal(amount);
            Destroy(gameObject);
        }
    }
}
