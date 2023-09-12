using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMovement : MonoBehaviour
{
    public bool isDisabled { get; set; } = false;
    [SerializeField] private CharacterController controller;
    private float moveSpeed = 10f;
    private float gravity = -9.81f;

    private Vector3 velocity;

    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundDistance = 0.4f;
    [SerializeField] private LayerMask groundMask;
    private bool isGrounded;

    private float jumpHeight = 3f;

    void Update()
    {
        if (!isDisabled)
        {
            isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

            if (isGrounded && velocity.y < 0)
                velocity.y = -2f;


            float moveX = Input.GetAxis("Horizontal");
            float moveZ = Input.GetAxis("Vertical");

            Vector3 move = transform.right * moveX + transform.forward * moveZ;

            controller.Move(move * moveSpeed * Time.deltaTime);

            if (Input.GetButtonDown("Jump") && isGrounded)
                velocity.y = Mathf.Sqrt(jumpHeight * -2 * gravity);

            velocity.y += gravity * Time.deltaTime;

            controller.Move(velocity * Time.deltaTime);
        }
    }
}
