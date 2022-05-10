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
            PerformMelee(MeleeAttackType.Light);
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

        if(heavyAttackButtonPressed.action.triggered && heavyAttackInstance == null)
        {
            PerformMelee(MeleeAttackType.Heavy);
            return;
        }
    }

    public void PerformMelee(MeleeAttackType meleeAttackType)
    {
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
}