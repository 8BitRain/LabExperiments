using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using DG.Tweening;

public class PlayAnimation : Action
{
    private Transform currentTarget;
    public string animationName;
    public float animationTime;

    private bool endAnimation = false;
    public override TaskStatus OnUpdate()
    {
        //Return a task status of success once idle animation has been started
        if(!endAnimation)
        {
            transform.GetComponent<AnimationController>().ChangeAnimationState(transform.GetComponent<Animator>(),animationName);
            DOVirtual.DelayedCall(animationTime, () => {
                endAnimation = true;
            });
        }

        if(endAnimation)
        {
            return TaskStatus.Success;
        }
        return TaskStatus.Running;
    }

    public override void OnEnd()
    {
        endAnimation = false;
    }
}