using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Powerup", menuName = "ScriptableObjects/Powerup", order = 16)]
[System.Serializable]
public class Powerup : ScriptableObject
{
    public float healthIncrease = 0;
    public float damageIncrease = 0;
}
