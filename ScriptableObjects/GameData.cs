using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameData", menuName = "ScriptableObjects/GameData", order = 15)]
[System.Serializable]
public class GameData : ScriptableObject
{
    public string currentScene;
}
