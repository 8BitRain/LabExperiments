using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Wave", menuName = "ScriptableObjects/Wave", order = 13)]
[System.Serializable]
public class Wave : ScriptableObject
{
    public AIUnit[] AIUnits;
}
