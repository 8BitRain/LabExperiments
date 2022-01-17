using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using DG.Tweening;
using UnityEngine;

public class MoonBolt : Skill, IStats
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

    public AbilityComponents abilityComponents;
    private Dictionary<string, int> moonBoltComponents;
    private GameObject spellInstance;
    [SerializeField]
    private GameStatAmount[] _statsYouFillnInspector;

    

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
        moonBoltComponents = new Dictionary<string, int>();
        foreach (var abilityComponent in abilityComponents.abilityComponents)
        {
            print(abilityComponent.name);
            print(abilityComponent.index);
            moonBoltComponents.Add(abilityComponent.name, abilityComponent.index);
        }
    }

    public void OnSkillInput(InputAction.CallbackContext ctx) => skillInput = ctx.ReadValueAsButton();

    public bool TryGetStat(GameStat stat, out float amount)
    {
        amount = 0;
        for (int statIndex = 0; statIndex < _statsYouFillnInspector.Length; statIndex++)
        {
            var checkedStat = _statsYouFillnInspector[statIndex];
            if (checkedStat.TheStat != stat)
                continue;
                
            amount = checkedStat.Amount;
            return true;
        }
        return false;
    }

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
        GetPlayerReference().transform.DOMove(GetPlayerReference().transform.position + GetPlayerReference().transform.up*3.0f, 3.0f);

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

    private void OnTriggerEnter(Collider other)
    {
        print("Collided with " + other.gameObject.name);
        foreach (var layer in layers)
        {
            //Collision w/ defined layer(s)
            if(layer == (layer | (1 << other.gameObject.layer)))
            {
                spellInstance.transform.GetChild(moonBoltComponents["Collision"]).gameObject.SetActive(true);
            }
        }
    }

    /*public void RefreshSkill(GameObject instance, Cooldown cooldown)
    {
        if (this.gameObject != instance)
            return;
        
        //This line may be unnescesary
        if(cooldown.skillName == Skill.Name.MoonBolt)
        {
            print("Refreshing skill: " + cooldown.name + ": " + cooldown.activeSkill.name);
            skillUsed = false;
            fireSkill = false;
        }
    }*/

    public IEnumerator SpellDelay(float duration, GameObject Spell)
    {
        yield return new WaitForSeconds(duration);
        //BlowBack Move player backwards
        GetPlayerReference().transform.LookAt(GetPlayerReference().transform.position + Camera.main.transform.forward);
        GetPlayerReference().transform.DOMove(GetPlayerReference().transform.position -GetPlayerReference().transform.forward * 30.0f, 1.5f);
        
        print("Moonbolt casting");
        spellInstance = Instantiate(Spell, GetSkillSpawnPosition().position, GetSkillSpawnPosition().rotation);
        spellInstance.transform.LookAt(spellInstance.transform.position + Camera.main.transform.forward);

        //Grab chargeLineComponent of Beam
        Transform beam = spellInstance.transform.GetChild(moonBoltComponents["Beam"]);
        Transform chargeLine = spellInstance.transform.GetChild(moonBoltComponents["ChargeLine"]);
        Transform particleBeam = spellInstance.transform.GetChild(moonBoltComponents["ParticleBeam"]);

        //beam.DOMove(spellInstance.transform.position + Camera.main.transform.forward  * 15, 2.0f);
        beam.DOScaleY(28.0f, 2f);
        //chargeLine.DOMove(spellInstance.transform.position + Camera.main.transform.forward  * 15, 2.0f);
        chargeLine.DOScaleY(4.0f, 2f);
        //particleBeam.DOMove(spellInstance.transform.position + Camera.main.transform.forward  * 15, 2.0f);
        particleBeam.DOScaleZ(4.0f, 2f);
        //spellInstance.transform.DOMove(transform.position + transform.forward  * 15, 2.0f);
        //spellInstance.transform.DOScaleZ(4.0f, 2f);
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

}
