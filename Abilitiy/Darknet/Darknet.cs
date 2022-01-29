using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using DG.Tweening;
using UnityEngine;

public class Darknet : MonoBehaviour
{
    public enum AnimationState
    {
        Channel,
        Cast,
        Recoil
    }

    public Skill.Name skillName;
    [Header("Skill Settings")]
    public GameObject playerReference;
    public float cooldownTime;
    public Cooldown cooldownObjRef;
    public Skill.CooldownBar cooldownBarPosition;
    public InputActionReference skillInputButton;
    private bool skillInput;

    [Header("Controllers")]
    public AnimationController animController;
    public CooldownController cooldownController;

    [Header("Channel Animation")]
    public Darknet.AnimationState channel;
    [Header("Cast Animation")]
    public Darknet.AnimationState cast;
    [Header("Recoil Animation")]
    public Darknet.AnimationState recoil;

    
    [Header("Skill Components")]
    public GameObject chargeSphere;
    public GameObject chargeLine;
    public GameObject chargeParticle;

    public Transform chargeSphereSpawn;
    public Transform chargeLineSpawn;
    public Transform chargeParticleSpawn;
    public Transform skillSpawn;

    public GameObject abilityTargeter;
    private GameObject abilityTargeterInstance;

    [Header("Charge Line Settings")]
    public float maxHeight;

    [Header("Player Settings")]
    [Range(0f, 1.5f)]
    public float recoilAnimationLockTime;

    public bool fireSkill = false;

    public bool skillUsed = false;

    

    private void OnEnable()
    {
        //CooldownController.onRefreshCooldown += RefreshSkill;
        MobileAbilityTargetingSystem.onSkillCast += UseSkill;
    }

    private void OnDisable()
    {
        //CooldownController.onRefreshCooldown -= RefreshSkill;
        MobileAbilityTargetingSystem.onSkillCast -= UseSkill;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //This skill activates and is the controller for the Moonshot skill
        //The moonshot skill rises slowly from kosem's rod with a slow speed. The moonshot skill is fired at a fast rate towards a target then explodes
        //1. Set up a listener for player input from input system
        //2. Movement will be orchestrated by Dotween. The player characther will be moved via this script.
        //3. Have the skill play an animation on the player, defined by the user in the inspector, an enum that defines the animation to play

        if((skillInputButton.action.triggered || fireSkill) && !skillUsed)
        {
            //Option 1: Targeting ability, so Ensure player movement is locked
            playerReference.GetComponent<PlayerMovementController>().DisableMovement();
            print("skillInput: " + this.skillName);
            skillUsed = true;

            //This is a targeted ability, so summon the ability targeter.
            abilityTargeterInstance = Instantiate(abilityTargeter, transform.position, Quaternion.identity);
            abilityTargeterInstance.GetComponent<MobileAbilityTargetingSystem>().playerReference = playerReference;
            abilityTargeterInstance.transform.rotation = Quaternion.Euler(-90,0,0);
        }
        

    }

    public void OnSkillInput(InputAction.CallbackContext ctx) => skillInput = ctx.ReadValueAsButton();

    public void UseSkill(GameObject instance)
    {
        if(abilityTargeterInstance != instance)
        {
            return;
        }

        //Destroy Targeter
        Destroy(abilityTargeterInstance);
        //Power begins forming around the staves crystal
        playerReference.transform.DOMove(playerReference.transform.position + playerReference.transform.up*3.0f, 3.0f);
        animController.ChangeAnimationState(playerReference.GetComponent<Animator>(),channel.ToString());
        GameObject chargeLineInstance = Instantiate(chargeLine, chargeLineSpawn.transform.position, chargeLineSpawn.transform.rotation);
        chargeLineInstance.transform.SetParent(playerReference.transform);
        chargeLineInstance.transform.localScale = new Vector3(0,0,0);
        chargeLineInstance.transform.DOScale(Vector3.one, 2.0f).OnComplete(() => {
            //Luna Sphere appears at the location of the charged lines
            GameObject chargeSphereInstance = Instantiate(chargeSphere, chargeLineInstance.transform.position, chargeLineInstance.transform.rotation);
            chargeSphereInstance.transform.SetParent(playerReference.transform);
            chargeSphereInstance.transform.localScale = new Vector3(0,0,0);
            chargeSphereInstance.transform.DOScale(Vector3.one*10, 4.0f);

            //Luna Sphere rises into sky
            chargeLineInstance.transform.DOMove(chargeLineInstance.transform.position + chargeLineInstance.transform.up*maxHeight, 2.0f).OnComplete(() => {
                Destroy(chargeLineInstance.gameObject);
            });
            chargeSphereInstance.transform.DOMove(chargeSphereInstance.transform.position + chargeSphereInstance.transform.up*maxHeight, 2.0f).OnComplete(() => {
                //Fall from sky as skill completes
                //Destroy(chargeLineInstance);
                chargeSphereInstance.transform.DOMoveZ(15, 2).SetEase(Ease.OutQuad);
                chargeSphereInstance.transform.DOMoveY(5, 2).SetEase(Ease.InQuad);
                EngageCooldown();
            });
        });
    }

    public void EngageCooldown()
    {
        Cooldown cooldownInstance = Instantiate(cooldownObjRef, transform.position, transform.rotation);
        cooldownInstance.time = cooldownTime;
        cooldownInstance.activeSkill = this.gameObject;
        cooldownInstance.cooldownBarPosition = cooldownBarPosition;

        cooldownController.StartCooldown(cooldownInstance);
        animController.ChangeAnimationState(playerReference.GetComponent<Animator>(),recoil.ToString());

        //Unlock player movement
        StartCoroutine(PlayerAnimationLock(recoilAnimationLockTime, playerReference.GetComponent<PlayerMovementController>()));
    }

    public void RefreshSkill(GameObject instance, Cooldown cooldown)
    {
        if (this.gameObject != instance)
            return;
        
        
        if(cooldown.skillName == Skill.Name.Darknet)
        {
            print("Refreshing skill: " + cooldown.name + ": " + cooldown.activeSkill.name);
            skillUsed = false;
            fireSkill = false;
        }
    }

    //This function acts as an animation lock. Spells can have recoil animations that are either cancellable or not cancellable. 
    public IEnumerator PlayerAnimationLock(float duration, PlayerMovementController player)
    {
        yield return new WaitForSeconds(duration);
        player.EnableMovement();
    }

}
