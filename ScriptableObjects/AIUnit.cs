using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AIUnit", menuName = "ScriptableObjects/AIUnit", order = 14)]
[System.Serializable]
public class AIUnit : ScriptableObject
{
    public enum AIUnitType
    {
        NONE,
        MALOCH,
        BAZOOP,
        ESTER,
        COSMO,
        XOCHI,
        LADYLUCK,
        CURSE
    }

    public AIUnitType AIUnitEntity;
    public int multiple = 1;
    public int scoreValue = 1000;
}
