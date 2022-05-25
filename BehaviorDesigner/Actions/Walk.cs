using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using DG.Tweening;

public class Walk : Action
{
    public enum Direction
    {
        Forward,
        Backward,
        Left,
        Right,
    }

    private Direction walkDirection;
    // The transform that the object is targeting
    public SharedGameObject targetGameObject;

    public float movementDistance;
    public SharedFloat walkTime;
    private Transform currentTarget;

    public override TaskStatus OnUpdate()
    {
        var currentGameObject = GetDefaultGameObject(targetGameObject.Value);
        var walkTimeInstance = walkTime.Value;
        currentTarget = currentGameObject.GetComponent<Transform>();

        transform.LookAt(currentTarget);

        ChooseRandomWalkDirection();
        switch (walkDirection)
        {
            case Direction.Forward:
                transform.DOMove(transform.position + transform.forward * movementDistance, walkTimeInstance);
                transform.GetComponent<AnimationController>().ChangeAnimationState(transform.GetComponent<Animator>(),"Mvm_Walk_Front");
                break;
            case Direction.Backward:
                transform.DOMove(transform.position + -transform.forward * movementDistance, walkTimeInstance);
                transform.GetComponent<AnimationController>().ChangeAnimationState(transform.GetComponent<Animator>(),"Mvm_Walk_Back");
                break;
            case Direction.Left:
                transform.DOMove(transform.position + -transform.right * movementDistance, walkTimeInstance);
                transform.GetComponent<AnimationController>().ChangeAnimationState(transform.GetComponent<Animator>(),"Mvm_Walk_Back_L");
                break;
            case Direction.Right:
                transform.DOMove(transform.position + transform.right * movementDistance, walkTimeInstance);
                transform.GetComponent<AnimationController>().ChangeAnimationState(transform.GetComponent<Animator>(),"Mvm_Walk_Back_R");
                break;
            default:
                break;
        }

        return TaskStatus.Success;
    }

    public void ChooseRandomWalkDirection()
    {
        int choice = Random.Range(0,3);
        switch (choice)
        {
            case 0:
                walkDirection = Direction.Forward;
                break;
            case 1:
                walkDirection = Direction.Backward;
                break;
            case 2:
                walkDirection = Direction.Left;
                break;
            case 3:
                walkDirection = Direction.Right; 
                break;
            default:
                break;
        }
    }
}