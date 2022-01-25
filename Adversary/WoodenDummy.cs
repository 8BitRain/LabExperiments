using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

public class WoodenDummy : MonoBehaviour
{
    private void OnEnable()
    {
        HurtBox.gotCollision += CollisionLogic;
    }

    private void OnDisable()
    {
        HurtBox.gotCollision -= CollisionLogic;
    }

    void CollisionLogic(GameObject instance, GameObject collisionPoint, AbilityComponent abilityComponent)
    {
        if(instance != this.gameObject)
            return;
        Debug.Log(gameObject.name + "is processing collision");

        CollisionComponent collisionComponent = abilityComponent.collisionComponent;
        transform.DOPunchScale(UnityEngine.Random.insideUnitSphere * collisionComponent.squashAndStretchAmount, collisionComponent.squashAndStretchTime);
        transform.DORotate(transform.rotation.eulerAngles + transform.rotation.eulerAngles*720, 1.5f);

        switch (collisionComponent.knockbackDirection)
        {
            case CollisionComponent.KnockBackDirection.Forward:
                transform.DOMove(transform.position + transform.forward*collisionComponent.knockbackAmount, collisionComponent.knockbackTime);
                break;
            case CollisionComponent.KnockBackDirection.Backward:
                transform.DOMove(transform.position - transform.forward*collisionComponent.knockbackAmount, collisionComponent.knockbackTime);
                break;
            case CollisionComponent.KnockBackDirection.Lateral:
                transform.DOMove(transform.position + transform.right*collisionComponent.knockbackAmount, collisionComponent.knockbackTime);
                break;
            default:
                break;
        }
        
    }
}
