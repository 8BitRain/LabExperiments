using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Projectile : MonoBehaviour
{
    public ProjectileComponent projectileComponent;
    public GameObject VFX;
    public HitBox hitBox;
    private GameObject projectileSummonerReference;

    private void OnEnable()
    {
        HitBox.collision += CollisionLogic;
    }

    private void OnDisable()
    {
        HitBox.collision -= CollisionLogic;
    }

    void Start()
    {
        
        //Initialize();
    }

    public void Initialize()
    {
        Debug.Log("Running initialize" + Time.time);
        //Debug.Log("Printing hitbox value: " + hitBox.gameObject.name);
        Destroy(this.gameObject, projectileComponent.keepAliveTime);

        if(this.hitBox != null)
        {
            Debug.Log("PROJECTILE SUMMONER REFERENCE: ", GetProjectileSummonerReference());
            EventsManager.instance.OnTriggerHitBox(hitBox.gameObject, GetProjectileSummonerReference(), true, projectileComponent.hitBoxStartDelay, projectileComponent.hitBoxDuration);
            Debug.Log("Projectile: " + this.gameObject.name + "Triggering Hitbox");
        }
        else
        {
            Debug.Log("Hitbox is null");
        }

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

    public void CollisionLogic(GameObject targetInstance, GameObject hurtBoxInstance, GameObject summonerInstance, AbilityComponent abilityComponent)
    {
        if(this.gameObject != summonerInstance)
        {
            Debug.Log("Projectile: " + this.gameObject.name + " is not equivalent to the summoner instance: " + summonerInstance.gameObject.name);
            return;
        }

        Debug.Log("Projectile: " + this.gameObject.name + " " + "collided with " + targetInstance.gameObject.name);
        Debug.Log("Spawning at: " + targetInstance.transform.position + "rotation: " + targetInstance.transform.rotation);
        GameObject vfxInstance = Instantiate(VFX, targetInstance.transform.position, Quaternion.identity);
        //vfxInstance.transform.SetParent(targetInstance.transform);
        //vfxInstance.transform.position = new Vector3(0,0,0);
        Debug.Log("Projectile VFX: position is " + vfxInstance.transform.position);
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

    public GameObject GetVFX()
    {
        return VFX;
    }

    public GameObject GetProjectileSummonerReference()
    {
        return projectileSummonerReference;
    }

    public void SetProjectileSummonerReference(GameObject reference)
    {
        Debug.Log("Setting projectileSummonerReference to: " + reference.name);
        this.projectileSummonerReference  = reference;
    }

    //on collision play vfx;
}
