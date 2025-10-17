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

    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float jumpHeight = 10f;
    private Vector3 velocity;

    [Header("Ground Check")]
    [SerializeField] private GameObject groundObject;
    [SerializeField] private float groundRadius = 0.1f;
    [SerializeField] private LayerMask groundMask;

    

    [Header("Rotation")]
    [SerializeField] private float mouseSensitivity = 2f;
    private float xRotation = 0f;
    [SerializeField] Camera camera;


    
    
    
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
        // Clamp Magnitude clamps an entire Vector
        moveDirection = Vector3.ClampMagnitude(moveDirection, 1f);

        float speed = Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : moveSpeed;
        controller.Move(moveDirection * speed * Time.deltaTime);

        if (isGrounded() && Input.GetKeyDown(KeyCode.Space))
        {
            // Use the formula v^2 = v0^2 + 2 * a * delta y
            // v0^2 = -2 * delta a * y => v0 = sqrt(-2 * a * delta y)
            // This creates the velocity vector, now needs to be applied
            velocity.y = Mathf.Sqrt(-2 * gravity * jumpHeight);
        }

        // Apply the velocity vector under the constraints of gravity
        //v = v0 + a * t
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

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
