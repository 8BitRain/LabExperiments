using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

public class Chase : Action
{
    // The speed of the object
    public float speed = 0; 
    // The transform that the object is moving towards
    public SharedGameObject targetGameObject;

    private Transform currentTarget;
    private Animator animator;
    public string chaseAnimation;

    public override void OnStart()
    {
        animator = this.GetComponent<Animator>();
        this.GetComponent<AnimationController>().ChangeAnimationState(animator, chaseAnimation);
        animator.SetBool("Chasing", true);
    }

    public override TaskStatus OnUpdate()
    {
        //this.GetComponent<AnimationController>().ChangeAnimationState(animator, chaseAnimation);
        var currentGameObject = GetDefaultGameObject(targetGameObject.Value);
        currentTarget = currentGameObject.GetComponent<Transform>();
        // Return a task status of success once we've reached the target
        if (Vector3.SqrMagnitude(transform.position - currentTarget.position) < 10f) {
            animator.SetBool("Chasing", false);
            return TaskStatus.Failure;
        }
        // We haven't reached the target yet so keep moving towards it
        transform.position = Vector3.MoveTowards(transform.position, currentTarget.position, speed * Time.deltaTime);
        
        return TaskStatus.Running;
    }
}