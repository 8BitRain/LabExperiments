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
    private bool cooldownTriggered = false;
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
    private bool abilityConnected = false;
    private Coroutine abilityCoroutine;
    private Coroutine animationCoroutine;
    private Transform target;
    
    // Start is called before the first frame update
    private void OnEnable()
    {
        HitBox.collision += CollisionLogic;
    }

    private void OnDisable()
    {
        HitBox.collision -= CollisionLogic;
    }

    void Start()
    {
    }

    void OnDestroy()
    {
        if(!cooldownTriggered)
        {
            EngageCooldown();
        }
        Destroy(abilityInstance.gameObject);
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
        abilityInstance.transform.LookAt(Camera.main.transform.forward);
        
        //Iterate through ability instances modular components
        this.abilityCoroutine = StartCoroutine(PlayModularComponents());

        skillUsed = true;
    }


    //Called when the skill collides with a target 
    public void CollisionLogic(GameObject targetInstance, GameObject hurtBoxInstance, GameObject summonerInstance, AbilityComponent abilityComponent)
    {
        if(this.abilityInstance != summonerInstance)
            return;
        StopCoroutine(animationCoroutine);
        SetAbilityConnected(true);
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
                        if(!GetAbilityConnected())
                        {
                            animationCoroutine = StartCoroutine(AnimationDelay(abilityComponent.animationComponent));
                            yield return new WaitUntil(() => GetAbilityConnected());
                        }
                        else
                        {
                            yield return new WaitForSeconds(abilityComponent.animationComponent.animationEndDelay);
                        }
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

    public IEnumerator AnimationDelay(AnimationComponent animationComponent)
    {

        yield return new WaitForSeconds(animationComponent.animationEndDelay);
        StopCoroutine(abilityCoroutine);
        Destroy(this.gameObject);
    }

    public void PlayModularComponent(GameObject modularAbilityInstance, AbilityComponent abilityComponent)
    {

        TriggerHitBox(modularAbilityInstance, true);

        //Controls how the component travels
        switch (abilityComponent.travelDirection)
        {
            case AbilityComponent.MovementDirection.Forward:
                modularAbilityInstance.transform.DOMove(GetPlayerReference().transform.position + GetPlayerReference().transform.forward*abilityComponent.travelAmount, abilityComponent.timeToTravel);
                break;
            case AbilityComponent.MovementDirection.Backward:
                modularAbilityInstance.transform.DOMove(GetPlayerReference().transform.position - GetPlayerReference().transform.forward*abilityComponent.travelAmount, abilityComponent.timeToTravel);
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
            case AbilityComponent.MovementDirection.BackwardDiagonalLeft:
                Vector3 diagonalVector = (GetPlayerReference().transform.forward * abilityComponent.playerMovementAmount) + (GetPlayerReference().transform.right * abilityComponent.playerMovementAmount);
                GetPlayerReference().transform.DOMove(GetPlayerReference().transform.position - diagonalVector, abilityComponent.playerMovementTime);
                break;
            case AbilityComponent.MovementDirection.Up:
                GetPlayerReference().transform.DOMove(GetPlayerReference().transform.position - GetPlayerReference().transform.up*abilityComponent.playerMovementAmount, abilityComponent.playerMovementTime);
                break;
            default:
                break;
        }

        if(abilityComponent.stickToPlayer)
            modularAbilityInstance.transform.SetParent(GetPlayerReference().transform);
        
        if(abilityComponent.stickToPlayerTime != 0)
            StartCoroutine(AbilityComponentStickToPlayerCoroutine(abilityComponent.stickToPlayerTime,modularAbilityInstance.transform));
        
        if(abilityComponent.isMobile)
            modularAbilityInstance.transform.DOMove(modularAbilityInstance.transform.position + Camera.main.transform.forward * abilityComponent.travelAmount, abilityComponent.timeToTravel);

        
        if(abilityComponent.canScale)
        {
            modularAbilityInstance.transform.localScale = abilityComponent.minScaleVector;
            modularAbilityInstance.transform.DOScale(abilityComponent.maxScaleVector * abilityComponent.scaleStrength, abilityComponent.timeToScale);
        }

        TriggerHitBox(modularAbilityInstance, false);
        
        //Does player have an animation component?
        if(abilityComponent.animationComponent != null)
        {
            string animation = abilityComponent.animationComponent.animation.ToString();
            Debug.Log("Play Animation: " + animation);
            GetAnimationController().ChangeAnimationState(GetPlayerReference().GetComponent<Animator>(),animation);
        }
    
    }

    public void TriggerHitBox(GameObject modularAbilityInstance, bool isActive)
    {
        if(modularAbilityInstance.TryGetComponent<ModularAbilityComponent>(out ModularAbilityComponent modularAbilityComponent))
        {
            HitBox hitBox = modularAbilityComponent.hitBox;
            if(hitBox != null)
            {
                print("Hitbox Triggered");
                if(!isActive)
                {
                    Debug.Log("Spell Instance Name: " + modularAbilityInstance.name);
                    Debug.Log("Hitbox instance name: " + hitBox.gameObject.name);
                    EventsManager.instance.OnTriggerHitBox(hitBox.gameObject, this.abilityInstance, false, modularAbilityComponent.GetAbilityComponent().hitBoxDuration);
                }
                else
                {
                    EventsManager.instance.OnTriggerHitBox(hitBox.gameObject, this.abilityInstance, true, 0);
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
        //Animation Controller Coroutine
        GetAnimationController().PlayerAnimationLock(recoilAnimationLockTime, GetPlayerReference().GetComponent<PlayerMovementController>(), isMobileSkill);
        
        cooldownTriggered = true;
    }

    //This function unparents a spell component
    public IEnumerator AbilityComponentStickToPlayerCoroutine(float duration, Transform abilityComponent)
    {
        yield return new WaitForSeconds(duration);
        abilityComponent.SetParent(null);
    }
    

    public void SetAbilityConnected(bool hasConnected)
    {
        this.abilityConnected = hasConnected;
    }

    public bool GetAbilityConnected()
    {
        return this.abilityConnected;
    }

}
