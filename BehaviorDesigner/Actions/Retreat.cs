using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using DG.Tweening;

public class Retreat : Action
{
    // The speed of the object
    public float speed = 0; 
    // The transform that the object is moving towards
    public SharedGameObject targetGameObject;

    private Transform currentTarget;
    private Animator animator;
    public string retreatAnimation;
    public float stoppingDistance;

    public override void OnStart()
    {
        animator = this.GetComponent<Animator>();
        this.GetComponent<AnimationController>().ChangeAnimationState(animator, retreatAnimation);
        animator.SetBool("Retreating", true);
    }

    public override TaskStatus OnUpdate()
    {
        var currentGameObject = GetDefaultGameObject(targetGameObject.Value);
        currentTarget = currentGameObject.GetComponent<Transform>();
        
        float distanceFromTarget = Vector3.Distance(transform.position, currentTarget.position);
        // Return a task status of failure once we've reached the target
        //TODO ensure 10.0f is replaced with a variable that represents withinSightDistance
        if (distanceFromTarget >= stoppingDistance) {
            animator.SetBool("Retreating", false);
            return TaskStatus.Failure;
        }
        // We haven't reached the furthest range from the target yet so keep moving away from it
        transform.position = Vector3.MoveTowards(transform.position, currentTarget.position - currentTarget.transform.forward*(stoppingDistance - distanceFromTarget) , speed * Time.deltaTime);
        
        return TaskStatus.Running;
    }
}