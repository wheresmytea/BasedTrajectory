using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pickUpSystem : MonoBehaviour
{
    public GunSystem gunScript;
    public Rigidbody rb;
    public BoxCollider coll;
    public Transform player, gunContainer, fpsCam;

    public float pickUpRange;
    public float dropForwardForce, dropUpwardForce;
    public bool equipped;
    public static bool slotFull;

    public void Start()
    {
        if (!equipped)
        {
            gunScript.enabled = false;
            rb.isKinematic = false;
            coll.isTrigger = false;
        }
        if (equipped)
        {
            gunScript.enabled = true;
            rb.isKinematic = true;
            coll.isTrigger = true;
            slotFull = true;
        }
    }
    private void Update()
    {
        //check if player is in range and "F" is pressed
        Vector3 distanceToPlayer = player.position - transform.position;
        if (!equipped && distanceToPlayer.magnitude <= pickUpRange && Input.GetKeyDown(KeyCode.F) && !slotFull) PickUp();
        //Drop if equipped and "Q" is pressed
        if (equipped && Input.GetKeyDown(KeyCode.Q)) Drop();
    }

    private void PickUp()
    {
        equipped = true;
        slotFull = true;
        //Make weapon a child of the camera and move it to default position
        transform.SetParent(gunContainer);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.Euler(Vector3.zero);
        transform.localScale = Vector3.one;
        //Make Rigidbody Kinematic and BoxCollider a trigger
        rb.isKinematic = true;
        coll.isTrigger = true;
        //Enable Script
        gunScript.enabled = true;

    }

    private void Drop()
    {
        equipped = false;
        slotFull = false;
        //set parent to null
        transform.SetParent(null);
        //Make Rigidbody not Kinematic and BoxCollider normal
        rb.isKinematic = false;
        coll.isTrigger = false;
        //gun carries momentum of player
        rb.velocity = player.GetComponent<Rigidbody>().velocity;
        //add force
        rb.AddForce(fpsCam.forward * dropForwardForce, ForceMode.Impulse);
        rb.AddForce(fpsCam.up * dropForwardForce, ForceMode.Impulse);
        //Add random rotation
        float random = Random.Range(-1f, 1f);
        rb.AddTorque(new Vector3(random, random, random) * 10);
        //Enable Script
        gunScript.enabled = false;
    }
}
