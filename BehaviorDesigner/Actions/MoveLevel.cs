using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

public class MoveLevel : Action
{
    // The speed of the level movement
    public float speed = 0; 
    // The transform that the object is moving towards

    public SharedVector3 direction;
    public SharedVector3 destination;

    public SharedFloat spawnNextLevelAssetDistance;

    public override void OnStart()
    {
        destination = transform.position + direction.Value * spawnNextLevelAssetDistance.Value;
    }
    public override TaskStatus OnUpdate()
    {

        if (Vector3.Distance(this.transform.position, destination.Value) <= .1f) {
            return TaskStatus.Success;
        }

        // We haven't reached the target yet so keep moving towards it
        transform.position = Vector3.MoveTowards(transform.position, destination.Value, speed * Time.deltaTime);
        //distanceTravelledSinceComponentSpawned.Value = transform.position.magnitude;
        return TaskStatus.Running;
    }
}