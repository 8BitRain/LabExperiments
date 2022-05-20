using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;
using System;
using UnityEngine.UI;
using UnityEngine;
using DG.Tweening;

public class AbilityController : MonoBehaviour
{
    [Header("Ability Settings")]
    public Transform Agent;
    public Transform abilitySpawn;
    public GameObject selector;
    public enum SelectedButton {North, South, East, West};
    public SelectedButton selectedButton;


    [Header("Ability Prefabs")]
    public Skill specialAbilitySouth;
    public Skill specialAbilityNorth;
    public Skill specialAbilityEast;
    public Skill specialAbilityWest;

    
    private Skill abilityNorthInstance;
    private Skill abilitySouthInstance;
    private Skill abilityEastInstance;
    private Skill abilityWestInstance;

    //2 means skill is actively being used
    //1 means ability is in cooldown status
    //0 means ability has cooleddown
    private int abilityA = 0;
    private int abilityB = 0;
    private int abilityC = 0;
    private int abilityD = 0;

    [Header("Block Settings")]
    public GameObject VFXBlock;
    public Transform VFXBlockSpawn;
    private GameObject VFXBlockInstance;

    [Header("Dodge Settings")]
    public float dodgeDistance = 10f;
    public float dodgeDuration = .5f;
    public float dodgeTweenTimer = .5f;
    public float dodgeCooldown = 1f;

    [Header("Input Settings")]
    /// <summary>Vector2 action for pressing a face button </summary>
    [Tooltip("Vector2 action for South Button ")]
    public InputActionReference SouthButtonPressed;

    /// <summary>Vector2 action for pressing a face button </summary>
    [Tooltip("Vector2 action for North Button ")]
    public InputActionReference NorthButtonPressed;

    /// <summary>Vector2 action for pressing a face button </summary>
    [Tooltip("Vector2 action for West Button ")]
    public InputActionReference WestButtonPressed;

    /// <summary>Vector2 action for pressing a face button </summary>
    [Tooltip("Vector2 action for East Button ")]
    public InputActionReference EastButtonPressed;

    private bool gaurdInput = false;
    public void OnGaurd(InputAction.CallbackContext ctx) => gaurdInput = ctx.ReadValueAsButton();

    private bool displayAbilityWindowInput = false;
    public void OnDisplayAbilityWindow(InputAction.CallbackContext ctx) => displayAbilityWindowInput = ctx.ReadValueAsButton();

    public static event Action<GameObject, bool> onActivateAbilityWindow;

    //State variables
    bool dodgeCooldownIsActive = false;

    private void OnEnable()
    {
        CooldownController.onUpdateCooldown += UpdateCooldown;
    }

    private void OnDisable()
    {
        CooldownController.onUpdateCooldown -= UpdateCooldown;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }


    // Update is called once per frame
    void Update()
    {
        if(displayAbilityWindowInput)
        {
            onActivateAbilityWindow.Invoke(this.gameObject, true);
            EventsManager.instance.OnAbilityWindowActiveLockInput(this.gameObject);
            if(NorthButtonPressed.action.triggered)
            {
                selectedButton = SelectedButton.North;
                Debug.Log("UI: North Button pressed");
                UseAbility();
                //UI Hook to communicate w/ HUD selector update
            }

            if(SouthButtonPressed.action.triggered)
            {
                selectedButton = SelectedButton.South;
                Debug.Log("UI: South Button pressed");
                //UseAbility();
                //UI Hook to communicate w/ HUD selector update
            }

            if(EastButtonPressed.action.triggered)
            {
                selectedButton = SelectedButton.East;
                Debug.Log("UI: East Button pressed");
                UseAbility();
                //UI Hook to communicate w/ HUD selector update
            }

            if(WestButtonPressed.action.triggered)
            {
                selectedButton = SelectedButton.West;
                Debug.Log("UI: West Button pressed");
                //UseAbility();
                //UI Hook to communicate w/ HUD selector update
            }
        }  
        else
        {
            onActivateAbilityWindow.Invoke(this.gameObject, false);
            EventsManager.instance.OnAbilityWindowInactiveUnlockInput(this.gameObject);
        }

        if(gaurdInput && !GetComponent<Animator>().GetBool("Dodging") && !GetComponent<Animator>().GetBool("Attacking") && !GetComponent<Animator>().GetBool("Damaged"))
        {
            GetComponent<Animator>().SetBool("Gaurding", true);
            Gaurd();
        }
        
        if(!gaurdInput)
        {
            Debug.Log("Stop Gaurding");
            GetComponent<Animator>().SetBool("Gaurding", false);

            if(!GetComponent<Animator>().GetBool("Dodging") && !GetComponent<Animator>().GetBool("Attacking") && !GetComponent<Animator>().GetBool("Damaged"))
            {
                GetMovementController().DisableApplyGravityLockPlayerInput();
                GetMovementController().EnableMovement();
            }

            if(VFXBlockInstance != null)
            {
                Destroy(VFXBlockInstance);
            }
        }

        if(gaurdInput && GetMovementController().movementInput.magnitude != 0)
        { 
            Dodge();
        }
    }

    public void Gaurd()
    {
        GetMovementController().DisableMovement();
        GetAnimationController().ChangeAnimationState(this.GetComponent<Animator>(), DefenseAnimations.AnimationState.Gaurd.ToString());

        if(VFXBlockInstance == null)
        {
            VFXBlockInstance = Instantiate(VFXBlock, VFXBlockSpawn.position, VFXBlockSpawn.rotation);
        }
    }

