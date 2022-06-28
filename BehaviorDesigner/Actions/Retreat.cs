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

    private float distanceFromTarget;
    private Vector3 retreatVector;
    private Tween retreatTween;
    private bool movementComplete = false;

    public override void OnStart()
    {
        animator = this.GetComponent<Animator>();
        this.GetComponent<AnimationController>().ChangeAnimationState(animator, retreatAnimation);
        animator.SetBool("Retreating", true);
        Debug.Log("Retreating");

        //retreatVector = new Vector3(Random.Range(7,15), 0, (stoppingDistance - distanceFromTarget) + 5);

        var currentGameObject = GetDefaultGameObject(targetGameObject.Value);
        currentTarget = currentGameObject.GetComponent<Transform>();
        distanceFromTarget = Vector3.Distance(transform.position, currentTarget.position);

        /* Finding a vector based on vibes*/
        //retreatVector = new Vector3(currentTarget.position.x, 0, currentTarget.position.z + (stoppingDistance - distanceFromTarget) + 5);

        /* Finding a vector given an angle and direction vector*/
        //retreatVector = new Vector3(distanceFromTarget* Mathf.Cos(180), 0, distanceFromTarget*Mathf.Cos(0));

        /* Determining a vector based on opposite direction from player */
        //Turn around first
        transform.LookAt(-transform.forward + transform.position);
        retreatVector = transform.forward * Mathf.Abs((stoppingDistance - distanceFromTarget) + 5) + transform.position;

        /* Look at vector we are retreating to*/
        //transform.LookAt(retreatVector);
        //transform.LookAt(currentTarget.position + retreatVector - transform.forward);
    }

    public override TaskStatus OnUpdate()
    {
        var currentGameObject = GetDefaultGameObject(targetGameObject.Value);
        currentTarget = currentGameObject.GetComponent<Transform>();
        
        distanceFromTarget = Vector3.Distance(transform.position, currentTarget.position);
        // Return a task status of failure once we've reached the target
        //TODO ensure 10.0f is replaced with a variable that represents withinSightDistance
        if (distanceFromTarget >= stoppingDistance || movementComplete) {
            animator.SetBool("Retreating", false);
            return TaskStatus.Failure;
        }
        // We haven't reached the furthest range from the target yet so keep moving away from it
        //transform.position = Vector3.MoveTowards(transform.position, retreatVector, speed * Time.deltaTime);
        retreatVector = transform.forward * Mathf.Abs((stoppingDistance - distanceFromTarget) + 5) + transform.position;
        if(retreatTween == null)
        {
            retreatTween = transform.DOMove(retreatVector, 1f).OnComplete(() => {
                //We delete the tween once we've completed. This can cause looping issues with the AI unit returning to the start position of the tween, because the tween is killed.
                retreatTween = null;
            });
        }
        //transform.LookAt(retreatVector);
        
        return TaskStatus.Running;
    }
}