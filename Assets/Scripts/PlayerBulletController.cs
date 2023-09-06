using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class PlayerBulletController : MonoBehaviour
{
    #region Setup

    private PlayerControl _playerInput;
    [SerializeField] private GameObject bulletDecal;
    [SerializeField] private GameObject Player;
    private Animator animator;

    #endregion

    #region Bullet
    [Header("Bullet Settings")]
    
    [SerializeField] private float bulletSpeed = 100f;
    [SerializeField] private float timeToDestroy = 1f;
    
    public Vector3 target { get; set; }
    public bool hit { get; set; }

    #endregion

    private void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, target, bulletSpeed * Time.fixedDeltaTime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("TargetBoard"))
        {
            ContactPoint contactPoint = collision.GetContact(0);
            Vector3 decalPosition = contactPoint.point + contactPoint.normal * .01f;
            GameObject decal = GameObject.Instantiate(bulletDecal, decalPosition, Quaternion.LookRotation(contactPoint.normal));
            
            Destroy(gameObject);
        }
        else if (collision.gameObject.CompareTag("Enemy"))
        {
            
        }
        
        Debug.Log("Temas edilen nesnenin tagi: " + collision.gameObject.tag);
    }

    private void OnEnable()
    {
        Destroy(gameObject, timeToDestroy);
    }
}
