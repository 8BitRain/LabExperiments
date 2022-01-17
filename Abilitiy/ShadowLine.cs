using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using DG.Tweening;
using UnityEngine;

public class ShadowLine : MonoBehaviour
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
    public ShadowLine.AnimationState channel;
    [Header("Cast Animation")]
    public ShadowLine.AnimationState cast;
    [Header("Recoil Animation")]
    public ShadowLine.AnimationState recoil;

    
    [Header("Skill Components")]
    public GameObject LineCastTargeter;
    public GameObject TeleportationStartVFX;
    public GameObject TeleportationInProgressVFX;
    public GameObject TeleportaionEndVFX;

    [Header("Charge Line Settings")]
    public float maxHeight;

    public bool fireSkill = false;

    public bool skillUsed = false;

    private void OnEnable()
    {
        CooldownController.onRefreshCooldown += RefreshSkill;
    }

    private void OnDisable()
    {
        CooldownController.onRefreshCooldown -= RefreshSkill;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //1. Skill starts by instantiating a line that will show the total distance the player will move
        //2. Once the player pull the right trigger, teleport the player 15m to the distance they selected
        //3. If there is currently a spell on the field, and that spell is in the trajectory of the dash, that spell can be moved???
        //4. Player animation should be a warp. They blink from their position, (maybe they turn invisible), and the camera quickly follows them forward.

        if((skillInputButton.action.triggered || fireSkill) && !skillUsed)
        {
            print("skillInput: " + this.skillName);
            skillUsed = true;


        }
        

    }

    public void OnSkillInput(InputAction.CallbackContext ctx) => skillInput = ctx.ReadValueAsButton();

    public void UseSkill(GameObject instance)
    {
    }

    public void EngageCooldown()
    {
        Cooldown cooldownInstance = Instantiate(cooldownObjRef, transform.position, transform.rotation);
        cooldownInstance.time = cooldownTime;
        cooldownInstance.activeSkill = this.gameObject;
        cooldownInstance.cooldownBarPosition = cooldownBarPosition;

        cooldownController.StartCooldown(cooldownInstance);
        animController.ChangeAnimationState(playerReference.GetComponent<Animator>(),recoil.ToString());
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

}
