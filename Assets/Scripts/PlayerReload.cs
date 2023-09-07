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
        if (_isReloadPressed && bulletClip != 30)
        {
            animator.SetBool("isReload", true);
            Player.GetComponent<PlayerShoot>()._canShoot = false;

        }
        else if (bulletClip == 0 && bulletAmmo > 0)
        {
            animator.SetBool("isReload", true);
            Player.GetComponent<PlayerShoot>()._canShoot = false;
        }
    }
    
    
    void ReloadAnimationEvent()
    {
        Reload();
        // if (bulletClip != 30)
        // {
        //     StartCoroutine(IsReloadingWaiting());
        // }
    }
    
    void Reload()
    {
        if (bulletClip == 0 && bulletAmmo > 0)
        {
            Debug.Log("Bullet is 0 in Clip. Reloading");
            bulletClip = bulletClipCapacity;
            bulletAmmo -= bulletClipCapacity;
        }
        if (bulletClipCapacity > bulletClip && bulletClip > 0 && bulletAmmo > 0)
        {
            bulletDiff = bulletClipCapacity - bulletClip;
            if (bulletDiff > bulletAmmo)
            {
                bulletDiff = bulletAmmo;
            }
            bulletAmmo -= bulletDiff;
            bulletClip += bulletDiff;
        }
        
        animator.SetBool("isReload", false);
        Player.GetComponent<PlayerShoot>()._canShoot = true;

    }

    // IEnumerator IsReloadingWaiting()
    // {
    //     animator.SetBool("isReload", true);
    //     Reload();
    //     yield return new WaitForSeconds(2.5f);
    //     animator.SetBool("isReload", false);
    // }
    
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
