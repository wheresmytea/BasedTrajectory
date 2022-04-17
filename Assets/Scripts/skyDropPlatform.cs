using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class skyDropPlatform : MonoBehaviour
{
    private void OnTriggerEnter(Collider other5)
    {
        PlayerMovement skyDrop = other5.GetComponent<PlayerMovement>();
        if (skyDrop)
        {
            Destroy(gameObject);
        }
    }
}
