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

    public MeleeAttackBase lightAttack;
    public MeleeAttackBase[] lightAttacks;
    public MeleeAttackBase heavyAttack;

    private MeleeAttackBase lightAttackInstance;
    private MeleeAttackBase heavyAttackInstance;

    private int lightAttackState = 0;

    private Tween comboInputDelay;
    private bool playerInputCanInterruptCombo = false;

    public enum MeleeAttackType
    {
        Light,
        Heavy
    }

    /// <summary>Vector2 action for pressing a face button </summary>
    [Tooltip("Vector2 action for light attack Button ")]
    public InputActionReference lightAttackButtonPressed;

    /// <summary>Vector2 action for pressing a face button </summary>
    [Tooltip("Vector2 action for heavy attack Button ")]
    public InputActionReference heavyAttackButtonPressed;

    public static event Action<GameObject, float> onStaminaStatusChange;

    void Update()
    {
        if(lightAttackButtonPressed != null && heavyAttackButtonPressed != null)
        {
            GetPlayerInput();
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
        DOTween.Kill(comboInputDelay);
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
        if(Vector3.Distance(transform.position, target.position) <= 10)
        {
            PerformMelee(meleeAttackType);
        }
        else
        {
            this.GetComponent<Animator>().SetBool("Attacking", true);
            transform.LookAt(target);
            this.GetComponent<PlayerMovementController>().DisableMovement();
            this.GetComponent<AnimationController>().ChangeAnimationState(this.GetComponent<Animator>(), "DashIn");
            transform.DOMove(target.position + target.forward*10, .5f).OnComplete(() => {
                PerformMelee(meleeAttackType);
            });
        }
    }
}