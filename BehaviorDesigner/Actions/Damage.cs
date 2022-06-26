using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

public class Damage : Action
{
    public string animationName;
    private Transform currentTarget;

    public override TaskStatus OnUpdate()
    {
        return TaskStatus.Success;
    }
}