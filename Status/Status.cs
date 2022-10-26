using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Status : MonoBehaviour
{

    public float hp = 100;
    public float maxHP = 100;
    public float mp = 100;
    public float stamina = 100;
    public float staminaRefillTime = 1.75f;
    public float staminaRefillDelay = 3.0f;

    public static event Action<GameObject, float> onHealthStatusChange;
    public static event Action<GameObject, float> onStaminaStatusChange;

    private void OnEnable()
    {
        HurtBox.gotCollision += SetHP;
        HurtBox.gotCollision += SetStamina;
        //TODO add logic tied to blocking or other stamina related expenditures
    }

    private void OnDisable()
    {
        HurtBox.gotCollision -= SetHP;
        HurtBox.gotCollision -= SetStamina;
    }
    
    void Start()
    {
        maxHP = hp;
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
                EventsManager.instance.onPlayerDefeated.Invoke();
            }
        }
    }

    public void SetHP(GameObject hurtBoxAgentInstance,GameObject hurtBoxInstance,AbilityComponent abilityComponent)
    {
        if(this.gameObject != hurtBoxAgentInstance)
        {
            Debug.Log("Observer Pattern Event Error: " + this.gameObject.name + " is not the same as " + hurtBoxAgentInstance.name);
            return;
        }
        else
        {
            Debug.Log("Observer Pattern Event Success: " + this.gameObject.name + " is the same as " + hurtBoxAgentInstance.name);
        }

        if(this.gameObject.GetComponent<Animator>().GetBool("Gaurding"))
        {
            Debug.Log(this.gameObject.name + " blocked: " + abilityComponent.componentName);
            //hp = hp - abilityComponent.collisionComponent.hpDamage;
        }
        
        if(!this.gameObject.GetComponent<Animator>().GetBool("Gaurding"))
        {
            hp = hp - abilityComponent.collisionComponent.hpDamage;
            onHealthStatusChange.Invoke(this.gameObject, hp);
        }
    }

    public void HPPickup(float healthIncrease)
    {
        //Use full amount of health increase
        if(hp + healthIncrease <= maxHP)
        {
            hp = hp + healthIncrease;
        }
        //Only heal to max health
        else
        {
            hp = maxHP;
        }
    }

    public void SetStamina(GameObject hurtBoxAgentInstance,GameObject hurtBoxInstance,AbilityComponent abilityComponent)
    {
        if(this.gameObject != hurtBoxAgentInstance)
        {
            Debug.Log("Set Stamina: Observer Pattern Event Error: " + this.gameObject.name + " is not the same as " + hurtBoxAgentInstance.name);
            return;
        }
        else
        {
            Debug.Log("Set Stamina: Observer Pattern Event Success: ");
        }

        if(this.gameObject.GetComponent<Animator>().GetBool("Gaurding"))
        {
            Debug.Log(this.gameObject.name + " blocked: " + abilityComponent.componentName);
            stamina = stamina - abilityComponent.collisionComponent.hpDamage;
            onStaminaStatusChange.Invoke(this.gameObject, stamina);
        }

    }

    public void SetStamina(GameObject instance, float staminaAmount)
    {
        if(this.gameObject != instance)
        {
            return;
        }
        stamina = stamina - staminaAmount;
        onStaminaStatusChange.Invoke(this.gameObject, stamina);
    }

    public float GetStamina()
    {
        return this.stamina;
    }

    public float GetHP()
    {
        return this.hp;
    }


}
