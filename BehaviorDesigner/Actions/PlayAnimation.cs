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
    public override void OnStart()
    {
        endAnimation = false;
    }
    public override TaskStatus OnUpdate()
    {

        Debug.Log("endAnimation Value: " + endAnimation);
        //Return a task status of success once idle animation has been started
        if(!endAnimation)
        {
            transform.GetComponent<AnimationController>().ChangeAnimationState(transform.GetComponent<Animator>(),animationName);
            DOVirtual.DelayedCall(animationTime, () => {
                endAnimation = true;
            }, false);

            //If we can play a seperate animation layer, lets select the index, set the speed 
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