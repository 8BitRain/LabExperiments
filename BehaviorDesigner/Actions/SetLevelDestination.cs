using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

public class SetLevelDestination : Action
{

    public SharedVector3 destination;

    public SharedFloat spawnNextLevelAssetDistance;

    public SharedVector3 direction;

    public override void OnStart()
    {
        destination = transform.position + direction.Value * spawnNextLevelAssetDistance.Value;
    }
    public override TaskStatus OnUpdate()
    {

        return TaskStatus.Success;
    }
}