using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

public class GaurdConditional : Conditional
{
        // The transform that the object is moving towards
    public SharedGameObject targetGameObject;

    private Transform currentTarget;

    public override TaskStatus OnUpdate()
    {
        var currentGameObject = GetDefaultGameObject(targetGameObject.Value);
        currentTarget = currentGameObject.GetComponent<Transform>();

        if(currentTarget.GetComponent<Animator>().GetBool("Attacking"))
        {
            return TaskStatus.Success;
        }
        else
        {
            return TaskStatus.Failure;
        }
    }
}
