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
}
