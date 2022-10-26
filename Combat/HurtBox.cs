using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using System;
using Cinemachine;
using UnityEngine;


public class HurtBox : MonoBehaviour
{
    public GameObject Agent;
    private Transform lastCollisionPoint;
    //Events
    public static event Action<GameObject, GameObject, float, float> recievedCollision;
    public static event Action<GameObject, GameObject, GameObject, AbilityComponent> gotCollision;
    [SerializeField] private UnityEvent onRecievedCollision;

    private void OnEnable()
    {
        HitBox.collidedWithTarget += ApplyDamagedLogic;
        HitBox.collision += ApplyCollision;
    }

    private void OnDisable()
    {
        HitBox.collidedWithTarget -= ApplyDamagedLogic;
        HitBox.collision -= ApplyCollision;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //Applies damage to specific instance connected through hitbox
    /*
    * Example: P1 attacks P2 
    * P1's hitbox triggers the collidedWithTarget event
    * P1's hitbox supplies the event with P2's gameObject reference, hurtBox reference, damage, and knockback
    * P2's hurtbox wants to apply damage to P2 and P2 only. So we make sure the collision instance only effects P2 hence this.gameObject != hurtBoxInstance return
    */
    void ApplyDamagedLogic(GameObject hurtBoxAgentInstance, GameObject hurtBoxInstance, float damage, float knockback)
    {
        if(this.gameObject != hurtBoxInstance)
            return;
        
        recievedCollision.Invoke(hurtBoxAgentInstance, transform.parent.gameObject, damage, knockback);
        
    }

    void ApplyCollision(GameObject hurtBoxAgentInstance, GameObject hurtBoxInstance, GameObject damageDealer, AbilityComponent abilityComponent)
    {
        if(this.gameObject != hurtBoxInstance)
            return;

        if(Agent.GetComponent<Animator>().GetBool("Dodging"))
        {
            Debug.Log("Slow Time: Trigger slow time");
            EventsManager.instance.OnSlowTime(Agent);
            return;
        }

        lastCollisionPoint = damageDealer.transform;
        CameraShake(damageDealer, abilityComponent);
        //Custom Event Assigned to Hurtbox to spawn a hit impact particle effect
        onRecievedCollision.Invoke();
        gotCollision.Invoke(hurtBoxAgentInstance, hurtBoxInstance, damageDealer, abilityComponent);
    }

    //Modular camera shake to help
    void CameraShake(GameObject hitBoxAgent, AbilityComponent abilityComponent)
    {
        if(abilityComponent.screenShakeComponent != null)
        {
            Debug.Log("Trigger Screenshake");
            //We check to see if the hitBoxAgent has a cameraController attached.
            if(hitBoxAgent.TryGetComponent<CameraController>(out CameraController cameraControllerA))
            {
                GameObject virtualCam = cameraControllerA.GetCameraInstance().GetComponent<CinemachineBrain>().ActiveVirtualCamera.VirtualCameraGameObject;
                Debug.Log("VirtualCam name: " + virtualCam.name);
                virtualCam.GetComponent<CinemachineScreenShake>().DoShake(abilityComponent.screenShakeComponent);
            }


            //We check to see if the current instance has a cameraController attached. 
            if(Agent.TryGetComponent<CameraController>(out CameraController cameraControllerB))
            {
                GameObject virtualCam = cameraControllerB.GetCameraInstance().GetComponent<CinemachineBrain>().ActiveVirtualCamera.VirtualCameraGameObject;
                Debug.Log("VirtualCam name: " + virtualCam.name);
                virtualCam.GetComponent<CinemachineScreenShake>().DoShake(abilityComponent.screenShakeComponent);
            }
        }
    }

    public Transform GetLastCollision()
    {
        if(lastCollisionPoint != null)
        {   
            return lastCollisionPoint;
        }   
        return null;
    }

    public void SpawnHitImpactVFX(GameObject vfx)
    {
        Vector3 spawnPosition = Agent.GetComponent<Body>().Back.position;
        Instantiate(vfx, spawnPosition, Agent.transform.rotation);
    }
}
