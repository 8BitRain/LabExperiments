using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime;
public class AIDirector : MonoBehaviour
{
    [Header("Wave Generator")]
    public Wave[] Waves;
    public Enemy[] AIEnemies;

    public Transform AIGroup;

    public Transform spawnOrigin;
    [Header("Behavior Tree Necessary Variables")]
    public  List<GameObject> spawnedWave = new List<GameObject>();
    public GameObject Player;


    private Dictionary<AIUnit.AIUnitType, Enemy> AIEnemyDictionary = new Dictionary<AIUnit.AIUnitType, Enemy>();
    // Start is called before the first frame update
    void Start()
    {
        foreach (Enemy enemy in AIEnemies)
        {
            AIEnemyDictionary.Add(enemy.AIUnitType, enemy);
        }
        SpawnAIUnits(Waves[0]);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public List<GameObject> SpawnAIUnits(Wave wave){
        //Loop through the wave and spawn the AI unity from the Wave
        for (int i = 0; i < wave.AIUnits.Length; i++)
        {
            for(int j = 0; j < wave.AIUnits[i].multiple; j++)
            {
                //Access the AIEnemyDictionary to spawn the correct AI unit based 
                //on the information supplied by wave
                Debug.Log("Load " + AIEnemyDictionary[wave.AIUnits[i].AIUnitEntity]);
                Vector3 spawnOffset = new Vector3(0, 20, Random.Range(0,40)) + spawnOrigin.position;
                GameObject AIUnit = Instantiate(AIEnemyDictionary[wave.AIUnits[i].AIUnitEntity], spawnOffset, spawnOrigin.rotation).gameObject;
                AIUnit.transform.SetParent(AIGroup);

                AIUnit.GetComponent<BehaviorTree>().SetVariableValue("TargetGameObject", Player);
                
                spawnedWave.Add(AIUnit);
            }
        }
        return spawnedWave;
    }
}
