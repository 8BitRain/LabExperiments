using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Status : MonoBehaviour
{

    public float hp = 100;
    public float mp = 100;

    private void OnEnable()
    {
        HurtBox.gotCollision += SetHP;
    }

    private void OnDisable()
    {
        HurtBox.gotCollision -= SetHP;
    }
    
    void Start()
    {
        
    }

    void Update()
    {
        if(hp <= 0)
        {
            //TODO Should probably have a generic death controller that controls OnDeath logic
            if(TryGetComponent<Enemy>(out Enemy enemy))
            {
                enemy.OnDeath();
            }
            else
            {
                //TODO add logic for player character. 
            }
        }
    }

    public void SetHP(GameObject hurtBoxAgentInstance,GameObject hurtBoxInstance,AbilityComponent abilityComponent)
    {
        if(this.gameObject != hurtBoxAgentInstance)
            return;
        
        hp = hp - abilityComponent.collisionComponent.hpDamage;
    }


}
