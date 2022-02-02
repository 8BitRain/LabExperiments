using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Projectile : MonoBehaviour
{
    public ProjectileComponent projectileComponent;
    public GameObject VFX;
    // Start is called before the first frame update
    void Start()
    {
        Destroy(this.gameObject, projectileComponent.keepAliveTime);
        if(GetRotationBool())
        {
            transform.rotation = GetInitialRotation();
        }
        
        if(projectileComponent.canTravel)
        {
            transform.DOMove(transform.position + transform.forward*projectileComponent.velocity, projectileComponent.time);
        }

        if(projectileComponent.canRotate)
        {
            transform.DORotate(projectileComponent.rotationVector, projectileComponent.rotationTime).SetLoops(projectileComponent.rotationLoops);
        }
    }

    public Quaternion GetInitialRotation()
    {
        return Quaternion.Euler(projectileComponent.initialRotation);
    }

    public Quaternion GetRotationVector()
    {
        return Quaternion.Euler(projectileComponent.rotationVector);
    }

    public bool GetRotationBool()
    {
        return projectileComponent.canRotate;
    }

    //on collision play vfx;
}
