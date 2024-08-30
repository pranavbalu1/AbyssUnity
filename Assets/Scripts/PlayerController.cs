using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    public Camera playerCamera;
    public float walkSpeed = 6f;
    public float runSpeed = 12f;
    public float jumpPower = 7f;
    public float gravity = 10f;

    public float lookSpeed = 2f;
    public float lookXLimit = 45f;

    private CharacterController characterController;
    private Vector3 moveDirection = Vector3.zero;
    private float rotationX = 0;

    private bool canMove = true;

    private void Start()
    {
        // Get the CharacterController component
        characterController = GetComponent<CharacterController>();

        // Lock the cursor to the center of the screen and hide it
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        // Handle player movement
        HandleMovement();

        // Handle player rotation
        HandleRotation();
    }

    private void HandleMovement()
    {
        // Get the forward and right directions based on the player's transform
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);

        // Check if the player is running
        bool isRunning = Input.GetKey(KeyCode.LeftShift);

        // Calculate the current speed in the x and y directions
        float curSpeedX = canMove ? (isRunning ? runSpeed : walkSpeed) * Input.GetAxis("Vertical") : 0;
        float curSpeedY = canMove ? (isRunning ? runSpeed : walkSpeed) * Input.GetAxis("Horizontal") : 0;

        // Store the current y direction of movement
        float movementDirectionY = moveDirection.y;

        // Calculate the move direction based on the forward and right directions and the current speeds
        moveDirection = (forward * curSpeedX) + (right * curSpeedY);

        // Check if the player is jumping and can move and is grounded
        if (Input.GetButton("Jump") && canMove && characterController.isGrounded)
        {
            // Set the y direction of movement to the jump power
            moveDirection.y = jumpPower;
        }
        else
        {
            // Keep the previous y direction of movement
            moveDirection.y = movementDirectionY;
        }

        // Apply gravity to the move direction if the player is not grounded
        if (!characterController.isGrounded)
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }

        // Move the character controller based on the move direction
        characterController.Move(moveDirection * Time.deltaTime);
    }

    private void HandleRotation()
    {
        // Check if the player can move
        if (!canMove)
            return;

        // Calculate the rotation around the x-axis based on the mouse input
        rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
        rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);

        // Apply the rotation to the player camera
        playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);

        // Rotate the player around the y-axis based on the mouse input
        transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
    }

    //sticky movement when near trap object
    public void setStickyMovement()
    {
        //reduce the speed of the player by 90%
        walkSpeed = walkSpeed * 0.1f;
        runSpeed = runSpeed * 0.1f;

    }

    //reset the speed of the player
    public void setNormalMovement()
    {
        walkSpeed = 6f;
        runSpeed = 12f;
    }

    public void isTrapped(bool inTrap)
    {
        //trigger some animation to player when trapped
        Debug.Log($"Player is trapped (from player.cs) {inTrap}");
    }


}
