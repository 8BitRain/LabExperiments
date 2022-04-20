using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using DG.Tweening;

public class TakingDamage : Conditional
{

    public float takeDamageTime;
    public override TaskStatus OnUpdate()
    {
        //IsTakingDamage(this.transform) ? TaskStatus.Success : TaskStatus.Running;
        if(IsTakingDamage(this.transform))
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
    public bool IsTakingDamage(Transform target)
    {
        return this.GetComponent<Animator>().GetBool("Damaged");
    }   
}