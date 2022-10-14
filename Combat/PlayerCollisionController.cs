using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlayerCollisionController : CollisionController
{
    private PlayerMovementController playerMovementController;
    private AnimationController playerAnimationController;
    private Animator playerAnimator;
    
    public void Start()
    {
        playerMovementController = this.GetComponent<PlayerMovementController>();
        playerAnimationController = this.GetComponent<AnimationController>();
        playerAnimator = this.GetComponent<Animator>();
    }

    public override void CollisionLogic(GameObject instance, GameObject collisionPoint, AbilityComponent abilityComponent)
    {
        if(instance != this.gameObject)
            return;

        if(playerAnimator.GetBool("Gaurding"))
        {
            KnockbackLogic(abilityComponent.collisionComponent, .5f);
            return;
        }

        Debug.Log(gameObject.name + "is processing collision");

        //Disable Player Movement by controls, but enable gravity.
        playerMovementController.DisableMovement();
        playerMovementController.EnableApplyGravityLockPlayerInput();

        //Set Damaged State
        transform.GetComponent<Animator>().SetBool("Wakeup", false);
        //Setting to false then true to trigger a state re-run in behavior tree
        transform.GetComponent<Animator>().SetBool("Damaged", false);
        transform.GetComponent<Animator>().SetBool("Damaged", true);

        //Play animation
        playerAnimationController.ChangeAnimationState(playerAnimator, damagedAnimation);

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
        DOVirtual.DelayedCall(3f, () => {
            if(playerMovementController != null)
            {
                playerMovementController.DisableApplyGravityLockPlayerInput();
                playerMovementController.EnableMovement();
                transform.GetComponent<Animator>().SetBool("Damaged", false);
            }
        });
    }

    public override void DamageFlash()
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

    //TODO use this when player is gaurding. Attacks should knockback
    public void KnockbackLogic(CollisionComponent collisionComponent, float multiplier)
    {
        switch (collisionComponent.knockbackDirection)
        {
            case CollisionComponent.KnockBackDirection.Forward:
                
                if(TryGetComponent<Rigidbody>(out Rigidbody rigidbodyInstance1))
                {
                    rigidbodyInstance1.AddForce(transform.forward*collisionComponent.knockbackAmount/40 * multiplier);
                }
                else
                {
                    transform.DOMove(transform.position + transform.forward*collisionComponent.knockbackAmount*multiplier, collisionComponent.knockbackTime);
                }
                break;
            case CollisionComponent.KnockBackDirection.Backward:
                if(TryGetComponent<Rigidbody>(out Rigidbody rigidbodyInstance2))
                {
                    rigidbodyInstance2.AddForce(transform.forward*collisionComponent.knockbackAmount/40 * multiplier);
                }
                else
                {
                    transform.DOMove(transform.position - transform.forward*collisionComponent.knockbackAmount*multiplier, collisionComponent.knockbackTime);
                }
                break;
            case CollisionComponent.KnockBackDirection.Lateral:
                transform.DOMove(transform.position + transform.right*collisionComponent.knockbackAmount*multiplier, collisionComponent.knockbackTime);
                break;
            case CollisionComponent.KnockBackDirection.Up:
                //TODO: Add Rigidbody based force movement
                transform.DOMove(transform.position + transform.up*collisionComponent.knockbackAmount*multiplier, collisionComponent.knockbackTime);
                break;
            default:
                break;
        }
    }
}
