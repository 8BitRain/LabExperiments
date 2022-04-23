using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

public class Knockdown : Action
{
    public string animationName;
    private Transform currentTarget;

    public override TaskStatus OnUpdate()
    {
        //Return a task status of success once idle animation has been started
        transform.GetComponent<AnimationController>().ChangeAnimationState(transform.GetComponent<Animator>(),animationName);
        transform.GetComponent<Animator>().SetBool("Wakeup", false);
        return TaskStatus.Success;
    }
}