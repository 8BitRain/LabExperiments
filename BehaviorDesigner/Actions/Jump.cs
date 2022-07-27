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
        animator.SetBool("Grounded", false);
        animator.SetBool("Jumping", true);
        navMeshAgent = this.GetComponent<NavMeshAgent>();
        if(!navMeshAgent.isOnOffMeshLink)
        {
            Debug.Log("Not on mesh Off Link");
            navMeshAgent.enabled = false;
        }
        AIReference = this.GetComponent<Enemy>();

        //reset y_velocity to prevent super bouncing
        _velocity = AIReference.GetVelocity();
        AIReference.SetVelocity(new Vector3(_velocity.x, 0, _velocity.y));
        _velocity.y = 0; 
        //_velocity.y += Mathf.Sqrt(JumpHeight * -2f * gravity);
        _velocity.y += Mathf.Sqrt(1f * -2f * -4.9f);
        AIReference.SetVelocity(new Vector3(_velocity.x, _velocity.y, _velocity.z));

    }

    public override TaskStatus OnUpdate()
    {
        Debug.Log("AI Jump: Y Velocity" + _velocity.y);
        bool isGrounded = animator.GetBool("Grounded");
        if(AIReference.GetVelocity().y < 0)
        {
            animator.SetBool("Jumping", false);
            //navMeshAgent.enabled = true;
            Debug.Log("We are now falling. End Jump action");
            return TaskStatus.Success;
        }
        else
        {
            return TaskStatus.Running;
        }
    }
}