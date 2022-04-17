using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerTeleport : MonoBehaviour
{
    void Start()
    {
        
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.U))
        {
            gameObject.transform.position = new Vector3(71.29f, 26.10f, 33.01f);
        }
    }
}
