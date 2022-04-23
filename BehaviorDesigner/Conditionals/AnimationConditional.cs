using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using DG.Tweening;

public class AnimationConditional : Conditional
{

    public string animatorBoolName;
    public override TaskStatus OnUpdate()
    {
        if(GetAnimationBool(this.transform))
        {
            return TaskStatus.Success;
        }
        else
        {
            return TaskStatus.Failure;
        }
    }   
    public override void OnEnd()
    {
        //this.GetComponent<WoodenDummy>().isColliding = false;
    }   
    // Returns true if targetTransform is within sight of current transform
    public bool GetAnimationBool(Transform target)
    {
        return this.GetComponent<Animator>().GetBool(animatorBoolName);
    }   
}