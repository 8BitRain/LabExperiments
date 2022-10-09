using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelAsset", menuName = "ScriptableObjects/LevelAsset", order = 12)]
[System.Serializable]
public class LevelAsset : ScriptableObject
{
    public enum LevelAssetID
    {
        BUILDING,
        TV
    }

    public LevelAssetID levelAssetID;
}
