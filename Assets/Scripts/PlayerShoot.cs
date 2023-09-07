using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShoot : MonoBehaviour
{
    #region Setup

    private PlayerMovement _playerMovement;
    private PlayerControl _playerInput;
    [SerializeField] private Animator animator;
    private Transform cameraTransform;


    #endregion

    #region AimZoom

    [Header("Zoom Settings")] 
    private bool _isAimZoomPressed = false;
    
    #endregion

    #region Shoot
    
    [SerializeField] private float fireRate;
    [SerializeField] private PlayerReload _reload;
    [SerializeField] private LayerMask shootLayer;
    private bool _isShootPressed = false;
    public bool _canShoot = true;
    private float nextFire;

    #endregion

    #region Bullet

    [Header("Bullet Settings")] 
    [SerializeField] private float bulletDamage;
    [SerializeField] private Transform outputRayWeapon;

    [SerializeField] private float range;
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform bulletParent;

    #endregion

    private void Awake()
    {
        _playerInput = new PlayerControl();
        
        _playerInput.PlayerController.Shoot.started += OnShoot;
        _playerInput.PlayerController.Shoot.canceled += OnShoot;
        
        _playerInput.PlayerController.Zoom.started += OnZoom;
        _playerInput.PlayerController.Zoom.performed += OnZoom;
        _playerInput.PlayerController.Zoom.canceled += OnZoom;

    }
    private void Start()
    {
        cameraTransform = Camera.main.transform;
        _playerMovement = GetComponent<PlayerMovement>();
    }


    private void Update()
    {
        if (_isShootPressed && Time.time > nextFire && _canShoot)
        {
            nextFire = Time.time + fireRate;

            StartCoroutine(Trigger());
        }
        
        AimZoom();
    }


    void AimZoom()
    {
        if (_isAimZoomPressed)
        {
            animator.SetBool("isAimZoomWalk", true);
        }
        else if (_isAimZoomPressed && _playerMovement._isMovementPressed)
        {
            animator.SetBool("isAimZoomWalk", true);
        }
        else if (_isAimZoomPressed && _playerMovement._isCrouchPressed)
        {
            animator.SetBool("isAimZoomCrouch", true);
        }
        else
        {
            animator.SetBool("isAimZoomWalk", false);
            animator.SetBool("isAimZoomCrouch", false);
        }
    }


    IEnumerator Trigger()
    {
        yield return new WaitForSeconds(fireRate);
        if (_isShootPressed && _reload.bulletClip > 0)
        {
            animator.SetBool("isShoot", true);
            RaycastHit hit;
            
            if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out hit, Mathf.Infinity, shootLayer))
            {
                Vector3 targetPosition = hit.point;

                GameObject bullet = Instantiate(bulletPrefab, outputRayWeapon.position, Quaternion.identity, bulletParent);

                PlayerBulletController bulletController = bullet.GetComponent<PlayerBulletController>();

                if (bulletController != null)
                {
                    bulletController.target = targetPosition;
                }
            }
            
            _reload.bulletClip--;
        }
        else
            animator.SetBool("isShoot", false);
    }
    
    void OnShoot(InputAction.CallbackContext context)
    {
        _isShootPressed = context.ReadValueAsButton();
    }
    void OnZoom(InputAction.CallbackContext context)
    {
        _isAimZoomPressed = context.ReadValueAsButton();
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
