using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine.AI;

public class Jump : Action
{

    private Animator animator;
    private NavMeshAgent navMeshAgent;
    public string jumpAnimation;
    private Vector3 _velocity;
    private Enemy AIReference;

    public override void OnStart()
    {
        animator = this.GetComponent<Animator>();
        this.GetComponent<AnimationController>().ChangeAnimationState(animator, jumpAnimation);
        animator.SetBool("Jumping", true);
        navMeshAgent = this.GetComponent<NavMeshAgent>();
        if(!navMeshAgent.isOnOffMeshLink)
        {
            Debug.Log("Not on mesh Off Link");
            navMeshAgent.enabled = false;
        }
        this.GetComponent<AnimationController>().ChangeAnimationState(animator,"Player_jump");
        AIReference = this.GetComponent<Enemy>();

        //reset y_velocity to prevent super bouncing
        _velocity = AIReference.GetVelocity();
        AIReference.SetVelocity(new Vector3(_velocity.x, 0, _velocity.y));
        _velocity.y = 0; 
        _velocity.y += Mathf.Sqrt(3f * -2f * -9.81f);
        AIReference.SetVelocity(new Vector3(_velocity.x, _velocity.y, _velocity.z));

    }

    public override TaskStatus OnUpdate()
    {
        bool isGrounded = animator.GetBool("Grounded");
        if(isGrounded)
        {
            animator.SetBool("Jumping", false);
            navMeshAgent.enabled = true;
            return TaskStatus.Success;
        }
        else
        {
            return TaskStatus.Running;
        }
    }
}