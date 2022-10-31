using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Score", menuName = "ScriptableObjects/Score", order = 17)]
[System.Serializable]
public class Score : ScriptableObject
{
    public int value = 0;
}
