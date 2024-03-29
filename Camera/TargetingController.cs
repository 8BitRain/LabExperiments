﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using System;
using UnityEngine;
using DG.Tweening;

public class TargetingController : MonoBehaviour
{
    [Header("Targeting Input")]
    //public InputActionReference lockOnInput;
    //public InputActionReference targetSwitchRightInput;
    //public InputActionReference targetSwitchLeftInput;

    [Header("Player Configurations")]
    public Transform Player;

    [Header("Targeting Input")]
    private bool lockOnInput = false;
    private bool targetSwitchRightInput = false;
    private bool targetSwitchLeftInput = false;

    [Header("Target List")]
    public LayerMask Target; 

    [Header("Camera Settings")]
    public CameraSettings cameraSettings;

    public Transform[] targets;

    //Timer
    private float _swapTimer = 0;

    private bool lockedOn = false;
    private Transform targetToLock;
    private int currentTarget = 0;

    private Tween ambientTargetLock;

    //Events
    public static event Action<GameObject, GameObject, Camera> onUpdateTargetArrowPosition;
    public static event Action<GameObject> onEnableTargetArrow;
    public static event Action<GameObject> onDisableTargetArrow;
    public static event Action<GameObject, Transform, CameraSettings> onEnableLockOnCamera;
    public static event Action<GameObject> onDisableLockOnCamera;
    public static event Action<GameObject, Transform> onUpdateLockOnCameraTarget;


    void Start()
    {
        Player = this.gameObject.transform;
    }

    public void OnLockOn(InputAction.CallbackContext ctx) => lockOnInput = ctx.ReadValueAsButton();
    public void OnLockOnSwitchTargetRight(InputAction.CallbackContext ctx) => targetSwitchRightInput = ctx.ReadValueAsButton();
    public void OnLockOnSwitchTargetLeft(InputAction.CallbackContext ctx) => targetSwitchLeftInput = ctx.ReadValueAsButton();

    void Update()
    {

        //Check for nearby enemies within a range of 30m.
        //TODO instead of calling this here, we really want this logic triggered anytime a player is about to perform a melee attack. We could  create a function
        //for this logic here, then call it in the MeleeAttackController!
        /*if(!lockedOn)
        {
            if(ambientTargetLock == null)
            {
                ambientTargetLock = DOVirtual.DelayedCall(5.0f, () => {
                    FindNearbyTargets();
                    ambientTargetLock = null;
                });
            }
        }*/

        //Enemy is defeated while locked on
        if(lockedOn)
        {
            if(targets[0] == null)
            {
                TargetLockOff();
            }

            if(targets[0].TryGetComponent<Status>(out Status status))
            {
                if(status.hp <= 0)
                {
                    //We can either switch to the next target nearby or exit target lock
                    Debug.Log("TargetLock: Current target defeated. Lock off or switch target");
                    //Remove first target
                    targets = RemoveAt(targets, 0);

                    if(targets.Length > 0)
                    {
                        currentTarget = 0;
                    }
                    else
                    {
                        TargetLockOff();
                    }
                }
            }
        }

        //Turn off LockOn
        if(lockOnInput && lockedOn)
        {
            //Experimental Add widescreen bars
            //GameManager gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
            /*if(gameManager.canDisplayWidescreenUI)
            {
                gameManager.disableWidescreenBars(playerScriptReference.GetPlayerID());
            }*/

            TargetLockOff();
        }

        //LockOn to Target
        if(lockOnInput && !lockedOn)
        {
            ResetAmbientTargetLock();
            //Look for surrounding enemies
            bool targetsFound = FindNearbyTargets();
            if(targetsFound)
            {
                //Experimental Add widescreen bars
                //TODO enable gameManager
                //GameManager gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
                /*if(gameManager.canDisplayWidescreenUI)
                {
                    gameManager.enableWidescreenBars(this.GetPlayerID());
                }*/

                print("found targets, locking on");
                

                //reset currentTarget 
                currentTarget = 0;
                targetToLock = targets[currentTarget];

                //Target lock set to TargetLockPosition of target
                //Transform targetToLockHead = targetToLock.GetComponent<Body>().TargetLock;
                Transform targetToLockHead = targetToLock;
                print("Target to lock: " + targetToLockHead);

                //Activate Lock On Camera
                onEnableLockOnCamera.Invoke(this.gameObject, targetToLockHead, cameraSettings);

                //Activate & Adjust position of targeter
                onEnableTargetArrow.Invoke(this.gameObject);

                //How can I grab the camera reference?
                onUpdateTargetArrowPosition.Invoke(this.gameObject, targetToLockHead.gameObject, GetComponent<CameraController>().GetCameraInstance().GetComponent<Camera>());

                lockedOn = true;
                lockOnInput = false;
            }
            

        }

        //Swap to Right/Left Target
        if(lockedOn)
        {
            if(targetSwitchRightInput && _swapTimer >= .2f)
            {
                if(currentTarget >= targets.Length - 1)
                {
                    currentTarget = 0;
                }
                else if(currentTarget < targets.Length - 1)
                {
                    currentTarget += 1;
                }
                //currentTarget += 1;
                print("locked on right");
                targetSwitchRightInput = false;
                targetSwitchLeftInput = false;
                _swapTimer = 0;
            }

            if(targetSwitchLeftInput && _swapTimer >= .2f)
            {
                //An else statement here might be more beneficial since placing the >= after the < would cause an error in which if currentTarget.length = 3 and currentTarget = 2, currentTarget++ would increment currentTarget to 3, then trigger setting currentTarget to 0, creating an off by 1 error.
                //currentTarget -=1;
                if(currentTarget > 0)
                {
                    currentTarget -= 1;
                }
                else if(currentTarget == 0)
                {
                    currentTarget = targets.Length - 1;
                }
                targetSwitchLeftInput = false;
                targetSwitchRightInput = false;
                print("locked on left");
                _swapTimer = 0;
            }

            _swapTimer += Time.deltaTime;

            targetToLock = targets[currentTarget];

            Transform targetToLockHead;
            targetToLockHead = targetToLock;

            //Update target to lock

            if(targetToLockHead != null)
            {
                //playerScriptReference.UpdateDynamicTargetLock(targetToLockHead);
                onUpdateLockOnCameraTarget.Invoke(this.gameObject, targetToLockHead);
                //EngageDynamicTargetLock(targetToLockHead);
                // Enable GameManager
                //GameManager gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
                //gameManager.updateTargetPosition(playerScriptReference.GetPlayerID(), targetToLockHead.gameObject);
                onUpdateTargetArrowPosition.Invoke(this.gameObject, targetToLockHead.gameObject, GetComponent<CameraController>().GetCameraInstance().GetComponent<Camera>());
            }
        }
    }

