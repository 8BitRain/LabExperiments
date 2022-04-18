using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModularAttackElement : MonoBehaviour, IMeleeAttackComponent
{
    public MeleeAttackComponent meleeAttackComponent;
    public HitBox hitBox;
    public Projectile projectile;
    public GameObject VFX;
    public CameraSettings cameraSettings;

    public MeleeAttackComponent GetMeleeAttackComponent()
    {
        return this.meleeAttackComponent;
    }

    public CameraSettings GetCameraSettings()
    {
        return this.cameraSettings;
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
