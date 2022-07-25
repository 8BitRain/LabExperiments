using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using DG.Tweening;
using UnityEngine.AI;

//Return whether current agent is on an off link
public class OffNavMeshLinkStatus : Conditional
{
    public NavMeshAgent navMeshAgent;

    public override void OnStart()
    {
        if(this.gameObject.TryGetComponent<NavMeshAgent>(out NavMeshAgent navMeshAgent))
        {
            this.navMeshAgent = navMeshAgent;
        }
    }

    public override TaskStatus OnUpdate()
    {
        if(navMeshAgent.isOnOffMeshLink)
        {
            Debug.Log("Agent is on an offMeshLink");
            return TaskStatus.Success;
        }
        else
        {
            Debug.Log("Agent is not on an offMeshLink");
            return TaskStatus.Failure;
        }

    }   

}