    public void TargetLockOff()
    {
        //Disable Target Arrow
        onDisableTargetArrow.Invoke(this.gameObject);

        lockedOn = false;
        //playerScriptReference.DisengageDynamicTargetLock();
        onDisableLockOnCamera.Invoke(this.gameObject);

        lockOnInput = false;

        //Set currentTarget value to sentinel value & Reset targets array
        currentTarget = -1;
        targets = new Transform[0];

        print("Lock off");
    }

    //Remove value from array
    public Transform[] RemoveAt<Transform>(Transform[] source, int index)
    {
        Transform[] dest = new Transform[source.Length - 1];
        if( index > 0 )
            Array.Copy(source, 0, dest, 0, index);

        if( index < source.Length - 1 )
            Array.Copy(source, index + 1, dest, index, source.Length - index - 1);

        return dest;
    }
    
    public bool GetTargetLockStatus()
    {
        return this.lockedOn;
    }

    public void FreeFlowTargetLock()
    {
        if(this.lockedOn == false)
            FindNearbyTargets();
    }

    //Resets the ability to lock on to ambient targets. Used for freeflow combat.
    public void ResetAmbientTargetLock()
    { 
        if(ambientTargetLock != null)
        {
            ambientTargetLock.Kill();
            ambientTargetLock = null;
        }
    }

    public GameObject GetCurrentTarget()
    {
        if(currentTarget == -1)
            return null;

        try
        {
            print("Current Target" + targets[currentTarget].name);
            return targetToLock.gameObject; 
        }
        catch (System.Exception)
        {
            return null;
        }
    }

    public bool FindNearbyTargets()
    {
        Debug.Log("Find nearby targets");
        Vector3 position = Player.transform.position + this.GetComponent<CharacterController>().center;
        RaycastHit[] targetHit = Physics.SphereCastAll(position, 30f, transform.forward, 30f, Target);

        //This sort method compares the magnitude (distance) of target to the player. The closer the enemy, the sooner we want to target
        System.Array.Sort(targetHit, (x,y) => ((int)(x.transform.position - transform.position).magnitude).CompareTo((int)(y.transform.position - transform.position).magnitude));

        bool foundTargets = false;

        //Create an array based on the number of targets around us;
        targets = new Transform[targetHit.Length];
        if(targetHit.Length > 0)
        {
            int iter = 0;
            foundTargets = true;

            foreach(RaycastHit target in targetHit)
            {
                print(target.transform.name + " : " + target.distance);

                //Check if target has a status object
                if(target.transform.gameObject.TryGetComponent<Status>(out Status status))
                {
                    //Only add target to target list if their hp is above 100. This prevents targeting defeated AI units
                    if(status.hp <= 0)
                    {
                        iter++;
                    }
                    else
                    {
                        targets[iter] = target.transform;
                        iter++;
                    }
                }
                else
                {
                    targets[iter] = target.transform;
                    iter++;
                }
                //Problem with rigidbodies read here https://forum.unity.com/threads/raycast-hit-rigidbody-object-instead-of-collider.544297/
                /*if(target.rigidbody != null)
                {
                    print(target.rigidbody.transform);
                    targets[iter] = target.rigidbody.transform;
                } 
                else
                {
                    targets[iter] = target.transform;
                }*/
                //print(target.GetType().ToString());
            }
        }

        if(targets.Length > 0)
        {
            if(targets[0] == null)
            {
                Debug.Log("TargetController: There are no targets");
                targets = new Transform[0];
                return false;
            }
        }

        return foundTargets;
    }
}
