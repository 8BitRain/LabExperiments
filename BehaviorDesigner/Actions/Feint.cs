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
        Stationary
    }

    public FeintDirection feintDirection;
    // The transform that the object is targeting
    public SharedGameObject targetGameObject;
    public string animationName;

    public float movementDistance;
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
                transform.DOMove(transform.position + transform.forward * movementDistance, 1);
                break;
            case FeintDirection.Backward:
                transform.DOMove(transform.position + -transform.forward * movementDistance, 1);
                break;
            case FeintDirection.Left:
                transform.DOMove(transform.position + -transform.right * movementDistance, 1);
                break;
            case FeintDirection.Right:
                transform.DOMove(transform.position + transform.right * movementDistance, 1);
                break;
            case FeintDirection.Stationary:
                break;
            default:
                break;
        }

        transform.GetComponent<AnimationController>().ChangeAnimationState(transform.GetComponent<Animator>(),animationName);
        return TaskStatus.Success;
    }
}