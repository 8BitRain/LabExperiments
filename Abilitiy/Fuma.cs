using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using DG.Tweening;
using UnityEngine;

public class Fuma : Skill
{
    [Header("Skill Settings")]
    public float cooldownTime;
    public Cooldown cooldownObjRef;
    public Skill.CooldownBar cooldownBarPosition;
    public bool isMobileSkill = false;
    public bool isHeldSkill = false;
    private bool skillInput;

    [Header("Skill Components")]
    public GameObject FumaAbilityComponent;

    [Header("Player Settings")]
    [Range(0f, 3.0f)]
    public float recoilAnimationLockTime;

    [Header("Inspector Commands")]
    public bool fireSkill = false;

    public LayerMask[] layers;

    private GameObject abilityInstance;

    // Start is called before the first frame update
    void Start()
    {
    }

    public void OnSkillInput(InputAction.CallbackContext ctx) => skillInput = ctx.ReadValueAsButton();

    public override void UseSkill()
    {
        //If the skill is current in use, return, we don't need to activate teh skill again
        if(skillUsed)
            return;

        if(isMobileSkill)
        {
            //Lock Player RightSideFaceButtonActions
            GetPlayerReference().GetComponent<PlayerMovementController>().LockInputRightSideControllerFaceButtons(this.gameObject);
        }
        else
        {
            //Lock Player Movement
            GetPlayerReference().GetComponent<PlayerMovementController>().DisableMovement();
            //GetPlayerReference().GetComponent<PlayerMovementController>().EnableSteering();
        }

        //Instantiate ability
        abilityInstance = Instantiate(FumaAbilityComponent, GetSkillSpawnPosition().position, GetSkillSpawnPosition().rotation);
        
        //Iterate through ability instances modular components
        StartCoroutine(PlayModularComponents());

        skillUsed = true;
    }



    public void CollisionLogic()
    {

    }

    public IEnumerator PlayModularComponents()
    {
        foreach (Transform modularComponent in abilityInstance.GetComponentsInChildren<Transform>())
        {
            if(modularComponent.TryGetComponent<IAbilityComponent>(out IAbilityComponent modularAbilityComponent))
            {
                AbilityComponent abilityComponent = modularAbilityComponent.GetAbilityComponent();
                PlayModularComponent(modularComponent.gameObject, modularAbilityComponent.GetAbilityComponent());

                //Animation Delay Logic
                if(abilityComponent.animationComponent != null)
                {
                    if(abilityComponent.animationComponent.animationEndDelay != 0)
                    {
                        Debug.Log("Animation Delay: " + abilityComponent.animationComponent.animation + " for: " + abilityComponent.animationComponent.animationEndDelay);
                        yield return new WaitForSeconds(abilityComponent.animationComponent.animationEndDelay);
                    }
                    else
                    {
                        yield return null;
                    }
                }
            }
        }
        EngageCooldown();
    }

