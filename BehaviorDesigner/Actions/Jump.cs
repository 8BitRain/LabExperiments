using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine.AI;
using DG.Tweening;

public class Jump : Action
{

    private Animator animator;
    private NavMeshAgent navMeshAgent;
    public string jumpAnimation;
    private Vector3 _velocity;
    private Enemy AIReference;
    private WoodenDummy tackyAIReference;

    private bool _traversingLink = false;
    public float jumpForwardDistance = .05f;

    public override void OnStart()
    {
        animator = this.GetComponent<Animator>();
        AIReference = this.GetComponent<Enemy>();
        tackyAIReference = this.GetComponent<WoodenDummy>();
        this.GetComponent<AnimationController>().ChangeAnimationState(animator, jumpAnimation);
        animator.SetBool("Grounded", false);
        animator.SetBool("Jumping", true);
        navMeshAgent = this.GetComponent<NavMeshAgent>();
        //navMeshAgent.autoTraverseOffMeshLink = false;

        tackyAIReference._isGrounded = false;

        if(!navMeshAgent.isOnOffMeshLink)
        {
            Debug.Log("Not on mesh Off Link");
            navMeshAgent.enabled = false;

            //reset y_velocity to prevent super bouncing
            _velocity = AIReference.GetVelocity();
            AIReference.SetVelocity(new Vector3(_velocity.x, 0, _velocity.z));
            _velocity.y = 0; 
            //_velocity.y += Mathf.Sqrt(JumpHeight * -2f * gravity);
            _velocity.y += Mathf.Sqrt(tackyAIReference.JumpHeight * -2f * -4.9f);
            _velocity.z += jumpForwardDistance;
            AIReference.SetVelocity(new Vector3(_velocity.x, _velocity.y, _velocity.z));
        }
        else
        {
            //https://stackoverflow.com/questions/12247647/unity3d-offnavmesh-jump-issue
            OffMeshLinkData link = navMeshAgent.currentOffMeshLinkData;
            _traversingLink = true;
            transform.DOMove(link.endPos, 1.5f).OnComplete(() => {
                navMeshAgent.CompleteOffMeshLink();
                _traversingLink = false;
            });
        }
    }

    public override TaskStatus OnUpdate()
    {
        Debug.Log("AI Jump: Y Velocity" + _velocity.y);
        bool isGrounded = animator.GetBool("Grounded");
        if(AIReference.GetVelocity().y < 0 && !navMeshAgent.enabled)
        {
            /* Enabling creates a double jump effect
            _velocity.z = 0;
            AIReference.SetVelocity(new Vector3(_velocity.x, _velocity.y, _velocity.z));
            */
            animator.SetBool("Jumping", false);
            //navMeshAgent.enabled = true;
            Debug.Log("We are now falling. End Jump action");
            return TaskStatus.Success;
        }

        if(navMeshAgent.enabled && !_traversingLink)
        {
            navMeshAgent.Resume();
            return TaskStatus.Success;
        }
        return TaskStatus.Running;
    }
}