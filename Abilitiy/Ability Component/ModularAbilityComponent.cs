using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModularAbilityComponent : MonoBehaviour, IAbilityComponent
{
    public AbilityComponent abilityComponent;
    public HitBox hitBox;
    public Projectile projectile;
    public GameObject VFX;

    public AbilityComponent GetAbilityComponent()
    {
        return this.abilityComponent;
    }

    public Projectile GetProjectile()
    {
        return projectile;
    }

    public GameObject GetVFX()
    {
        return VFX;
    }

    
}
