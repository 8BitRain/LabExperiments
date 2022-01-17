using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using DG.Tweening;
using UnityEngine;

public class Skill001_MoonBolt : MonoBehaviour
{
    public enum AnimationState
    {
        Channel,
        Cast,
        Recoil
    }

    public enum Name
    {
        MoonBolt
    }
    [Header("Skill Settings")]
    public GameObject playerReference;
    public Skill.Name skillName;
    public float cooldownTime;
    public Cooldown cooldownObjRef;
    public InputActionReference skillInputButton;
    private bool skillInput;

    [Header("Controllers")]
    public AnimationController animController;
    public CooldownController cooldownController;

    [Header("Channel Settings")]
    public Skill001_MoonBolt.AnimationState channel;
    [Header("Cast Settings")]
    public Skill001_MoonBolt.AnimationState cast;
    [Header("Recoil Settings")]
    public Skill001_MoonBolt.AnimationState recoil;

    
    [Header("Skill Components")]
    public GameObject chargeSphere;
    public GameObject chargeLine;
    public GameObject chargeParticle;

    public Transform chargeSphereSpawn;
    public Transform chargeLineSpawn;
    public Transform chargeParticleSpawn;
    public Transform skillSpawn;

    [Header("Charge Line Settings")]
    public float maxHeight;

    public bool fireSkill = false;

    private bool skillUsed = false;

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
        //This skill activates and is the controller for the Moonshot skill
        //The moonshot skill rises slowly from kosem's rod with a slow speed. The moonshot skill is fired at a fast rate towards a target then explodes
        //1. Set up a listener for player input from input system
        //2. Movement will be orchestrated by Dotween. The player characther will be moved via this script.
        //3. Have the skill play an animation on the player, defined by the user in the inspector, an enum that defines the animation to play

        /*if(skillInput || fireSkill && !skillUsed)
        {
            print("skillInput");
            skillUsed = true;
            UseSkill();
            

        }*/

        if(skillInputButton.action.triggered || fireSkill && !skillUsed)
        {
            print("skillInput");
            skillUsed = true;
            UseSkill();
        }
        

    }

    public void OnSkillInput(InputAction.CallbackContext ctx) => skillInput = ctx.ReadValueAsButton();

    public void UseSkill()
    {
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

        cooldownController.StartCooldown(cooldownInstance);
        animController.ChangeAnimationState(playerReference.GetComponent<Animator>(),recoil.ToString());
    }

    public void RefreshSkill(GameObject instance, Cooldown cooldown)
    {
        if (this.gameObject != instance)
            return;
        
        
        if(cooldown.skillName == Skill.Name.MoonBolt)
        {
            print("Refreshing skill: " + cooldown.name + ": " + cooldown.activeSkill.name);
            skillUsed = false;
            fireSkill = false;
        }
    }

}
