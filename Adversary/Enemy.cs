#if UNITY_EDITOR
using UnityEditor;
#endif

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using BehaviorDesigner.Runtime;

public class Enemy : MonoBehaviour
{
    public float seekRange = 40f;
    public float meleeRange = 10f;
    public float zoneAttackRange = 30f;

    private BehaviorTree behaviorTree;

    private AnimationController animationController;

    public void Start()
    {
        this.animationController = GetComponent<AnimationController>();
        this.behaviorTree = GetComponent<BehaviorTree>();
    }

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

        //Set Damaged State
        transform.GetComponent<Animator>().SetBool("Wakeup", false);
        //Setting to false then true to trigger a state re-run in behavior tree
        transform.GetComponent<Animator>().SetBool("Damaged", false);
        transform.GetComponent<Animator>().SetBool("Damaged", true);

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
                PlayKnockbackAnimation();
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
    
    public void PlayKnockbackAnimation()
    {
        int animChance = Random.Range(0,9);
        if(animChance <= 2)
        {
            GetAnimationController().ChangeAnimationState(this.GetComponent<Animator>(), DamageAnimations.AnimationState.TakeHit_R1.ToString());
        }
        if(animChance >= 3 && animChance< 6)
        {
            GetAnimationController().ChangeAnimationState(this.GetComponent<Animator>(), DamageAnimations.AnimationState.TakeHit_L1.ToString());
        }
        if(animChance >= 6)
        {
            GetAnimationController().ChangeAnimationState(this.GetComponent<Animator>(), DamageAnimations.AnimationState.TakeHit_F2.ToString());
        }
    }
    public void DamageFlash()
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

    public void DeathFlash()
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

    public void OnDeath()
    {
        //Turn off hurtbox(s)
        HurtBox[] hurtBoxes = this.GetComponentsInChildren<HurtBox>();
        foreach (HurtBox hurtBox in hurtBoxes)
        {
            hurtBox.enabled = false;
        }

        //Stop Behavior Tree
        behaviorTree.enabled = false;
        GetAnimationController().ChangeAnimationState(this.GetComponent<Animator>(), "Knockdown");
        DeathFlash();
        Destroy(this.gameObject, 5);
    }

    public AnimationController GetAnimationController()
    {
        return animationController;
    }

    #if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        Vector3 center = transform.position;
        Handles.color = Color.green;
        Handles.DrawWireDisc(center, new Vector3(0,1,0), seekRange);

        Handles.color = Color.red;
        Handles.DrawWireDisc(center, new Vector3(0,1,0), meleeRange);

        Handles.color = Color.yellow;
        Handles.DrawWireDisc(center, new Vector3(0,1,0), zoneAttackRange);
    }
    #endif
}
