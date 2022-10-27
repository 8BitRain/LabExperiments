using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;
using System;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine;

public class MeleeAttackController : MonoBehaviour
{
    public Transform Agent;
    public Transform meleeSpawn;
    public bool isAIAgent = false;
    [Header("Reticle Settings")]
    public FPSReticle reticle;

    [Header("Melee Settings")]
    public MeleeAttackBase lightAttack;
    public MeleeAttackBase[] lightAttacks;
    public MeleeAttackBase heavyAttack;
    
    public enum MeleeAttackType
    {
        Light,
        Heavy
    }

    //TODO add editor logic to hide this menu for AI agents.
    [Header("DashIn Settings")]
    public float dashInTimeConstant;
    public float dashInStoppingDistance;
    public float dashInAttackRange;
    public float dashInMinDistance;
    public float dashInMaxDistance;

    private MeleeAttackBase lightAttackInstance;
    private MeleeAttackBase heavyAttackInstance;

    private int lightAttackState = 0;

    private Tween comboInputDelay;
    private bool playerInputCanInterruptCombo = false;
    private Tween dashInTween;
    private bool isDashingIn = false;

    private TargetingController targetingController;


    [Header("Input Settings")]
    /// <summary>Vector2 action for pressing a face button </summary>
    [Tooltip("Vector2 action for light attack Button ")]
    public InputActionReference lightAttackButtonPressed;

    /// <summary>Vector2 action for pressing a face button </summary>
    [Tooltip("Vector2 action for heavy attack Button ")]
    public InputActionReference heavyAttackButtonPressed;

    void Start()
    {
        targetingController = GetComponent<TargetingController>();
    }

    void Update()
    {
        if(lightAttackButtonPressed != null && heavyAttackButtonPressed != null)
        {
            GetPlayerInput();
        }

        if(isDashingIn)
        {
            transform.LookAt(targetingController.targets[0]);
        }

    }

    public void GetPlayerInput()
    {
        if(lightAttackButtonPressed.action.triggered && lightAttackInstance == null)
        {
            //Dash in
            if(this.GetComponent<TargetingController>().targets == null)
            {
                return;
            }

            if(this.GetComponent<TargetingController>().targets.Length > 0)
            {
                try
                {
                    DashIn(this.GetComponent<TargetingController>().targets[0], MeleeAttackType.Light);
                }
                catch (System.Exception e)
                {
                    PerformMelee(MeleeAttackType.Light);
                    Debug.LogError("Caught an error where DashIn did something naughty >_<...: " + e);
                }
            }
            else
            {
                PerformMelee(MeleeAttackType.Light);
            }
            return;
        }

        if(heavyAttackButtonPressed.action.triggered && heavyAttackInstance == null)
        {
            PerformMelee(MeleeAttackType.Heavy);
            return;
        }

        //Logic for continuing a combo on hit, cancelling hitstop
        if(lightAttackButtonPressed.action.triggered && lightAttackInstance != null && GetPlayerInputCanInterruptCombo())
        {
            lightAttackInstance.CancelHitStop();
            PerformMelee(MeleeAttackType.Light);
            SetPlayerInputCanInterruptCombo(false);
            return;
        }

        if(heavyAttackButtonPressed.action.triggered && heavyAttackInstance != null && GetPlayerInputCanInterruptCombo())
        {
            heavyAttackInstance.CancelHitStop();
            PerformMelee(MeleeAttackType.Heavy);
            SetPlayerInputCanInterruptCombo(false);
            return;
        }
    }

    public void PerformMelee(MeleeAttackType meleeAttackType)
    {
        //TODO: NUEDEAD GAME INVESTIGATE STAMINA
        if(this.GetComponent<Status>() != null)
        {
            if(this.GetComponent<Status>().stamina <= 0)
            {
                return;
            }
        }

        if(!GetIsAIAgent())
        {
            if(targetingController.GetTargetLockStatus())
                transform.LookAt(targetingController.targets[0]);
                //transform.LookAt(new Vector3(targetingController.targets[0].transform.position.x, 0, targetingController.targets[0].transform.position.z));
        }

        //TODO: NUEDEAD GAME INVESTIGATE STAMINA
        if(this.GetComponent<Status>() != null)
        {
            this.GetComponent<Status>().SetStamina(this.gameObject, 10f);
        }
        
        this.GetComponent<Animator>().SetBool("Attacking", true);
        switch (meleeAttackType)
        {
            case MeleeAttackType.Light:
                if(!isAIAgent)
                {
                    if(!targetingController.GetTargetLockStatus())
                        targetingController.FreeFlowTargetLock();
                }
                lightAttackInstance = Instantiate(lightAttacks[lightAttackState]);
                InitializeAbility(lightAttackInstance);
                lightAttackInstance.Melee();
                UpdateLightAttackState();
                DealDamageToSelf(lightAttackInstance);
                return;
            case MeleeAttackType.Heavy:
                heavyAttackInstance = Instantiate(heavyAttack);
                InitializeAbility(heavyAttackInstance);
                heavyAttackInstance.Melee();
                return;
            default:
                break;
        }
    }

