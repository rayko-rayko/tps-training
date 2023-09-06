using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class PlayerReload : MonoBehaviour
{
    #region Setup

    private PlayerControl _playerInput;
    [SerializeField] private GameObject Player;
    private Animator animator;

    #endregion
    
    #region Reload

    [Header("Reload Settings")] 
    private bool _isReloadPressed = false;
    [SerializeField] private int bulletClipCapacity = 30;
    [SerializeField] private int bulletAmmo = 90;
    [SerializeField] public int bulletClip = 30;
    
    public int bulletDiff;

    #endregion

    private void Awake()
    {
        animator = Player.GetComponent<Animator>();
        _playerInput = new PlayerControl();
        
        _playerInput.PlayerController.Reload.started += OnReload;
        _playerInput.PlayerController.Reload.canceled += OnReload;
    }

    private void Update()
    {
        Reload();
    }

    void Reload()
    {
        if (_isReloadPressed)
        {
            if (bulletClip == 0 && bulletAmmo > 0)
            {
                StartCoroutine(IsReloadingWaiting());
                Debug.Log("Bullet is 0 in Clip. Reloading");
                bulletClip = bulletClipCapacity;
                bulletAmmo -= bulletClipCapacity;
            }

            if (bulletClipCapacity > bulletClip && bulletClip > 0 && bulletAmmo > 0)
            {
                StartCoroutine(IsReloadingWaiting());
                bulletDiff = bulletClipCapacity - bulletClip;
                if (bulletDiff > bulletAmmo)
                {
                    bulletDiff = bulletAmmo;
                }

                bulletAmmo -= bulletDiff;
                bulletClip += bulletDiff;
            }
        }
        else
            animator.SetBool("isReload", false);
    }

    IEnumerator IsReloadingWaiting()
    {
        animator.SetBool("isReload", true);
        yield return new WaitForSeconds(5 * Time.deltaTime);
        animator.SetBool("isReload", false);

    }
    
    void OnReload(InputAction.CallbackContext context)
    {
        _isReloadPressed = context.ReadValueAsButton();
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
