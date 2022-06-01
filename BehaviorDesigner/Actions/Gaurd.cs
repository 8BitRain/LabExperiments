using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

public class Gaurd : Action
{
    private Transform currentTarget;

    public override TaskStatus OnUpdate()
    {
        //Return a task status of success once gaurd animation state has changed
        this.GetComponent<Animator>().SetBool("Gaurding", true);
        transform.GetComponent<AnimationController>().ChangeAnimationState(transform.GetComponent<Animator>(),"Gaurd");
        this.GetComponent<Body>().DisplayVFXBlock();

        //this.GetComponent<Body>().GetHurtBox().enabled = false;
        return TaskStatus.Success;
    }
}