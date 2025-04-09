using System;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 8f;
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float gravityMultiplier = 2f;

    [Header("Ground Detection")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundDistance = 0.4f;
    [SerializeField] private LayerMask groundMask;

    private CharacterController controller;

    private Vector3 velocity;
    private bool isGrounded;

    private void Start()
    {
        controller = GetComponent<CharacterController>();

        if (groundCheck == null)
        {
            GameObject check = new GameObject("GroundCheck");
            check.transform.parent = transform;
            check.transform.localPosition = new Vector3(0, -0.9f, 0);
            groundCheck = check.transform;
        }
    }

    private void Update()
    {
        CheckGrounded();
        HandleMovement();
        HandleJump();
        ApplyGravity();
    }

    private void CheckGrounded()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }
    }

    private void HandleMovement()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;

        if (move.magnitude > 0)
        {
            move.Normalize();
        }

        controller.Move(move * moveSpeed * Time.deltaTime);
    }

    private void HandleJump()
    {
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity * gravityMultiplier);
        }
    }

    private void ApplyGravity()
    {
        velocity.y += gravity * gravityMultiplier * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}