    public void PlayModularComponent(GameObject abilityInstance, AbilityComponent abilityComponent)
    {

        TriggerHitBox(abilityInstance, true);

        //Controls how the component travels
        switch (abilityComponent.travelDirection)
        {
            case AbilityComponent.MovementDirection.Forward:
                abilityInstance.transform.DOMove(GetPlayerReference().transform.position + abilityInstance.transform.forward*abilityComponent.travelAmount, abilityComponent.timeToTravel);
                break;
            case AbilityComponent.MovementDirection.Backward:
                abilityInstance.transform.DOMove(GetPlayerReference().transform.position - abilityInstance.transform.forward*abilityComponent.travelAmount, abilityComponent.timeToTravel);
                break;
            default:
                break;
        }

        //Controls how the player travels
        switch (abilityComponent.playerMovementDirection)
        {
            case AbilityComponent.MovementDirection.Forward:
                GetPlayerReference().transform.DOMove(GetPlayerReference().transform.position + GetPlayerReference().transform.forward*abilityComponent.playerMovementAmount, abilityComponent.playerMovementTime);
                break;
            case AbilityComponent.MovementDirection.Backward:
                GetPlayerReference().transform.DOMove(GetPlayerReference().transform.position - GetPlayerReference().transform.forward*abilityComponent.playerMovementAmount, abilityComponent.playerMovementTime);
                break;
            default:
                break;
        }

        if(abilityComponent.stickToPlayer)
            abilityInstance.transform.SetParent(GetPlayerReference().transform);
        
        if(abilityComponent.stickToPlayerTime != 0)
            StartCoroutine(AbilityComponentStickToPlayerCoroutine(abilityComponent.stickToPlayerTime,abilityInstance.transform));
        
        if(abilityComponent.isMobile)
            abilityInstance.transform.DOMove(abilityInstance.transform.position + Camera.main.transform.forward * abilityComponent.travelSpeed, abilityComponent.timeToTravel);

        
        if(abilityComponent.canScale)
        {
            abilityInstance.transform.localScale = abilityComponent.minScaleVector;
            abilityInstance.transform.DOScale(abilityComponent.maxScaleVector * abilityComponent.scaleStrength, abilityComponent.timeToScale);
        }

        TriggerHitBox(abilityInstance, false);
        
        //Does player have an animation component?
        if(abilityComponent.animationComponent != null)
        {
            string animation = abilityComponent.animationComponent.animation.ToString();
            Debug.Log("Play Animation: " + animation);
            GetAnimationController().ChangeAnimationState(GetPlayerReference().GetComponent<Animator>(),animation);
        }
    
    }

    public void TriggerHitBox(GameObject abilityInstance, bool isActive)
    {
        if(abilityInstance.TryGetComponent<ModularAbilityComponent>(out ModularAbilityComponent modularAbilityComponent))
        {
            HitBox hitBox = modularAbilityComponent.hitBox;
            if(hitBox != null)
            {
                print("Hitbox Triggered");
                if(!isActive)
                {
                    Debug.Log("Spell Instance Name: " + abilityInstance.name);
                    Debug.Log("Hitbox instance name: " + hitBox.gameObject.name);
                    EventsManager.instance.OnTriggerHitBox(hitBox.gameObject, false, modularAbilityComponent.GetAbilityComponent().hitBoxDuration);
                }
                else
                {
                    EventsManager.instance.OnTriggerHitBox(hitBox.gameObject, true, 0);
                }
                    
            }
        }

    }
    public IEnumerator Delay(float duration)
    {
        yield return new WaitForSeconds(duration);
    }

    public void EngageCooldown()
    {
        Cooldown cooldownInstance = Instantiate(cooldownObjRef, transform.position, transform.rotation);
        cooldownInstance.time = cooldownTime;
        cooldownInstance.activeSkill = this.gameObject;
        cooldownInstance.cooldownBarPosition = cooldownBarPosition;

        GetCooldownController().StartCooldown(cooldownInstance);
        //GetAnimationController().ChangeAnimationState(GetPlayerReference().GetComponent<Animator>(),recoil.ToString());

        //Unlock player movement
        StartCoroutine(PlayerAnimationLock(recoilAnimationLockTime, GetPlayerReference().GetComponent<PlayerMovementController>()));

    }

    //This function acts as an animation lock. Spells can have recoil animations that are either cancellable or not cancellable. 
    public IEnumerator PlayerAnimationLock(float duration, PlayerMovementController player)
    {
        yield return new WaitForSeconds(duration);
        if(isMobileSkill)
        {
            player.UnlockInputRightSideControllerFaceButtons(this.gameObject);
        }
        else
        {
            player.EnableMovement();
            //player.DisableSteering();
        } 
    }

    //This function unparents a spell component
    public IEnumerator AbilityComponentStickToPlayerCoroutine(float duration, Transform abilityComponent)
    {
        yield return new WaitForSeconds(duration);
        abilityComponent.SetParent(null);
    }

}
