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
    // Start is called before the first frame update

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
        Initialize();
    }

    public void Initialize()
    {
        Destroy(this.gameObject, projectileComponent.keepAliveTime);

        if(hitBox != null)
        {
            EventsManager.instance.OnTriggerHitBox(hitBox.gameObject, this.gameObject, true, projectileComponent.hitBoxStartDelay, projectileComponent.hitBoxDuration);
            Debug.Log("Projectile: " + this.gameObject.name + "Triggering Hitbox");
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
        this.projectileSummonerReference  = reference;
    }

    //on collision play vfx;
}