    public void Dodge()
    {
        if(this.dodgeCooldownIsActive)
            return;
        
        if(this.VFXBlockInstance != null)
        {
            Destroy(VFXBlockInstance);
        }

        //Disable Hurtbox
        this.GetComponent<Body>().GetHurtBox().gameObject.SetActive(false);

        GetMovementController().DisableApplyGravityLockPlayerInput();
        GetMovementController().DisableMovement();

        GetComponent<Animator>().SetBool("Dodging", true);
        GetComponent<Animator>().SetBool("Gaurding", false);

        float thumbstickX = GetMovementController().movementInput.x;
        float thumbstickY = GetMovementController().movementInput.y;
        
        //Debug.Log("Movement Input values: " + "X: " + thumbstickX + " Y: " + thumbstickY);


        float targetAngle = Mathf.Atan2(thumbstickX, thumbstickY) * Mathf.Rad2Deg + GetComponent<CameraController>().GetCameraInstance().eulerAngles.y;
        transform.rotation = Quaternion.Euler(0f, targetAngle, 0f);
        Vector3 movementDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
        GetAnimationController().ResetAnimationState();

        //if block is held down the block animation and run animation can clip
        DOVirtual.DelayedCall(.1f, () => {
            GetAnimationController().ChangeAnimationState(this.GetComponent<Animator>(), DefenseAnimations.AnimationState.Dodge_F.ToString());
            GetMovementController().transform.DOMove(GetMovementController().transform.position + movementDirection*dodgeDistance, dodgeTweenTimer);
        });

        this.dodgeCooldownIsActive = true;
        
        //Dodge Duration
        DOVirtual.DelayedCall(dodgeDuration, () => {
            GetComponent<Animator>().SetBool("Dodging", false);
            GetMovementController().EnableMovement();

            //Enable HitBox
            this.GetComponent<Body>().GetHurtBox().gameObject.SetActive(true);

        });

        //Dodge Cooldown
        DOVirtual.DelayedCall(dodgeCooldown, () => {
            this.dodgeCooldownIsActive = false;
        });
        


    }

    public void UseAbility()
    {
        //Prevents skill from being fired while a skill is in cooldown. Value stays at 1 until cooldown is complete
        //We want to adjust this by changing the value of abilityA,B,C,D
        if(abilityA == 2 || abilityB == 2 || abilityC == 2 || abilityD == 2)
            return;

        switch (selectedButton)
        {
            case SelectedButton.North:
                if(abilityA == 1)
                {
                    print("Skill in Cooldown");
                    return;
                }
                abilityNorthInstance = Instantiate(specialAbilityNorth);
                InitializeAbility(abilityNorthInstance);
                abilityNorthInstance.UseSkill();
                abilityA = 2;
                return;
            case SelectedButton.South:
                if(abilityB == 1)
                {
                    print("Skill in Cooldown");
                    return;
                }
                abilitySouthInstance = Instantiate(specialAbilitySouth);
                InitializeAbility(abilitySouthInstance);
                abilitySouthInstance.UseSkill();
                abilityB = 2;
                return;
            case SelectedButton.East:
                if(abilityC == 1)
                {
                    print("Skill in Cooldown");
                    return;
                }
                abilityEastInstance = Instantiate(specialAbilityEast);
                InitializeAbility(abilityEastInstance);
                abilityEastInstance.UseSkill();
                //Destroy(abilityEastInstance.gameObject, 1.5f);
                abilityC = 2;
                return;
            case SelectedButton.West:
                if(abilityD == 1)
                {
                    print("Skill in Cooldown");
                    return;
                }
                abilityWestInstance = Instantiate(specialAbilityWest);
                InitializeAbility(abilityWestInstance);
                abilityWestInstance.UseSkill();
                abilityD = 2;
                return;
            default:
                return;
        }
    }

    public void InitializeAbility(Skill ability)
    {
        ability.SetPlayerReference(this.gameObject);
        ability.SetPlayerAnimationController(this.gameObject.GetComponent<AnimationController>());
        ability.SetCooldownController(this.gameObject.GetComponent<CooldownController>());
        ability.SetSkillSpawnPoint(this.abilitySpawn);
    }

    public void UpdateCooldown(GameObject instance, Cooldown cooldown, int cooldownStateID)
    {
        if (this.gameObject != instance)
            return;
        print("Refreshing ABility: " + cooldown.cooldownBarPosition);
        switch(cooldown.cooldownBarPosition)
        {
            case Skill.CooldownBar.A:
                Debug.Log("Updating Ability A to state: " + cooldownStateID);
                abilityA = cooldownStateID;
                break;
            case Skill.CooldownBar.B:
                Debug.Log("Updating Ability B to state: " + cooldownStateID);
                abilityB = cooldownStateID;
                break;
            case Skill.CooldownBar.C:
                Debug.Log("Updating Ability C to state: " + cooldownStateID);
                abilityC = cooldownStateID;
                break;
            case Skill.CooldownBar.D:
                Debug.Log("Updating Ability D to state: " + cooldownStateID);
                abilityD = cooldownStateID;
                break;
            default: 
                break;
        }
    }

    public AnimationController GetAnimationController()
    {
        return this.GetComponent<AnimationController>();
    }
    
    public PlayerMovementController GetMovementController()
    {
        return this.GetComponent<PlayerMovementController>();
    }
}
