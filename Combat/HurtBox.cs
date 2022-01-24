using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using System;
using UnityEngine;


public class HurtBox : MonoBehaviour
{
    public GameObject Agent;
    //Events
    public static event Action<GameObject, GameObject, float, float> recievedCollision;
    public static event Action<GameObject, GameObject, AbilityComponent> gotCollision;
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

    void ApplyCollision(GameObject hurtBoxAgentInstance, GameObject hurtBoxInstance, AbilityComponent abilityComponent)
    {
        if(this.gameObject != hurtBoxInstance)
            return;
    
        gotCollision.Invoke(hurtBoxAgentInstance, hurtBoxInstance, abilityComponent);
    }
}
