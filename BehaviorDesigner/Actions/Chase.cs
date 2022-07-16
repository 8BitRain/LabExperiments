using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine.AI;

public class Chase : Action
{
    // The speed of the object
    public float speed = 0; 
    // The transform that the object is moving towards
    public SharedGameObject targetGameObject;

    private Transform currentTarget;
    private Animator animator;
    private NavMeshAgent navMeshAgent;
    public string chaseAnimation;

    public override void OnStart()
    {
        animator = this.GetComponent<Animator>();
        this.GetComponent<AnimationController>().ChangeAnimationState(animator, chaseAnimation);
        animator.SetBool("Chasing", true);
        navMeshAgent = this.GetComponent<NavMeshAgent>();
        //navMeshAgent.isStopped = false;
    }

    public override TaskStatus OnUpdate()
    {
        //this.GetComponent<AnimationController>().ChangeAnimationState(animator, chaseAnimation);
        var currentGameObject = GetDefaultGameObject(targetGameObject.Value);
        currentTarget = currentGameObject.GetComponent<Transform>();

        //TODO ensure 10.0f is replaced with a variable that represents withinSightDistance
        if(navMeshAgent == null)
        {
            // Return a task status of failure once we've reached the target
            if (Vector3.Distance(transform.position, currentTarget.position) <= 10f) {
                animator.SetBool("Chasing", false);
                return TaskStatus.Failure;
            }

            // We haven't reached the target yet so keep moving towards it
            transform.position = Vector3.MoveTowards(transform.position, currentTarget.position, speed * Time.deltaTime);

            return TaskStatus.Running;
        }
        else
        {
            //logic for using navMesh. It takes into account a stopping distance
            bool reachedDestination = navMeshAgent.SetDestination(currentTarget.position);
            if(reachedDestination)
            {
                animator.SetBool("Chasing", false);
            }
            return reachedDestination ? TaskStatus.Running : TaskStatus.Failure;
        }
    }
}