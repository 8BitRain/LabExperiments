using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModularAbilityComponent : MonoBehaviour, IAbilityComponent
{
    public AbilityComponent abilityComponent;
    public HitBox hitBox;

    public AbilityComponent GetAbilityComponent()
    {
        return this.abilityComponent;
    }

    
}
