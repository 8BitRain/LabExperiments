using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using DG.Tweening;
using UnityEngine.Events;
using UnityEngine;

public class PlayerCombatController : MonoBehaviour
{
    [Header("Player Combat Configuration")]
    public InputActionReference lightMeleeInput;
    public InputActionReference heavyMeleeInput;
    public InputActionReference jumpInput;
    public GameObject Player;
    public GameObject PlayerWeaponTip;

    //Attacks
    public Attack attack;
    public Attack[] attackStringOne;
    private int attackStringIndex;

    //Events
    public UnityEvent<Bladeclubber> OnTrajectory;

    [Header("Melee Attack Special VFX")]
    public ParticleSystem[] spearAttackVFX;
    private ParticleSystem meleeVFX;
    private bool canTriggerMeleeVFX;

    [Header("Light Attack Melee Combo Animation Strings")]
    public string[] lightMeleeComboStrings;

    //[Header("Light Attack Melee Combo Animation Strings")]
    //public string[] lightMeleeComboStrings;

    [Header("Force Jump Modifiers")]
    public float forceJumpHeight = 15;
    public float forceJumpForwardDistance = 15;

    [Header("Shadow of Mordor esque movement debug")]
    public float attackRange = 8.0f;
    public float enemyHeightOffset = 2;
    public bool enableEnemyHeightOffset;
    public float stoppingDistance = 5.0f;


    [Header("HitBox Controller Reference")]
    public HitBoxController hitBoxController;

    [Header("Camera Controller Reference")]
    public CameraController cameraController;
    
    [Tooltip("Variable that controls how long a cinematic shot of a martial arts move or special animation lasts. Ex. When reaching the kick, the camera zooms in on Eston's motion ")]  
    public float cinematicShotDuration;

    //Input & Timing Windows
    private bool InputWindowOpen = true;
    private bool _CombatCameraWindowOpen = true;



    [Header("Contextual Combat Options")]
    private bool _performFloatingStateAttack;
    private GameObject _floatingStateAttackInstance;

    //Player References
    private ThirdPersonMovement playerScriptReference;
    private Animator playerAnimator;

    //Combat State References
    private int combatState = 0;
    private enum CombatType{Light, Heavy};
    private CombatType combatType;

   

    //Coroutines
    private Coroutine attackCoroutine;

