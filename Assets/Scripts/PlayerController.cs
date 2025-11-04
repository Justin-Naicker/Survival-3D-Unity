using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XR;

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
    [SerializeField] private GameObject groundObject;

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



        float targetHeight = isCrouching ? crouchingHeight : standingHeight;
        Vector3 targetCenter = isCrouching ? new Vector3(0f, -0.5f, 0f) : Vector3.zero;

        // Lerp(a,b,t) = a + (b - a) * t
        // a = 2 (current height), b = 1 (target crouch height), t = 0.1 (time)
        // Lerp(2,1,0.1) = 2 + (1 - 2) * 0.1 = 1.9

        // Vector3.Lerp((x1, y1, z1), (x2, y2, z2), t) = (x1 +(x2 − x1) ∗ t, y1 + (y2 − y1) ∗ t, z1 + (z2 − z1) ∗t)
        controller.height = Mathf.Lerp(controller.height, targetHeight, Time.deltaTime * 8f);
        controller.center = Vector3.Lerp(controller.center, targetCenter, Time.deltaTime * 8f);
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