    public void UpdateLightAttackState()
    {
        //If this agent is an AI entity, let's not update combo states for now
        if(GetIsAIAgent())
            return;
        comboInputDelay.Kill();
        if(lightAttackState < lightAttacks.Length - 1)
        {
            lightAttackState++;
            //Establish combatInputDelay
            float animationTime = lightAttackInstance.attackComponent.GetComponent<ModularAttackElement>().meleeAttackComponent.animationComponent.animationEndDelay;
            comboInputDelay = DOVirtual.DelayedCall(animationTime + .5f, () => {
                lightAttackState = 0;
                this.GetComponent<Animator>().SetBool("Attacking", false);
            }, default);
        }
        else
        {
            lightAttackState = 0;
            //Trying to fix bug where attacking is not set to false after the last meleeAttack
            //this.GetComponent<Animator>().SetBool("Attacking", false);
        }
        Debug.Log("Light Attack State" + lightAttackState);
    }

    public void InitializeAbility(MeleeAttackBase meleeAttackBase)
    {
        meleeAttackBase.SetPlayerAnimationController(this.gameObject.GetComponent<AnimationController>());
        meleeAttackBase.SetPlayerReference(this.gameObject);
        meleeAttackBase.SetMeleeSpawnPoint(this.meleeSpawn);
        meleeAttackBase.SetAIAgentStatus(GetIsAIAgent());
    }

    public bool GetPlayerInputCanInterruptCombo()
    {
        return playerInputCanInterruptCombo;
    }

    public void SetPlayerInputCanInterruptCombo(bool value)
    {
        playerInputCanInterruptCombo = value;
    }

    public bool GetIsAIAgent()
    {
        return isAIAgent;
    }

    public void DashIn(Transform target, MeleeAttackType meleeAttackType)
    {
        float distanceToTarget = Vector3.Distance(transform.position, target.position);
        if(distanceToTarget < dashInMinDistance || distanceToTarget > dashInMaxDistance)
        {
            PerformMelee(meleeAttackType);
        }
        /*if(Vector3.Distance(transform.position, target.position) <= dashIn)
        {
            PerformMelee(meleeAttackType);
        }*/
        else
        {
            //End all other instances of tweening on this transform, this prevents 
            transform.DOKill();
            this.GetComponent<Animator>().SetBool("Attacking", true);
            transform.LookAt(target);
            this.GetComponent<PlayerMovementController>().DisableMovement();
            this.GetComponent<AnimationController>().ChangeAnimationState(this.GetComponent<Animator>(), "DashIn");
            isDashingIn = true;

            float dashInTime = (distanceToTarget/dashInMaxDistance * dashInTimeConstant);
            dashInTween = transform.DOMove(target.position + target.forward*dashInStoppingDistance, dashInTime).SetEase(Ease.InSine).OnComplete(() => {
                PerformMelee(meleeAttackType);
                isDashingIn = false;
            });
        }
    }

    public void CancelDashIn()
    {
        if(dashInTween != null)
        {
            Debug.Log("Canceling Dash");
            dashInTween.Kill();
            isDashingIn = false;
        }
    }

    public void DealDamageToSelf(MeleeAttackBase meleeAttackBase)
    {
        //TODO for a move to do damage to oneself, we want to make sure the animation and all other logic play before destruction of
        //the game object
        //So what we can do is wait for the animation time + lifetime of spawned attack?
        if(meleeAttackBase.GetMeleeAttackComponent() != null)
        {
            if(meleeAttackBase.GetMeleeAttackComponent().collisionComponent.damageSelf == true)
            {
                if(this.GetComponent<Status>() != null)
                {
                    //TODO remove references to animationCompleteWaitTime
                    DOVirtual.DelayedCall(meleeAttackBase.GetMeleeAttackComponent().animationComponent.animationEndDelay, () => {
                        Debug.Log(this.gameObject.name + ":Self Destruct!");
                        this.GetComponent<Status>().SetHP(this.gameObject, null, null, meleeAttackBase.GetMeleeAttackComponent());
                    });
                }
            }
        }
        else
        {
            Debug.LogError(this.gameObject.name + ": attempted to Self Destruct but could not");
        }
    }

    public void DestroyAttackInstance()
    {
        if(lightAttackInstance != null)
            Destroy(lightAttackInstance);
        if(heavyAttackInstance != null)
            Destroy(heavyAttackInstance);
    }
}