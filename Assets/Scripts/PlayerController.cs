using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[

RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    private CharacterController controller;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float sprintSpeed = 8f;
    [SerializeField] private float crouchSpeed = 1f;

    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float jumpHeight = 1f;
    private Vector3 velocity;

    [Header("Ground Check")]
    [SerializeField] private float groundRadius = 0.1f;
    [SerializeField] private LayerMask groundMask;

    [Header("Crouch Check")]
    [SerializeField] private bool isCrouching;
    [SerializeField] private float standingHeight = 2f;
    [SerializeField] private float crouchingHeight = 1f;

    [Header("Rotation")]
    [SerializeField] private float mouseSensitivity = 2f;
    private float xRotation = 0f;
    [SerializeField] private new Camera camera;






    void Awake()
    {
        
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        controller = GetComponent<CharacterController>();

    }


    void Update()
    {
        PlayerMovement();
        PlayerRotation();
    }

    void PlayerMovement()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 moveDirection = horizontal * transform.right + vertical * transform.forward;
        moveDirection = Vector3.ClampMagnitude(moveDirection, 1f);

        isCrouching = Input.GetKey(KeyCode.LeftControl);

        float speed = isCrouching ? crouchSpeed : (Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : moveSpeed);
        controller.Move(moveDirection * speed * Time.deltaTime);

        if (isGrounded() && velocity.y < 0)
            velocity.y = -2f;

        if (isGrounded() && !isCrouching && Input.GetKeyDown(KeyCode.Space))
            velocity.y = Mathf.Sqrt(-2 * gravity * jumpHeight);

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        // Smooth crouch/stand
        float targetHeight = isCrouching ? crouchingHeight : standingHeight;
        controller.height = Mathf.Lerp(controller.height, targetHeight, Time.deltaTime * 10f);
        controller.center = new Vector3(0, controller.height / 2f, 0);
    }


    void PlayerRotation()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        // Accounts for rotation system in which when mouseY > 0, Unity's system states that a Positive X rotation will tilt the object down
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -80f, 80f);
        camera.transform.localRotation = Quaternion.Euler(xRotation, 0, 0);
        transform.Rotate(Vector3.up * mouseX);

    }

    bool isGrounded()
    {
        return Physics.CheckSphere(groundObject.transform.position, groundRadius, groundMask);
    }
}
