using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

public class CancelGaurd : Action
{
    private Transform currentTarget;

    public override TaskStatus OnUpdate()
    {
        //Return a task status of success once gaurd animation state has changed
        this.GetComponent<Animator>().SetBool("Gaurding", false);
        this.GetComponent<Body>().HideVFXBlock();

        //this.GetComponent<Body>().GetHurtBox().enabled = true;
        return TaskStatus.Success;
    }
}