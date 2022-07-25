using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using DG.Tweening;
using UnityEngine.AI;


public class OffNavMeshLinkPositionCheck : Conditional
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
        if(navMeshAgent.currentOffMeshLinkData.startPos.y > navMeshAgent.currentOffMeshLinkData.endPos.y)
        {
            Debug.Log("off nav mesh start Pos > end Pos, we need to fall");
            return TaskStatus.Success;
        }
        else
        {
            Debug.Log("Off nav mesh start Pos < end Pos, we need to jump ");
            return TaskStatus.Failure;
        }

    }   

}
