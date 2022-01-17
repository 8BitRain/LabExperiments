using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AbilityComponents", menuName = "ScriptableObjects/AbilityComponents", order = 2)]
public class AbilityComponents : ScriptableObject
{

    [System.Serializable]
    public class AbilityComponentEntry
    {
        public string name;
        public int index;
    }


    public AbilityComponentEntry[] abilityComponents;

    //Causes errors as Scriptable Objects should not really be modified at runtime
    /*public void AddAbilityComponentsToStringMap(){
        Debug.Log("Adding abilitycomponents to string map");
        stringMap = new StringIntUnityDictionary();
        foreach (var abilityComponent in abilityComponents)
        {
            stringMap.Add(abilityComponent.name, abilityComponent.index);
        }
    }*/

}

