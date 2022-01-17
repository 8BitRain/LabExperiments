using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModularAbilityComponent : MonoBehaviour, IAbilityComponent
{
    public AbilityComponent abilityComponent;

    public AbilityComponent GetAbilityComponent()
    {
        return this.abilityComponent;
    }

    
}
