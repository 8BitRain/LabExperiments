﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CollisionController : MonoBehaviour
{
    public string damagedAnimation;

    private void OnEnable()
    {
        HurtBox.gotCollision += CollisionLogic;
    }

    private void OnDisable()
    {
        HurtBox.gotCollision -= CollisionLogic;
    }

    public virtual void CollisionLogic(GameObject instance, GameObject collisionPoint, GameObject damageDealer, AbilityComponent abilityComponent)
    {
        if(instance != this.gameObject)
            return;
        Debug.Log(gameObject.name + "is processing collision");

        //Set Damaged State
        transform.GetComponent<Animator>().SetBool("Wakeup", false);
        //Setting to false then true to trigger a state re-run in behavior tree
        transform.GetComponent<Animator>().SetBool("Damaged", false);
        transform.GetComponent<Animator>().SetBool("Damaged", true);

        /*DOVirtual.DelayedCall(1.0f, () => {
            transform.GetComponent<Animator>().SetBool("Damaged", true);
        });*/

        //Flash Red color when hit
        DamageFlash();

        CollisionComponent collisionComponent = abilityComponent.collisionComponent;
        transform.DOPunchScale(UnityEngine.Random.insideUnitSphere * collisionComponent.squashAndStretchAmount, collisionComponent.squashAndStretchTime);
        //transform.DORotate(transform.rotation.eulerAngles + transform.rotation.eulerAngles*720, 1.5f);

        switch (collisionComponent.knockbackDirection)
        {
            case CollisionComponent.KnockBackDirection.Forward:
                
                if(TryGetComponent<Rigidbody>(out Rigidbody rigidbodyInstance1))
                {
                    rigidbodyInstance1.AddForce(transform.forward*collisionComponent.knockbackAmount/40);
                }
                else
                {
                    transform.DOMove(transform.position + transform.forward*collisionComponent.knockbackAmount, collisionComponent.knockbackTime);
                }
                break;
            case CollisionComponent.KnockBackDirection.Backward:
                if(TryGetComponent<Rigidbody>(out Rigidbody rigidbodyInstance2))
                {
                    rigidbodyInstance2.AddForce(transform.forward*collisionComponent.knockbackAmount/40);
                }
                else
                {
                    transform.DOMove(transform.position - transform.forward*collisionComponent.knockbackAmount, collisionComponent.knockbackTime);
                }
                break;
            case CollisionComponent.KnockBackDirection.Lateral:
                transform.DOMove(transform.position + transform.right*collisionComponent.knockbackAmount, collisionComponent.knockbackTime);
                break;
            case CollisionComponent.KnockBackDirection.Up:
                //TODO: Add Rigidbody based force movement
                transform.DOMove(transform.position + transform.up*collisionComponent.knockbackAmount, collisionComponent.knockbackTime);
                break;
            default:
                break;
        }
        
    }

    public virtual void DamageFlash()
    {
        foreach (Transform child in GetComponentsInChildren<Transform>())
        {
            if(child.TryGetComponent<Renderer>(out Renderer renderer))
            {
                renderer.material.DOColor(Color.red, "_Tint", .2f).OnComplete(() => {
                    renderer.material.DOColor(Color.white, "_Tint", .2f);
                }).SetLoops(6);
            }
        }
    }
}
