﻿using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

public class StartCombat : Action
{
    // The transform that the object is moving towards
    public SharedGameObject targetGameObject;
    public MeleeAttackController.MeleeAttackType attackType;

    private Transform currentTarget;

    public override TaskStatus OnUpdate()
    {
        var currentGameObject = GetDefaultGameObject(targetGameObject.Value);
        currentTarget = currentGameObject.GetComponent<Transform>();

        //Return a task status of success once combat action has been performed
        Debug.Log("CurrentAnimationState" + transform.GetComponent<AnimationController>().GetCurrentState());
        transform.LookAt(currentTarget);
        transform.GetComponent<MeleeAttackController>().PerformMelee(attackType);
        transform.LookAt(currentTarget);
        return TaskStatus.Success;
    }
}