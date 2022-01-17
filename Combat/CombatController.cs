using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.Timeline;
using UnityEngine.Playables;
using UnityEngine.InputSystem;
using DG.Tweening;
using System;
using UnityEngine;

[System.Serializable] public struct MeleeAttack
{
    public string attackName;
    public float attackDamage;
    public float attackStagger;
    public float attackKnockback;
}


public class CombatController : MonoBehaviour
{
    [Header("Player Combat Configuration")]
    GameObject Player;
    private AnimationController animationController;
    private Animator animator;
    private PlayerMovementController playerMovementController;
    private Body bodyController;

    public InputActionReference lightMeleeInput;
    public InputActionReference heavyMeleeInput;
    public InputActionReference jumpInput;
    public InputActionReference blockInputHold;
    //public bool blockInput = false;

    [Header("Attacks")]
    public Attack Attack001;
    public Attack Attack002;
    public Attack Attack003;
    public Attack Attack004;
    public Attack Attack005;
    public Attack Attack006;

    public static event Action<GameObject> onEnterTargetAttackState;
    public static event Action<GameObject> onCompleteTargetAttackState;
    

    private AttackEnums.Attacks currAttack;
    private bool InputWindowOpen = true;
    private int currentComboState = 0;
    public enum State{DEFENDING, OTHER};
    private State currentState;
    
    void Awake()
    {
        blockInputHold.action.started += ctx => StartBlock();
        blockInputHold.action.canceled += ctx => EndBlock();
    }

    void OnEnable()
    {
        blockInputHold.action.Enable();
        HurtBox.recievedCollision += ApplyDamagedLogic;
    }

    void OnDisable()
    {
        blockInputHold.action.Disable();
        HurtBox.recievedCollision -= ApplyDamagedLogic;
    }

    void Start()
    {
        animator = this.GetComponent<Animator>();
        animationController = this.GetComponent<AnimationController>();
        playerMovementController = this.GetComponent<PlayerMovementController>();
        bodyController = this.GetComponent<Body>();
    }

    
    void Update()
    {
        switch (currentState)
        {
            case State.DEFENDING:
                break;
            case State.OTHER:
                break;
            default:
                break;
        }

        if(lightMeleeInput.action.triggered && !animator.GetBool("Jumping") && InputWindowOpen)
        {
            switch (currentComboState)
            {
                case 0:
                    animator.SetInteger("currentComboState", currentComboState);
                    AttackStart(Attack001, false);
                    StartCoroutine(InputWindowCoroutine(.5f));
                    currentComboState += 1;
                    break;
                case 1:
                    animator.SetInteger("currentComboState", currentComboState);
                    AttackStart(Attack002, true);
                    StartCoroutine(InputWindowCoroutine(.5f));
                    currentComboState = 0;
                    
                    break;
                default:
                    break;
            }
        }
    }

    IEnumerator InputWindowCoroutine (float time) 
    {
        float elapsedTime = 0;
        InputWindowOpen = false;
        
        while (elapsedTime < time)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        InputWindowOpen = true;
    }

    //public void OnHoldBlock(InputAction.CallbackContext ctx) => blockInput = ctx.ReadValueAsButton();

    public bool GetInputWindowOpen()
    {
        return InputWindowOpen;
    }

    public int GetCurrentComboState()
    {
        return currentComboState;
    }

    public string GetCurrentAttackState()
    {
        return currAttack.ToString();
    }

    public void AttackStart(Attack attack, bool isComboTransition)
    {
        //Clear previous Attack HitBoxes
        GetComponent<HitBoxController>().DeactivateHitBoxes(currAttack);

        //Current Attack Name State Update
        currAttack = attack.attackName;

        //Play Attack sfx
        if(attack.audioClip != null){
            AudioSource audioSource = GetComponent<AudioSource>();
            audioSource.clip = attack.audioClip;
            audioSource.Play();
        }

        //Update all listeners that player is attacking
        onEnterTargetAttackState.Invoke(this.gameObject);

        //Some logic that conditionally controls attack data (damage,knockback,multihit, element etc)
        if(isComboTransition)
            animationController.UpdateAnimationState(animator, attack.animationName);
        else
            animationController.ChangeAnimationState(animator, attack.animationName);

        GetComponent<HitBoxController>().ActivateHitBoxes(attack.attackName);

       
        animator.SetBool("Attacking", true);
        playerMovementController.DisableMovement();
    }

    public void AttackComplete()
    {
        GetComponent<HitBoxController>().DeactivateHitBoxes(currAttack);

        //Update all listeners that player is done attacking
        onCompleteTargetAttackState.Invoke(this.gameObject);

        animator.SetBool("Attacking", false);
        animationController.ChangeAnimationState(animator, "Player_idle");
        playerMovementController.EnableMovement();
        currentComboState = 0;
        animator.SetInteger("currentComboState", currentComboState);
    }

    public void StartBlock()
    {
        //Ensure we complete any current attack and remove active hitboxes
        AttackComplete();
        animator.speed = 0;
        print("Blocking");
        playerMovementController.DisableMovement();
        playerMovementController.EnableSteering();
        animationController.ChangeAnimationState(animator, "Player_block_001");
        this.GetComponent<VFXController>().PlayVFX(VFXController.VFX.Block, bodyController.BlockCore, bodyController.BlockCore.position, bodyController.BlockCore.eulerAngles, 0);
        this.ChangeState(State.DEFENDING);
    }

    public void EndBlock()
    {
        print("StopBlocking");
        animator.speed = 1;
        animationController.ChangeAnimationState(animator, "Player_idle");
        playerMovementController.DisableSteering();
        playerMovementController.EnableMovement();
        this.GetComponent<VFXController>().DestroyCurrentVFX(VFXController.VFX.Block);
        this.ChangeState(State.OTHER);
    }

    public void ApplyDamagedLogic(GameObject instance, GameObject bodyPartCollidedWith, float damage, float knockback)
    {
        if(instance != this.gameObject){
            print(instance.name + " and " + this.gameObject.name);
            return;
        }
        
        Debug.Log(this.gameObject.name + " recieved Collision on : " + bodyPartCollidedWith);
        ApplyDamage(damage);
        ApplyKnockback(knockback);
        /*if(currentState == State.DEFENDING)
        {
            //Unfreeze block animation
            animator.speed = 1;

            //Play ShieldBlock VFX
            this.GetComponent<VFXController>().PlayVFX(VFXController.VFX.HitShield, bodyPartCollidedWith.transform.position, new Vector3(-90,0,0), 3f);
            
            //Apply Knockback to target (based on tranform)
            ApplyKnockback(knockback);

            //PlayBlockSFX
            GetComponent<SFXController>().PlaySFX("block");
        }*/
    }

    public void ApplyDamage(float damage)
    {       
        Debug.Log("Applying " + damage + " to " + this.gameObject.name);
    }

    public void ApplyKnockback(float knockbackAmount)
    {
        Debug.Log("Applying " + knockbackAmount + " to " + this.gameObject.name);
        GetComponent<CharacterController>().enabled = false;
        transform.DOMove(-transform.forward * knockbackAmount, .5f);
        GetComponent<CharacterController>().enabled = true;
    }

    public void ChangeState(State newState)
    {
        if (this.currentState == newState)
            return;

        this.currentState = newState;
    }


}
