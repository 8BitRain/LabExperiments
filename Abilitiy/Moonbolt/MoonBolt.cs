using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using DG.Tweening;
using UnityEngine;

public class MoonBolt : Skill
{
    public enum AnimationState
    {
        MoonBoltChannel,
        MoonBoltCast,
        MoonBoltRecoil
    }

    [Header("Skill Settings")]
    public float cooldownTime;
    public Cooldown cooldownObjRef;
    public Skill.CooldownBar cooldownBarPosition;
    public InputActionReference skillInputButton;
    public bool isMobileSkill = false;
    public bool isHeldSkill = false;
    private bool skillInput;

    [Header("Channel Animation")]
    public MoonBolt.AnimationState channel;
    [Header("Cast Animation")]
    public MoonBolt.AnimationState cast;
    [Header("Recoil Animation")]
    public MoonBolt.AnimationState recoil;

    [Header("Skill Components")]
    public GameObject moonBoltBeam;
    [Range(0f, 1.5f)]
    public float moonBoltBeamSpawnDelay;

    [Header("Player Settings")]
    [Range(0f, 3.0f)]
    public float recoilAnimationLockTime;

    [Header("Inspector Commands")]
    public bool fireSkill = false;

    public LayerMask[] layers;

    private GameObject spellInstance;

    

    private void OnEnable()
    {
        //CooldownController.onRefreshCooldown += RefreshSkill;
    }

    private void OnDisable()
    {
        //CooldownController.onRefreshCooldown -= RefreshSkill;
    }

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

        //Update Player Animation for Moonbolt
        GetAnimationController().ChangeAnimationState(GetPlayerReference().GetComponent<Animator>(),cast.ToString());
        //GetPlayerReference().transform.DOMove(GetPlayerReference().transform.position + GetPlayerReference().transform.up*1.5f, 3.0f);

        //Beam shoots out from hand after short delay
        StartCoroutine(SpellDelay(moonBoltBeamSpawnDelay, moonBoltBeam));

        skillUsed = true;
    }

    public void EngageCooldown()
    {
        Cooldown cooldownInstance = Instantiate(cooldownObjRef, transform.position, transform.rotation);
        cooldownInstance.time = cooldownTime;
        cooldownInstance.activeSkill = this.gameObject;
        cooldownInstance.cooldownBarPosition = cooldownBarPosition;

        GetCooldownController().StartCooldown(cooldownInstance);
        GetAnimationController().ChangeAnimationState(GetPlayerReference().GetComponent<Animator>(),recoil.ToString());

        //Unlock player movement
        StartCoroutine(PlayerAnimationLock(recoilAnimationLockTime, GetPlayerReference().GetComponent<PlayerMovementController>()));

    }

    public void CollisionLogic()
    {

    }

    public void PlayModularComponent(GameObject spellInstance, AbilityComponent abilityComponent)
    {
        if(abilityComponent.stickToPlayer)
            spellInstance.transform.SetParent(GetPlayerReference().transform);
        
        if(abilityComponent.stickToPlayerTime != 0)
            StartCoroutine(AbilityComponentStickToPlayerCoroutine(abilityComponent.stickToPlayerTime,spellInstance.transform));
        
        if(abilityComponent.isMobile)
            spellInstance.transform.DOMove(spellInstance.transform.position + Camera.main.transform.forward * abilityComponent.travelSpeed, abilityComponent.timeToTravel);

        
        if(abilityComponent.canScale)
        {
            spellInstance.transform.localScale = abilityComponent.minScaleVector;
            spellInstance.transform.DOScale(abilityComponent.maxScaleVector * abilityComponent.scaleStrength, abilityComponent.timeToScale);
        }

        TriggerHitBox(spellInstance, false);
        
    }

    public void TriggerHitBox(GameObject spellInstance, bool isActive)
    {
        if(spellInstance.TryGetComponent<ModularAbilityComponent>(out ModularAbilityComponent modularAbilityComponent))
        {
            HitBox hitBox = modularAbilityComponent.hitBox;
            if(hitBox != null)
            {
                print("Hitbox Triggered");
                if(!isActive)
                {
                    Debug.Log("Spell Instance Name: " + spellInstance.name);
                    Debug.Log("Hitbox instance name: " + hitBox.gameObject.name);
                    EventsManager.instance.OnTriggerHitBox(hitBox.gameObject, false, modularAbilityComponent.GetAbilityComponent().hitBoxTime);
                }
                    
            }
        }

    }

    public IEnumerator SpellDelay(float duration, GameObject Spell)
    {
        yield return new WaitForSeconds(duration);
        //BlowBack Move player backwards
        GetPlayerReference().transform.LookAt(GetPlayerReference().transform.position + Camera.main.transform.forward);
        GetPlayerReference().transform.DOMove(GetPlayerReference().transform.position - GetPlayerReference().transform.forward * 30.0f, 1.5f);
        
        print("Moonbolt casting");
        spellInstance = Instantiate(Spell, GetSkillSpawnPosition().position, GetSkillSpawnPosition().rotation);
        spellInstance.transform.LookAt(spellInstance.transform.position + Camera.main.transform.forward);

        PlayModularComponent(spellInstance, spellInstance.GetComponent<IAbilityComponent>().GetAbilityComponent());
        //Iterate through ability container * components
        foreach (Transform modularComponent in spellInstance.GetComponentsInChildren<Transform>())
        {
            if(modularComponent.TryGetComponent<IAbilityComponent>(out IAbilityComponent modularAbilityComponent))
            {
                PlayModularComponent(modularComponent.gameObject, modularAbilityComponent.GetAbilityComponent());
            }
        }
        EngageCooldown();
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
