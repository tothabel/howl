using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class Movement : MonoBehaviour
{
    [Header("Components")]
        public Rigidbody rb;
        public Transform cam;

    [Header("Ground Movement")]
        public float maxSpeed;
        public float moveSpeed;
        public float groundDrag;
        private Vector3 direction;

        [Header("Jump")] 
            public float jumpForce;
            public float jumpCooldown;
            public float airMultiplier;
            private bool readyToJump = true;
            private bool grounded;

    [Header("Air Movement")] 
        [Header("Gliding")]
            private bool isGliding;
        
        

    [Header("Ground check")] 
        public float playerHeight;
        public LayerMask whatIsGround;
        //public bool grounded;

    [Header("Camera settings")]
        public float turnSmoothTime;
        private float turnSmoothVelocity;

    [Header("Keybinds")] 
        public KeyCode jumpKey;
        public KeyCode glideKey;

    private void Start()
    {
        rb.freezeRotation = true;
    }

    private void Update()
    {
        GetInputs();
        HandleDrag();
        SpeedControl();
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void GetInputs()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        direction = new Vector3(horizontal, 0, vertical).normalized;
        
        // when to jump
        if (Input.GetKey(jumpKey) && readyToJump && grounded)
        {
            readyToJump = false;
            Jump();
            Invoke(nameof(ResetJump), jumpCooldown);
        }
        
    }

    private void MovePlayer()
    {
        if (direction.magnitude == 0 || isGliding) return;
        
        // rotate player
        float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
        float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity,
            turnSmoothTime);
        transform.rotation = Quaternion.Euler(0f, angle, 0f);

        // calculate move direction
        Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;

        // move on ground
        if (grounded)
            rb.AddForce(moveDir.normalized * moveSpeed, ForceMode.Force);
        // move in air
        else if (!grounded)
            rb.AddForce(moveDir.normalized * (moveSpeed * airMultiplier), ForceMode.Force);
    }

    private void HandleDrag()
    {
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);

        rb.drag = grounded ? groundDrag : 0f;
        
        if (grounded) isGliding = false;
    }

    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        if (flatVel.magnitude > maxSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * maxSpeed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }
    }

    private void Jump()
    {
        // reset y velocity
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    private void ResetJump()
    {
        readyToJump = true;
    }
}
