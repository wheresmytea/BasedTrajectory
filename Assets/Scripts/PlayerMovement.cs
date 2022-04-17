using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController controller;
    public Transform groundCheck;
    public LayerMask groundMask;
    public float speed = 6f;
    public float gravity = -12f;
    public float groundDistance = 0.4f;

    public bool disabled = false;
    public float jumpHeight = 3f;
    public float currentHealth = 100;
    public float currentArmour = 10;
    public TextMeshProUGUI healthText, armourText;
    bool isGrounded;
    Vector3 velocity;

    void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        Vector3 move = transform.right * x + transform.forward * z;
        controller.Move(move * speed * Time.deltaTime);
        if(Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2 * gravity);
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        healthText.SetText("Health : " + currentHealth);
        armourText.SetText("Armour : " + currentArmour);
    }

    public void LeaveGame ()
    {
        if(Input.GetKeyDown(KeyCode.E))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
        }
    }
    public void Heal(float amount)
    {
        currentHealth += amount;
    }

    internal void Armo(float amount)
    {
        currentArmour += amount;
    }

    internal void Stormy(float amount)
    {
        currentHealth -= amount;
        currentArmour -= amount;
    }
}
