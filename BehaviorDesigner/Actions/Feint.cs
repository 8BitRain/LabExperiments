using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using DG.Tweening;

public class Feint : Action
{
    public enum FeintDirection
    {
        Forward,
        Backward,
        Left,
        Right,
        Stationary,

        Random
    }

    public FeintDirection feintDirection;
    // The transform that the object is targeting
    public SharedGameObject targetGameObject;
    public string animationName;

    public float movementDistance;

    public float time = 1;
    private Transform currentTarget;

    public override TaskStatus OnUpdate()
    {
        var currentGameObject = GetDefaultGameObject(targetGameObject.Value);
        currentTarget = currentGameObject.GetComponent<Transform>();

        transform.LookAt(currentTarget);

        //Return a task status of success once feint animation has started
        switch (feintDirection)
        {
            case FeintDirection.Forward:
                transform.DOMove(transform.position + transform.forward * movementDistance, time);
                break;
            case FeintDirection.Backward:
                transform.DOMove(transform.position + -transform.forward * movementDistance, time);
                break;
            case FeintDirection.Left:
                transform.DOMove(transform.position + -transform.right * movementDistance, time);
                break;
            case FeintDirection.Right:
                transform.DOMove(transform.position + transform.right * movementDistance, time);
                break;
            case FeintDirection.Stationary:
                break;
            case FeintDirection.Random:
                chooseDirection();
                break;
            default:
                break;
        }

        transform.GetComponent<AnimationController>().ChangeAnimationState(transform.GetComponent<Animator>(),animationName);
        return TaskStatus.Success;
    }

    public void chooseDirection()
    {
        int randomNum = Random.Range(0, 100);
        
        if(randomNum%2 == 0)
        {
            transform.DOMove(transform.position + -transform.right * movementDistance, 1);
        }
        else
        {
            transform.DOMove(transform.position + transform.right * movementDistance, 1);
        }
    }
}