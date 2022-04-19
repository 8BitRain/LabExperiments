using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;
using System;
using UnityEngine.UI;
using UnityEngine;

public class MeleeAttackController : MonoBehaviour
{
    public Transform Agent;
    public Transform meleeSpawn;

    public MeleeAttackBase lightAttack;
    public MeleeAttackBase heavyAttack;

    private MeleeAttackBase lightAttackInstance;
    private MeleeAttackBase heavyAttackInstance;

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
        }

        if(heavyAttackButtonPressed.action.triggered && heavyAttackInstance == null)
        {
            PerformMelee(MeleeAttackType.Heavy);
        }
    }

    public void PerformMelee(MeleeAttackType meleeAttackType)
    {
        switch (meleeAttackType)
        {
            case MeleeAttackType.Light:
                lightAttackInstance = Instantiate(lightAttack);
                InitializeAbility(lightAttackInstance);
                lightAttackInstance.Melee();
                return;
            case MeleeAttackType.Heavy:
                heavyAttackInstance = Instantiate(heavyAttack);
                InitializeAbility(heavyAttackInstance);
                return;
            default:
                break;
        }
    }

    public void InitializeAbility(MeleeAttackBase meleeAttackBase)
    {
        meleeAttackBase.SetPlayerAnimationController(this.gameObject.GetComponent<AnimationController>());
        meleeAttackBase.SetPlayerReference(this.gameObject);
        meleeAttackBase.SetMeleeSpawnPoint(this.meleeSpawn);
    }

}