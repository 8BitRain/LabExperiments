using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;
using System;
using UnityEngine.UI;
using UnityEngine;

public class AbilityController : MonoBehaviour
{
    public Transform Agent;
    public Transform abilitySpawn;
    public GameObject selector;
    public enum SelectedButton {North, South, East, West};
    public SelectedButton selectedButton;
    
    public Skill specialAbilitySouth;
    public Skill specialAbilityNorth;
    public Skill specialAbilityEast;
    public Skill specialAbilityWest;

    private Skill abilityNorthInstance;
    private Skill abilitySouthInstance;
    private Skill abilityEastInstance;
    private Skill abilityWestInstance;

    //1 means ability is in cooldown status
    //0 means ability has cooleddown
    private int abilityA = 0;
    private int abilityB = 0;
    private int abilityC = 0;
    private int abilityD = 0;


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
    
    private bool displayAbilityWindowInput = false;
    public void OnDisplayAbilityWindow(InputAction.CallbackContext ctx) => displayAbilityWindowInput = ctx.ReadValueAsButton();

    public static event Action<GameObject, bool> onActivateAbilityWindow;

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
    }

    public void UseAbility()
    {
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
                abilityA = 1;
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
                abilityB = 1;
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
                Destroy(abilityEastInstance.gameObject, 1.5f);
                abilityC = 1;
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
                abilityD = 1;
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

    public void RefreshSkill(GameObject instance, Cooldown cooldown)
    {
        if (this.gameObject != instance)
            return;
        print("Refreshing ABility: " + cooldown.cooldownBarPosition);
        switch(cooldown.cooldownBarPosition)
        {
            case Skill.CooldownBar.A:
                print("Refreshing ABility A");
                abilityA = 0;
                break;
            case Skill.CooldownBar.B:
                abilityB = 0;
                break;
            case Skill.CooldownBar.C:
                abilityC = 0;
                break;
            case Skill.CooldownBar.D:
                abilityD = 0;
                break;
            default: 
                break;
        }
        /*if(cooldown.skillName == Skill.Name.Darknet)
        {
            print("Refreshing skill: " + cooldown.name + ": " + cooldown.activeSkill.name);
            skillUsed = false;
            fireSkill = false;
        }*/
    }
}
