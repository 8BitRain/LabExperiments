using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime;
using UnityEngine.Events;
public class AIDirector : MonoBehaviour
{
    [Header("Wave Generator")]
    public Wave[] Waves;
    public Wave[] BossWaves;
    public Enemy[] AIEnemies;

    public Enemy[] BossAI;

    [Header("Wave Generator Amount")]
    public int standardWaveCount = 0;
    private int standardWaveIndex = 0;

    private bool bossWaveSpawned = false;

    [Header("Wave Generator Spawn")]
    public Transform AIGroup;

    public Transform spawnOrigin;
    public Vector2 spawnRange;
    [Header("Behavior Tree Necessary Variables")]
    public  List<GameObject> spawnedWave = new List<GameObject>();
    public GameObject Player;

    public UnityEvent onBossSpawn;


    private Dictionary<AIUnit.AIUnitType, Enemy> AIEnemyDictionary = new Dictionary<AIUnit.AIUnitType, Enemy>();
    private Dictionary<AIUnit.AIUnitType, Enemy> BossAIEnemyDictionary = new Dictionary<AIUnit.AIUnitType, Enemy>();
    // Start is called before the first frame update
    void Start()
    {
        foreach (Enemy enemy in AIEnemies)
        {
            AIEnemyDictionary.Add(enemy.AIUnitType, enemy);
        }

        foreach(Enemy enemy in BossAI)
        {
            BossAIEnemyDictionary.Add(enemy.AIUnitType, enemy);
        }
        //SpawnAIUnits(Waves[0]);
    }

    // Update is called once per frame
    void Update()
    {
        if(bossWaveSpawned && spawnedWave.Count == 0)
        {
            EventsManager.instance.onBossDefeated.Invoke();
        }
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
                Vector3 spawnOffset = new Vector3(0, 20, Random.Range(spawnRange.x,spawnRange.y)) + spawnOrigin.position;
                GameObject AIUnit = Instantiate(AIEnemyDictionary[wave.AIUnits[i].AIUnitEntity], spawnOffset, spawnOrigin.rotation).gameObject;
                AIUnit.transform.SetParent(AIGroup);

                AIUnit.GetComponent<BehaviorTree>().SetVariableValue("TargetGameObject", Player);
                
                spawnedWave.Add(AIUnit);
                Player.GetComponent<CameraController>().LoadAIHealthBars();
            }
        }
        //IncrementWaveIndex
        standardWaveIndex++;
        return spawnedWave;
    }

    public List<GameObject> SpawnBossAIUnits(Wave wave)
    {
        //Loop through the wave and spawn the AI unity from the Wave
        for (int i = 0; i < wave.AIUnits.Length; i++)
        {
            for(int j = 0; j < wave.AIUnits[i].multiple; j++)
            {
                //Access the AIEnemyDictionary to spawn the correct AI unit based 
                //on the information supplied by wave
                Debug.Log("Load " + BossAIEnemyDictionary[wave.AIUnits[i].AIUnitEntity]);
                Vector3 spawnOffset = new Vector3(0, 20, Random.Range(spawnRange.x,spawnRange.y)) + spawnOrigin.position;
                GameObject AIUnit = Instantiate(BossAIEnemyDictionary[wave.AIUnits[i].AIUnitEntity], spawnOffset, spawnOrigin.rotation).gameObject;
                AIUnit.transform.SetParent(AIGroup);
                AIUnit.GetComponent<BehaviorTree>().SetVariableValue("TargetGameObject", Player);
                
                spawnedWave.Add(AIUnit);
                Player.GetComponent<CameraController>().LoadAIHealthBars();
            }
        }
        standardWaveIndex++;
        bossWaveSpawned = true;
        onBossSpawn.Invoke();
        return spawnedWave;
    }



    public void DestroyAllActiveAIUnits()
    {
        foreach (GameObject aiUnit in spawnedWave)
        {
            Destroy(aiUnit);
        }

        spawnedWave.Clear();
    }

    public int GetStandardWaveCount()
    {
        return standardWaveCount;
    }

    public int GetStandardWaveIndex()
    {
        return standardWaveIndex;
    }

    public bool HasBossWaveSpawned()
    {
        return bossWaveSpawned;
    }
}