    // Start is called before the first frame update
    void Start()
    {
        //int count = attackStringOne.Count;
        if(Player != null)
        {
            playerScriptReference = Player.GetComponent<ThirdPersonMovement>();
            playerAnimator = Player.GetComponent<Animator>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
        if(lightMeleeInput.action.triggered && !playerScriptReference.GetSpecialAttackInputWindowActive() && InputWindowOpen && !playerAnimator.GetBool("Sprinting"))
        {
            Debug.Log("LightMeleeCombo");

            //If we are in the air, cancel the jump animation -> Go directly to the attack animation
            playerAnimator.SetBool("Jumping", false);
            //playerAnimator.SetBool("Running", false);
            playerAnimator.SetBool("Falling", false);
            playerAnimator.SetBool("Attacking", true);
            
            /*Testing using Prefabs*/
            playerAnimator.SetTrigger("lightMeleeCombo");

            string attackName = System.Enum.GetName(typeof(AttackEnums.Attacks),attackStringOne[attackStringIndex].attackName);
            
            int attackStringLength = attackStringOne.Length;
            
            //Attack(attackName,vfx,attackStringLength);
            

            //Update Combat State
            combatType = CombatType.Light;
            //UpdateCombatState();

            //Turn InputWindow Off (This is to prevent (square,square,square spam))
            InputWindowOpen = false;

            //Turn the player toward the target
            if(playerScriptReference.GetCurrentTarget() != null )
            {
                playerScriptReference.FaceTarget();

                //Move Player to enempy position Arkham Knight/Shadow of Mordor/Freeflow combat system style
                if(Vector3.Distance(playerScriptReference.GetCurrentTarget().transform.position, Player.transform.position) > attackRange)
                {
                    AttackTarget(playerScriptReference.GetCurrentTargetScriptReference());
                }
            }

            //Start timer to open input window back up
            StartCoroutine(InputWindowCoroutine(.5f));

        }

        if (jumpInput.action.triggered && !playerScriptReference.GetSpecialAttackInputWindowActive() && InputWindowOpen && !playerAnimator.GetBool("Sprinting"))
        {
            if(playerScriptReference.GetCurrentTarget() != null)
            {
                //Turn off Jumping in case we are jumping
                playerAnimator.SetBool("Jumping", false);
                playerAnimator.SetBool("Falling", false);

                //Turn jumping back on
                playerAnimator.SetBool("Jumping", true);

                playerScriptReference.FaceTarget();
                ForceJump(playerScriptReference.GetCurrentTargetScriptReference());

                InputWindowOpen = false;

                //Start timer to open input window back up
                StartCoroutine(InputWindowCoroutine(.5f));
            }
        }

        if(lightMeleeInput.action.triggered && !playerScriptReference.GetSpecialAttackInputWindowActive() && InputWindowOpen && playerAnimator.GetBool("Sprinting"))
        {
            Debug.Log("Sprinting Light Attack");
            playerAnimator.SetTrigger("sprintingLightAttack");
            //playerAnimator.SetBool("Sprinting", false);
            playerAnimator.SetBool("PerformingSprintingLightAttack", true);

            //Update Combat State
            combatType = CombatType.Light;
            //UpdateCombatState();

            //Turn InputWindow Off (This is to prevent (square,square,square spam))
            InputWindowOpen = false;


            //Start timer to open input window back up
            StartCoroutine(InputWindowCoroutine(.5f));
        }

        //Floating State Attack
        if(heavyMeleeInput.action.triggered 
            && !playerScriptReference.GetSpecialAttackInputWindowActive() 
            && InputWindowOpen && GetFloatingAttackState() 
            && !playerAnimator.GetBool("PerformingFloatingStateAttack"))
        {
            PerformFloatingStateAttack();
            combatType = CombatType.Light;
            //Start timer to open input window back up
            StartCoroutine(InputWindowCoroutine(.5f));
        }

        UpdateCombatState();
    }

    

    IEnumerator InputWindowCoroutine (float time) 
    {
        float elapsedTime = 0;

        
        while (elapsedTime < time)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        InputWindowOpen = true;
    }

    IEnumerator CombatCameraCoroutine (float time) 
    {
        float elapsedTime = 0;
        _CombatCameraWindowOpen = false;

        
        while (elapsedTime < time)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        cameraController.DisableCinematicKickCam();
        _CombatCameraWindowOpen = true;
    }

    //Updates Automatically. For now this should update based on events from the animation
    public void UpdateCombatState()
    {
        if(combatType == CombatType.Light)
        {
            if(playerAnimator.GetCurrentAnimatorStateInfo(0).IsTag("combo1"))
            {
                GetCurrentAnimationStateName();
                //VFX vfx = attackStringOne[0].VFX;
                //Instantiate(vfx, p)
                //TriggerVFX();
            }
            else if(playerAnimator.GetCurrentAnimatorStateInfo(0).IsTag("combo2"))
            {
                GetCurrentAnimationStateName();
            }
            else if(playerAnimator.GetCurrentAnimatorStateInfo(0).IsTag("combo3"))
            {
                GetCurrentAnimationStateName();
                playerScriptReference.CombatJump();
                if(_CombatCameraWindowOpen)
                {
                    cameraController.EnableCinematicKickCam();
                    StartCoroutine(CombatCameraCoroutine(cinematicShotDuration));
                }
            }
            else
            {
                //This script will be handled by an animation event
                //cameraController.ResetCinematicCombatCams();
            }
        }



        UpdatePlayerPosition();
    }

    //Initiates combat
    public void InitiateCombat(int comboNum)
    {
        if(comboNum != 0)
            //hitBoxController.ActivateHitBox(comboNum);
        
        if(comboNum == 0)
            StartCoroutine(ActivateDelayedHitbox(.5f));
        
        IEnumerator ActivateDelayedHitbox(float duration)
        {
            yield return new WaitForSeconds(duration);
            //hitBoxController.ActivateHitBox(comboNum);
        }

        print("PlayerCombatController: Initiate Combat");

        if(playerScriptReference.GetCurrentTarget() == null )
        {
            print("No current target, move player");
            Player.transform.DOMove(Player.transform.position + Player.transform.forward*5, .5f);
        }

        if(playerScriptReference.GetCurrentTarget() != null )
        {
            print("PlayerCombatController: Player has a target: " + playerScriptReference.GetCurrentTarget());
        }
    }

    //Deactivates combat
    public void DisengageCombat(int comboNum)
    {
        //hitBoxController.DeactivateHitBox(comboNum);
    }

    public void UpdatePlayerPosition()
    {
        if(playerAnimator.GetBool("PerformingFloatingStateAttack"))
        {
            //Move the player up and towards the enemy to deliver an attack
            playerScriptReference.JumpToTarget(GetFloatingAttackInstance().GetComponent<FloatingAttack>().GetFloatingEntity());
        }
    }

    public void PerformSprintingAttack()
    {
        playerAnimator.SetBool("PerformingSprintingLightAttack", true);
    }

    public void PerformFloatingStateAttack()
    {
        //We performed the floating attack, no need to perform again
        Debug.Log("PlayerCombatController: Performing floating state attack");
        playerScriptReference.SetPlayerGravity(false);
        playerAnimator.SetTrigger("floatingStateAttack");
        playerAnimator.SetBool("PerformingFloatingStateAttack", true);
        GetFloatingAttackInstance().GetComponent<FloatingAttack>().SetAttackTriggered();
        SetFloatingAttackState(false);
    }

    public void EndAnimation(string animName)
    {
        if(animName == "floatingStateAttack")
        {
            playerAnimator.SetBool("PerformingFloatingStateAttack", false);
            playerScriptReference.SetPlayerGravity(true);
            
            //TODO add condition that checks if we are performing a combo action
            //Destroy the FloatingAttackZoneInstance
            if(GetFloatingAttackInstance() != null)
            {
                Destroy(GetFloatingAttackInstance());
            }
        }
    }

    public void SetFloatingAttackState(bool value)
    {
        this._performFloatingStateAttack = value;
    }

    public bool GetFloatingAttackState()
    {
        return _performFloatingStateAttack;
    }

    public void SetFloatingAttackInstance(GameObject floatingAttackInstance)
    {
        this._floatingStateAttackInstance = floatingAttackInstance;
    }

    public GameObject GetFloatingAttackInstance()
    {
        return _floatingStateAttackInstance;
    }

    //This animation event is triggered by eston_rig|spearLunge2
    public void AnimationEventEndSprintingAttack()
    {
        playerAnimator.SetBool("PerformingSprintingLightAttack", false);
    }

    public string[] GetLightMeleeComboStrings()
    {
        return this.lightMeleeComboStrings;
    }
    //Get's the current animation name from the animator
    public string GetCurrentAnimationStateName()
    {
        Debug.Log("Current Animation State Name: " + this.playerAnimator.GetCurrentAnimatorClipInfo(0)[0].clip.name);
        return this.playerAnimator.GetCurrentAnimatorClipInfo(0)[0].clip.name;
    }

    void MoveTorwardsTarget(Bladeclubber target, float duration)
    {
        //OnTrajectory is a script that stops target movement
        //OnTrajectory.Invoke(target);
        playerScriptReference.SetCharacterControllerActive(false);
    
        /*
        *   Let's explain new Vector3(0,enemyHeightOffset,0)) + transform.forward*8
        *   We apply new Vector3(0,enemyHeightOffset,0) because it allows us to adjust how high the player moves toward the target
        *   Or, we can decide to lock the player character to the Y axis
        *   Either way, we are going to apply the stopping distance so the player doesn't collide with enemy hitbox
        */


        
        transform.DOMove(TargetOffset(target.transform, new Vector3(0,enemyHeightOffset,0), stoppingDistance), duration);
        
        //Lock Player Y when moving towards 
        //Vector3.MoveTowards(new Vector3(Player.transform.position.y, position.z), transform.position, .95f);
        //transform.DOMove(new Vector3(target.transform.position.x, Player.transform.position.y, target.transform.position.z - attackRange), duration);
    }

    /*
    * TargetOffset is a function that moves the player closer to a target defined by MoveTowardsTarget
    * params
    * target: the target
    * offset: The offset of the target. This calculation is useful for setting a height offset if the enemy is too tall or too short
    *         this values allows for adjusting how much (if allowed) the player can rise off the ground to strike a target
    * stoppingDistance: How close can we get to the target before stopping? This is useful for stopping the player from getting too close to the enemy, and colliding with it's rigidbody
    *                   
    */
    public Vector3 TargetOffset(Transform target, Vector3 offset, float stoppingDistance)
    {
        Vector3 position;
        position = target.position;

        //Lock player Y position to its current position
        if(!enableEnemyHeightOffset)
            return Vector3.MoveTowards(new Vector3(position.x, Player.transform.position.y, position.z) + -transform.forward*stoppingDistance, transform.position, .95f);
        
        return Vector3.MoveTowards(position + offset + -transform.forward*stoppingDistance, transform.position, .95f);
    }

    void JumpOverTarget(Bladeclubber target, float duration)
    {
        playerScriptReference.SetCharacterControllerActive(false);
        transform.DOLookAt(target.transform.position, .2f);

        float forwardDistance = Vector3.Distance(target.transform.position, Player.transform.position) + 2;
        transform.DOJump(target.transform.position + transform.up*2 + transform.forward*forwardDistance, forceJumpHeight, 1, 1.5f, false);
    }
    
    void Attack(string attackName, VFX VFX, int attackStringLength)
    {
        playerAnimator.SetTrigger(attackName);
        
        print("PlayerCombatController: attackName: " + attackName);

        if(VFX != null)
            Instantiate(VFX, Player.transform.position + Vector3.up * 5, Player.transform.rotation);

        //Ensure attack string index wraps around once all attacks have been completed.
        if(attackStringIndex < attackStringLength - 1)
        {
            attackStringIndex++;
            //playerAnimator.ResetTrigger(attackName);
        }    
        else
        {
            attackStringIndex = 0;
            playerAnimator.ResetTrigger(attackName);
        }
            
    }

    void AttackTarget(Bladeclubber target)
    {
        print("PlayerCombatController Attack: " + target);
        MoveTorwardsTarget(target, .5f);
        attackCoroutine = StartCoroutine(AttackCoroutine(1.5f));
        IEnumerator AttackCoroutine(float duration)
        {
            playerScriptReference.acceleration = 0;
            //Consider moving combat "state" to this file. IsAttacking is a combat state boolean
            //isAttackingEnemy = true;
            playerScriptReference.enabled = false;
            //Time.timeScale = .5f;
            yield return new WaitForSeconds(duration);
            //isAttackingEnemy = false;
            yield return new WaitForSeconds(.2f);
            //Time.timeScale = 1;
            playerScriptReference.enabled = true;
            LerpCharacterAcceleration();
            playerScriptReference.SetCharacterControllerActive(true);
        }
    }

    void ForceJump(Bladeclubber target)
    {
        print("PlayerCombatController ForceJump: " + target);
        JumpOverTarget(target, 1.5f);
        attackCoroutine = StartCoroutine(AttackCoroutine(1.5f));
        IEnumerator AttackCoroutine(float duration)
        {
            playerScriptReference.acceleration = 0;
            //Consider moving combat "state" to this file. IsAttacking is a combat state boolean
            //isAttackingEnemy = true;
            playerScriptReference.enabled = false;
            yield return new WaitForSeconds(duration);
            //isAttackingEnemy = false;
            yield return new WaitForSeconds(.2f);
            playerScriptReference.enabled = true;
            LerpCharacterAcceleration();
            Vector3 targetLookAtVector = new Vector3(target.transform.position.x, Player.transform.position.y, target.transform.position.z);
            transform.DOLookAt(targetLookAtVector, .2f);
            playerScriptReference.SetCharacterControllerActive(true);
        }
    }

    //Helpers


    void LerpCharacterAcceleration()
    {
        playerScriptReference.acceleration = 0;
        DOVirtual.Float(0, 1, .6f, ((acceleration)=> playerScriptReference.acceleration = acceleration));
    }

    void TriggerVFX()
    {
        canTriggerMeleeVFX = false;
        if(meleeVFX == null)
        {
            //Instantiate VFX
        }

        if(meleeVFX != null)
        {
            return;
        }
    }

    void ResetMeleeVFX()
    {
        meleeVFX = null;
        canTriggerMeleeVFX = true;
    }
    
}
