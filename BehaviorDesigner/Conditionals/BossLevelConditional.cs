using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using DG.Tweening;

public class BossLevelConditional : Conditional
{

    
    private AIDirector AIDirector;
    public override void OnStart()
    {
       AIDirector = this.GetComponent<LevelAssetGenerator>().AIDirector;
       Debug.Log("AssignAIDirector");
    }

    public override TaskStatus OnUpdate()
    {
        if(AIDirector == null)
        {
            Debug.Log("AIDirector is null");
            return TaskStatus.Failure;
        }

        if(AIDirector.HasBossWaveSpawned())
        {
            return TaskStatus.Success;
        }
        else
        {
            return TaskStatus.Failure;
        }
    }    
}