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
            if(this.GetComponent<TargetingController>().targets.Length > 0)
                DashIn(this.GetComponent<TargetingController>().targets[0], MeleeAttackType.Light);
            else
                PerformMelee(MeleeAttackType.Light);
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
        if(this.GetComponent<Status>().stamina <= 0)
        {
            return;
        }

        this.GetComponent<Status>().SetStamina(this.gameObject, 10f);
        
        this.GetComponent<Animator>().SetBool("Attacking", true);
        switch (meleeAttackType)
        {
            case MeleeAttackType.Light:
                if(!isAIAgent)
                {
                    targetingController.FreeFlowTargetLock();
                }
                lightAttackInstance = Instantiate(lightAttacks[lightAttackState]);
                InitializeAbility(lightAttackInstance);
                lightAttackInstance.Melee();
                UpdateLightAttackState();
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

    public void DestroyAttackInstance()
    {
        if(lightAttackInstance != null)
            Destroy(lightAttackInstance);
        if(heavyAttackInstance != null)
            Destroy(heavyAttackInstance);
    }
}