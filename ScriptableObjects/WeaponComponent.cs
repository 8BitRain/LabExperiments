using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WeaponComponent", menuName = "ScriptableObjects/WeaponComponent", order = 5)]
[System.Serializable]
public class WeaponComponent : ScriptableObject
{
    public bool canFloat;
    public float floatIntensity;
}
