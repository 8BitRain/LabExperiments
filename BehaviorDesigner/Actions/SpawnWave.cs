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
        /* Spawn Standard Wave or Boss Wave*/
        Debug.Log("AI Director wave index: " + AIDirector.GetStandardWaveIndex());
        Debug.Log("AI Director wave count: " + AIDirector.GetStandardWaveCount());
        if(AIDirector.GetStandardWaveIndex() < AIDirector.GetStandardWaveCount() - 1)
        {
            spawnedWave.Value = AIDirector.SpawnAIUnits(AIDirector.Waves[Random.Range(0, AIDirector.Waves.Length - 1)]);
        }
        else if (AIDirector.GetStandardWaveIndex() == AIDirector.GetStandardWaveCount() - 1)
        {
            spawnedWave.Value = AIDirector.SpawnBossAIUnits(AIDirector.BossWaves[0]);
        }
        else
        {
            Debug.Log("YOU WIN!");
        }

        

    }
    public override TaskStatus OnUpdate()
    {

        return TaskStatus.Success;
    }
}