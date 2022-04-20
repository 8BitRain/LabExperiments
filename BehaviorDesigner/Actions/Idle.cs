using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

public class Idle : Action
{
    private Transform currentTarget;

    public override TaskStatus OnUpdate()
    {
        //Return a task status of success once idle animation has been started
        transform.GetComponent<AnimationController>().ChangeAnimationState(transform.GetComponent<Animator>(),"Player_idle");
        return TaskStatus.Success;
    }
}