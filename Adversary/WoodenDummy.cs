using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;


public class WoodenDummy : Enemy
{
    [Header("Gravity/Verticality Settings")]
    public float gravity = -9.81f;
    public bool applyGravity;
    public bool _isGrounded = false;
    public Transform _groundChecker;
    public LayerMask Ground;
    public float JumpHeight = 2f;
    private Vector3 _velocity;
    public float GroundDistance = 0.2f;


    public void Update()
    {
        RaycastHit groundedRaycast;
        _isGrounded = Physics.Raycast(_groundChecker.position, Vector3.down, out groundedRaycast, GroundDistance, Ground);
        Debug.DrawRay(_groundChecker.position, Vector3.down * GroundDistance, Color.red);
        _velocity.y = 0f;
        if(!_isGrounded)
        {
            Gravity();
        }

        //Lock Rotation
        //transform.DORotate(new Vector3(0,transform.eulerAngles.y,0),0);
        transform.rotation = Quaternion.Euler(0,transform.eulerAngles.y,0);
    }

    public void Gravity()
    {
        if(applyGravity)
        {
            _velocity.y += gravity * Time.deltaTime;
            //Getting a better jumping arc will probably be factored here
            transform.Translate(_velocity);
            //Debug.Log("Applying gravity" + _velocity.y);
        }
    }
}
