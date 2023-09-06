using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;

public class PlayerMovement : MonoBehaviour
{
    #region Setup

    public static PlayerControl _playerInput;
    private Rigidbody rb;
    //[SerializeField] private CharacterController _controller;

    #endregion

    #region Movement

    private Vector3 currentMovement;
    private Vector2 movementInput;
    public bool _isMovementPressed = false;
    private bool _isRunPressed = false;
    public bool _isCrouchPressed = false;

    [Header("Movement Settings")] 
    [SerializeField] public float playerSpeed;
    [SerializeField] public float runSpeed;
    [SerializeField] private float rotationSpeed;

    #endregion

    #region Jump

    [Header("JumpSettings")] 
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private float jumpAmount;
    [SerializeField] private Vector3 fallGravity;
    private Vector3 gravity = Physics.gravity;
    private Vector3 constantGravity = new Vector3(0, -9.81f, 0);
    
    private float radius = .1f;
    private bool _isJumpPressed = false;

    #endregion

    #region Animation

    [Header("Animation")]
    
    public Animator animator;
    [SerializeField] private float animationSmoothTime = 0.2f;
    [SerializeField] private float animationPlayTransition = 0.15f;
    
    private int jumpAnimation;
    private int moveXAnimationParameterId;
    private int moveZAnimationParameterId;

    Vector2 currentAnimationBlendVector, animationVelocity;

    #endregion

    #region Camera

    private Transform cameraTransform;
    
    #endregion

    private void Awake()
    {
        _playerInput = new PlayerControl();

        _playerInput.PlayerController.Movement.started += OnMove;
        _playerInput.PlayerController.Movement.performed += OnMove;
        _playerInput.PlayerController.Movement.canceled += OnMove;

        _playerInput.PlayerController.Run.started += OnRun;
        _playerInput.PlayerController.Run.canceled += OnRun;

        _playerInput.PlayerController.Jump.started += OnJump;
        _playerInput.PlayerController.Jump.canceled += OnJump;

        _playerInput.PlayerController.Crouch.started += OnCrouch;
        _playerInput.PlayerController.Crouch.performed += OnCrouch;
        _playerInput.PlayerController.Crouch.canceled += OnCrouch;
        
    }
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        cameraTransform = Camera.main.transform;

        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        
        moveXAnimationParameterId = Animator.StringToHash("MoveX");
        moveZAnimationParameterId = Animator.StringToHash("MoveZ");
        jumpAnimation = Animator.StringToHash("Jump");
    }
    
    private void FixedUpdate()
    {
        PlayerMove();
        PlayerJump();
        PlayerCrouch();
        PlayerAnimation();
        PlayerCamera();
    }


    void PlayerMove()
    {
        Vector3 move = new Vector3(currentAnimationBlendVector.x, 0, currentAnimationBlendVector.y);
        move = move.x * cameraTransform.right.normalized + move.z * cameraTransform.forward.normalized;
        move.y = 0;

        if (_isRunPressed && _isMovementPressed)
        {
            float targetY = Mathf.Clamp(currentAnimationBlendVector.y * 2, 0f, +1);
            currentAnimationBlendVector.y = Mathf.SmoothDamp(currentAnimationBlendVector.y, targetY, ref animationVelocity.y, animationSmoothTime);
        }
        else
        {
            float targetY = 0f;
            currentAnimationBlendVector.y = Mathf.SmoothDamp(currentAnimationBlendVector.y, targetY, ref animationVelocity.y, animationSmoothTime);
        }

        Vector3 newPosition = transform.position + move * (Time.fixedDeltaTime * (_isRunPressed ? runSpeed : playerSpeed));
        rb.MovePosition(newPosition);
    }
    void PlayerJump() 
    {
        if (_isJumpPressed && _playerIsGrounded())
        {
            Debug.Log("if press playerJump" + _isJumpPressed);
            //rb.AddForce(Vector3.up * jumpAmount, ForceMode.Impulse);
            rb.velocity += new Vector3(0,  jumpAmount, 0);

            animator.CrossFadeInFixedTime(jumpAnimation, animationPlayTransition, 0, 0);
            animator.SetBool("isJump", true);
        }
        //else
            animator.SetBool("isJump", false);

        if (!_playerIsGrounded())
            Physics.gravity = Vector3.Lerp(fallGravity, gravity, Time.deltaTime);
        else 
            Physics.gravity = Vector3.Lerp(Physics.gravity, constantGravity, Time.deltaTime);

    }
    void PlayerAnimation()
    {
        currentAnimationBlendVector = Vector2.SmoothDamp(currentAnimationBlendVector, movementInput, ref animationVelocity, animationSmoothTime);
        
        animator.SetFloat(moveXAnimationParameterId, currentAnimationBlendVector.x);
        animator.SetFloat(moveZAnimationParameterId, currentAnimationBlendVector.y);
    }
    void PlayerCamera()
    {
        Quaternion targetRotation = Quaternion.Euler(0, cameraTransform.eulerAngles.y, 0);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }
    void PlayerCrouch()
    {
        if (_isCrouchPressed)
            animator.SetBool("isCrouch", true);
        else
            animator.SetBool("isCrouch", false);
    }
    
    
    bool _playerIsGrounded()
    {
        return Physics.CheckSphere(groundCheck.position, radius, groundMask);
    }
    
    void OnMove(InputAction.CallbackContext context)
    {
        movementInput = context.ReadValue<Vector2>();
        
        currentMovement.x = movementInput.x;
        currentMovement.z = movementInput.y;
        
        _isMovementPressed = movementInput.x != 0 || movementInput.y != 0;
    }
    void OnRun(InputAction.CallbackContext context)
    {
        _isRunPressed = context.ReadValueAsButton();
    }
    void OnJump(InputAction.CallbackContext context)
    {
        _isJumpPressed = context.ReadValueAsButton();
    }
    void OnCrouch(InputAction.CallbackContext context)
    {
        _isCrouchPressed = context.ReadValueAsButton();
    }
    
    
    private void OnEnable()
    {
        _playerInput.PlayerController.Enable();
    }
    private void OnDisable()
    {
        _playerInput.PlayerController.Disable();
    }

}
