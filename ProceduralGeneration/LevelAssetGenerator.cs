using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class LevelAssetGenerator : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject[] levelAssets;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public GameObject GenerateLevelAsset()
    {
        int randomNumMax = levelAssets.Length;
        GameObject generatedLevelAsset = Instantiate(levelAssets[Random.Range(0, randomNumMax)]);
        return generatedLevelAsset;
    }
}
