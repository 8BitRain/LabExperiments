using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine.AI;

public class Fall : Action
{

    private Animator animator;
    private NavMeshAgent navMeshAgent;
    public string fallAnimation;

    public override void OnStart()
    {
        animator = this.GetComponent<Animator>();
        this.GetComponent<AnimationController>().ChangeAnimationState(animator, fallAnimation);
        animator.SetBool("Falling", true);
        navMeshAgent = this.GetComponent<NavMeshAgent>();
        if(!navMeshAgent.isOnOffMeshLink)
        {
            Debug.Log("Not on mesh Off Link");
            navMeshAgent.enabled = false;
        }
    }

    public override TaskStatus OnUpdate()
    {
        bool isGrounded = animator.GetBool("Grounded");
        if(isGrounded)
        {
            animator.SetBool("Falling", false);
            navMeshAgent.enabled = true;
            return TaskStatus.Success;
        }
        else
        {
            return TaskStatus.Running;
        }
    }
}