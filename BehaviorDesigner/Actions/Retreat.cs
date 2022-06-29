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
    private Tweener retreatTweener;
    private bool movementComplete = false;

    public override void OnStart()
    {
        animator = this.GetComponent<Animator>();
        this.GetComponent<AnimationController>().ChangeAnimationState(animator, retreatAnimation);
        animator.SetBool("Retreating", true);
        

        var currentGameObject = GetDefaultGameObject(targetGameObject.Value);
        currentTarget = currentGameObject.GetComponent<Transform>();
        distanceFromTarget = Vector3.Distance(transform.position, currentTarget.position);

        /* Determining a vector based on opposite direction from player */
        //Turn around first
        transform.LookAt(-transform.forward + transform.position);
        retreatVector = transform.forward * Mathf.Abs((stoppingDistance - distanceFromTarget) + 5) + transform.position;

        Debug.Log("Retreat Logic: Starting Retreat" + " retreat vector is: " + "(" + retreatVector.x + "," + retreatVector.y + "," + retreatVector.z + ")");
    }

    public override TaskStatus OnUpdate()
    {
        distanceFromTarget = Vector3.Distance(transform.position, currentTarget.position);
        
        //Return a task status of sucess once we've reached the target
        //TODO ensure stoppingDistance references a global variable, specifically the variable that represents withinSightDistance
        if (distanceFromTarget >= stoppingDistance || movementComplete) {
            animator.SetBool("Retreating", false);
            Debug.Log("Retreat Logic: Reached target destination");
            retreatTween = null;
            return TaskStatus.Success;
        }
        else
        {
            //Update retreatVector;
            retreatVector = transform.forward * Mathf.Abs((stoppingDistance - distanceFromTarget) + 5) + transform.position;

            //Update retreatTween start and end positions
            if(retreatTween != null)
            {
                Tweener retreatTweener = (Tweener)retreatTween;
                retreatTweener.ChangeValues(transform.position, retreatVector);
            }

        }
        
        if(retreatTween == null)
        {
            Debug.Log("Retreat Logic: Retreat tween is null so we can set a new tween");
            retreatTween = transform.DOMove(retreatVector, 1f).OnComplete(() => {
                Debug.Log("Retreat Logic: Retreat Tween is complete");
            }).OnKill(() => {
                Debug.Log("Retreat Logic: Retreat Tween is killed");
            });
        }
        
        return TaskStatus.Running;
    }

    public override void OnEnd()
    {
        Debug.Log("Retreat Logic: Loop ended");
    }
}