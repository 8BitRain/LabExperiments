using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

public class SpawnWave : Action
{

    public SharedGameObjectList spawnedWave;
    private AIDirector AIDirector;


    public override void OnStart()
    {
        AIDirector = transform.GetComponent<AIDirector>();
        spawnedWave.Value = AIDirector.SpawnAIUnits(AIDirector.Waves[0]);

    }
    public override TaskStatus OnUpdate()
    {

        return TaskStatus.Success;
    }
}