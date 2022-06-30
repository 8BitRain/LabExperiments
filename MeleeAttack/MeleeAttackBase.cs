using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using DG.Tweening;
using UnityEngine;

public class MeleeAttackBase : MonoBehaviour
{
    [Header("References Components")]
    private GameObject playerReference;
    private AnimationController animationController;
    private Body bodyReference;
    private GameObject meleeAttackInstance;
    private Transform targetInstance;
    private Transform meleeSpawnPosition;
    private CameraSettings cameraSettingsInstance;
    private PlayerMovementController playerMovementController;
    private bool isAIAgent = false;
    
    [Header("Attack Components")]
    public GameObject attackComponent;
    

    [Header("Melee Settings")]
    public bool isMobileMelee = false;
    public bool isHeldMelee = false;
    public bool isRanged = false;

    [Header("Player Settings")]
    [Range(0f, 3.0f)]
    public float animationCompleteWaitTime;

    [Header("Animation Settings")]
    public float movementLockDuration;

    [Header("Lifecycle")]
    public bool fired = false;
    private bool onAnimation = false;
    private bool abilityConnected = false;
    private bool cooldownTriggered = false;

    [Header("Coroutines")]
    private Coroutine animationCoroutine;
    private Coroutine meleeCoroutine;

    [Header("Tweens")]
    private Tween hitStop;
    private Coroutine hitStopCouroutine;

    private void OnEnable()
    {
        HitBox.collision += CollisionLogic;
    }

    private void OnDisable()
    {
        HitBox.collision -= CollisionLogic;
    }

    void OnDestroy()
    {
        if(!cooldownTriggered)
        {
            EngageCooldown();
        }
        Destroy(meleeAttackInstance.gameObject);
    }

    public virtual void Melee()
    {
        //Cache Player Movement Controller if it exists
        if(GetPlayerReference().TryGetComponent<PlayerMovementController>(out PlayerMovementController movementController))
        {
            this.playerMovementController = movementController;
        }

        //If the skill is current in use, return, we don't need to activate teh skill again
        if(fired)
            return;

        if(isMobileMelee)
        {
            //Lock Player RightSideFaceButtonActions
            try
            {
                GetPlayerReference().GetComponent<PlayerMovementController>().LockInputRightSideControllerFaceButtons(this.gameObject);
            }
            catch (System.Exception)
            {
                //throw;
            }
        }
        else
        {
            //Lock Player Movement
            try
            {
                GetPlayerReference().GetComponent<PlayerMovementController>().DisableMovement();
                //GetPlayerReference().GetComponent<PlayerMovementController>().EnableSteering();
            }
            catch (System.Exception)
            {
                //throw;
            }
        }

        //Instantiate ability
        meleeAttackInstance = Instantiate(attackComponent, GetMeleeSpawnPosition().position, GetMeleeSpawnPosition().rotation);

        //For purely physical melee attacks we want to set parent relationship in order for weapon projectile slashes to appear.
        if(!isRanged)
        {
            meleeAttackInstance.transform.SetParent(GetPlayerReference().transform);
        }
        meleeAttackInstance.transform.LookAt(Camera.main.transform.forward);
        
        //Iterate through ability instances modular components
        this.meleeCoroutine = StartCoroutine(PlayModularComponents());

        fired = true;
    }

    public virtual IEnumerator PlayModularComponents()
    {
        foreach (Transform modularComponent in meleeAttackInstance.GetComponentsInChildren<Transform>())
        {
            if(modularComponent.TryGetComponent<IMeleeAttackComponent>(out IMeleeAttackComponent modularMeleeAttackComponent))
            {
                MeleeAttackComponent meleeAttackComponent = modularMeleeAttackComponent.GetMeleeAttackComponent();
                PlayModularComponent(modularComponent.gameObject, modularMeleeAttackComponent.GetMeleeAttackComponent());

                //Animation Delay Logic
                if(meleeAttackComponent.animationComponent != null)
                {
                    if(meleeAttackComponent.animationComponent.animationEndDelay != 0)
                    {
                        Debug.Log("Animation Delay: " + meleeAttackComponent.animationComponent.animation + " for: " + meleeAttackComponent.animationComponent.animationEndDelay);
                        animationCoroutine = StartCoroutine(AnimationDelay(meleeAttackComponent.animationComponent));
                        FireModularProjectile(modularComponent.gameObject);
                        yield return new WaitForSeconds(meleeAttackComponent.animationComponent.animationEndDelay);
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
        StopCoroutine(meleeCoroutine);
        Destroy(this.gameObject);
    }

    public void PlayModularComponent(GameObject modularAbilityInstance, MeleeAttackComponent meleeAttackComponent)
    {

        TriggerHitBox(modularAbilityInstance, true);

        //Does the modular component have ambient vfx to play?
        SummonVFX(modularAbilityInstance);

        //Use multi viewpoint camera to render action scene
        /*if(meleeAttackComponent.cameraSettings != null && this.targetInstance != null)
        {
            //I would prefer this to route to a more generic method, that then branches out to Engaging Dynamic TargetLock or Engages a special third person camera
            //GetPlayerReference().GetComponent<CameraController>().EngageDynamicTargetLock(GetPlayerReference(), this.targetInstance, meleeAttackComponent.cameraSettings);
            GetPlayerReference().GetComponent<CameraController>().RouteToCameraEngage(GetPlayerReference(), this.targetInstance, meleeAttackComponent.cameraSettings);
            cameraSettingsInstance = meleeAttackComponent.cameraSettings;
        }*/

        //Update ThirdPerson Camera for melee attacks
        //We check to make sure this is a player before manipulating camera
        if(this.GetPlayerReference().GetComponent<PlayerMovementController>() != null)
        {
            if(meleeAttackComponent.cameraSettings != null)
            {
                Debug.Log("Trigger Camera Controller");
                cameraSettingsInstance = meleeAttackComponent.cameraSettings;
                this.GetPlayerReference().GetComponent<CameraController>().UpdateThirdPersonCameraOffset(GetPlayerReference(), meleeAttackComponent.cameraSettings);
            }
        }

        //Controls how the component travels
        switch (meleeAttackComponent.travelDirection)
        {
            case MeleeAttackComponent.MovementDirection.Forward:
                modularAbilityInstance.transform.DOMove(GetPlayerReference().transform.position + GetPlayerReference().transform.forward*meleeAttackComponent.travelAmount, meleeAttackComponent.timeToTravel);
                break;
            case MeleeAttackComponent.MovementDirection.Backward:
                modularAbilityInstance.transform.DOMove(GetPlayerReference().transform.position - GetPlayerReference().transform.forward*meleeAttackComponent.travelAmount, meleeAttackComponent.timeToTravel);
                break;
            default:
                break;
        }
        
        Vector3 diagonalVector = new Vector3(0,0,0);
        //Controls how the player travels
        switch (meleeAttackComponent.playerMovementDirection)
        {
            case MeleeAttackComponent.MovementDirection.Forward:
                if(GetPlayerMovementController() != null)
                {
                    //If we are moving, allow the player to still control rotation and direction while performing melee attack
                    //If we aren't moving, we can allow the melee attack to move according to the direction the player is facing.
                    if(GetPlayerMovementController().movementInput.magnitude != 0)
                        GetPlayerMovementController().EnableSteeringAndMovement();
                    else
                        GetPlayerReference().transform.DOMove(GetPlayerReference().transform.position + GetPlayerReference().transform.forward*meleeAttackComponent.playerMovementAmount, meleeAttackComponent.playerMovementTime);
                }
                else
                {
                    GetPlayerReference().transform.DOMove(GetPlayerReference().transform.position + GetPlayerReference().transform.forward*meleeAttackComponent.playerMovementAmount, meleeAttackComponent.playerMovementTime);
                }
                break;
            case MeleeAttackComponent.MovementDirection.Backward:
                GetPlayerReference().transform.DOMove(GetPlayerReference().transform.position - GetPlayerReference().transform.forward*meleeAttackComponent.playerMovementAmount, meleeAttackComponent.playerMovementTime);
                break;
            case MeleeAttackComponent.MovementDirection.BackwardDiagonalLeft:
                diagonalVector = (GetPlayerReference().transform.forward * meleeAttackComponent.playerMovementAmount) + (GetPlayerReference().transform.right * meleeAttackComponent.playerMovementAmount);
                GetPlayerReference().transform.DOMove(GetPlayerReference().transform.position - diagonalVector, meleeAttackComponent.playerMovementTime);
                break;
            case MeleeAttackComponent.MovementDirection.ForwardDiagonalRight:
                diagonalVector = (GetPlayerReference().transform.forward * meleeAttackComponent.playerMovementAmount) + (GetPlayerReference().transform.right * meleeAttackComponent.playerMovementAmount);
                GetPlayerReference().transform.DOMove(GetPlayerReference().transform.position + diagonalVector, meleeAttackComponent.playerMovementTime);
                break;
            case MeleeAttackComponent.MovementDirection.Up:
                float distanceToTarget = 0;
                /*if(targetInstance != null)
                {
                    distanceToTarget = (GetPlayerReference().transform.position - targetInstance.transform.position).magnitude; 
                    GetPlayerReference().transform.DOMove(GetPlayerReference().transform.position + GetPlayerReference().transform.up*meleeAttackComponent.playerMovementAmount, meleeAttackComponent.playerMovementTime);
                } 
                else
                {
                    GetPlayerReference().transform.DOMove(GetPlayerReference().transform.position + GetPlayerReference().transform.up*meleeAttackComponent.playerMovementAmount, meleeAttackComponent.playerMovementTime);
                }*/
                GetPlayerReference().transform.DOMove(GetPlayerReference().transform.position + GetPlayerReference().transform.up*meleeAttackComponent.playerMovementAmount, meleeAttackComponent.playerMovementTime);
                break;
            case MeleeAttackComponent.MovementDirection.ForwardUp:
                GetPlayerReference().transform.DOMove(GetPlayerReference().transform.position + GetPlayerReference().transform.up*meleeAttackComponent.playerMovementAmount + GetPlayerReference().transform.forward*meleeAttackComponent.playerMovementAmount, meleeAttackComponent.playerMovementTime);
                break;
            default:
                break;
        }

        if(meleeAttackComponent.stickToPlayer)
            modularAbilityInstance.transform.SetParent(GetPlayerReference().transform);
        
        if(meleeAttackComponent.stickToPlayerTime != 0)
            StartCoroutine(MeleeAttackComponentStickToPlayerCoroutine(meleeAttackComponent.stickToPlayerTime,modularAbilityInstance.transform));
        
        if(meleeAttackComponent.isMobile)
            modularAbilityInstance.transform.DOMove(modularAbilityInstance.transform.position + Camera.main.transform.forward * meleeAttackComponent.travelAmount, meleeAttackComponent.timeToTravel);

        
        if(meleeAttackComponent.canScale)
        {
            modularAbilityInstance.transform.localScale = meleeAttackComponent.minScaleVector;
            modularAbilityInstance.transform.DOScale(meleeAttackComponent.maxScaleVector * meleeAttackComponent.scaleStrength, meleeAttackComponent.timeToScale);
        }

        TriggerHitBox(modularAbilityInstance, false);
        
        //Does player have an animation component?
        if(meleeAttackComponent.animationComponent != null)
        {
            string animation = meleeAttackComponent.animationComponent.animation.ToString();
            Debug.Log("Play Animation: " + animation);
            //Reset animation state first
            GetAnimationController().ResetAnimationState();

            //Play Animation
            GetAnimationController().ChangeAnimationState(GetPlayerReference().GetComponent<Animator>(),animation);
        }

        //TODO check if player can turn while using this skill
        if(targetInstance != null && meleeAttackComponent.lookAtTarget)
        {
            if(meleeAttackComponent.lookAtTargetLockY)
            {
                GetPlayerReference().transform
                .DOLookAt(new Vector3(targetInstance.transform.position.x, GetPlayerReference().transform.position.y, targetInstance.transform.position.z), .75f);
            }
            else
            {
                GetPlayerReference().transform.DOLookAt(targetInstance.transform.position, .75f);
            }
            
           //GetPlayerReference().GetComponent<CameraController>().RecenterThirdPersonCam(meleeAttackComponent.reTargetTime);
        }

        //Does the modular component play an audio effect?
        if(meleeAttackComponent.audioComponent != null)
        {
            if(modularAbilityInstance.TryGetComponent( out AudioSource audioSource))
            {
                audioSource.clip = meleeAttackComponent.audioComponent.ambient;
                audioSource.Play();
            }
        }

        //Does the modular component spawn a clone?
        if(modularAbilityInstance.TryGetComponent(out SummonClone summonClone))
        {
            GameObject cloneInstance = Instantiate(summonClone.clone, GetPlayerReference().transform.position, GetPlayerReference().transform.rotation);
            Debug.Log("Summoning Clone: " + GetAnimationController());
            summonClone.UpdateSummonAnimation(cloneInstance, GetPlayerReference().GetComponent<Animator>(), GetAnimationController());
        }
    
    }

    public void SummonVFX(GameObject modularAbilityInstance)
    {
        if(modularAbilityInstance.TryGetComponent<ModularAttackElement>(out ModularAttackElement modularMeleeAttackComponent))
        {
            if(modularMeleeAttackComponent.GetVFX() != null)
            {
                Body summonerBody = GetPlayerReference().GetComponent<Body>();
                switch (modularMeleeAttackComponent.meleeAttackComponent.vfxSpawnLocation)
                {
                    case MeleeAttackComponent.VFXSpawnLocation.DEFAULT:
                        Instantiate(modularMeleeAttackComponent.GetVFX(), GetPlayerReference().transform.position, GetPlayerReference().transform.rotation);
                        break;
                    case MeleeAttackComponent.VFXSpawnLocation.BACK:
                        Instantiate(modularMeleeAttackComponent.GetVFX(), summonerBody.Back.position, summonerBody.Back.rotation);
                        break;
                    default:
                        break;
                }
            }
        }
    }

    public void FireModularProjectile(GameObject modularAbilityInstance)
    {
        Debug.Log("Fire Modular Projectile");
        if(modularAbilityInstance.TryGetComponent<ModularAttackElement>(out ModularAttackElement modularMeleeAttackComponent))
        {
            if(modularMeleeAttackComponent.GetProjectile() != null)
            {
                Projectile modularProjectile = Instantiate(modularMeleeAttackComponent.GetProjectile(), GetMeleeSpawnPosition().position, GetMeleeSpawnPosition().rotation);
                
                if(!isRanged)
                {
                    modularProjectile.transform.SetParent(GetPlayerReference().transform);
                }
                modularProjectile.SetProjectileSummonerReference(GetPlayerReference());

                //This step is crucial. The projectile Summoner Reference is needed to accurately tell the hitbox not to hit its own hurtbox. 
                //TODO think of a better way to prevent hitting oneself. 
                modularProjectile.gameObject.SetActive(false);
                DOVirtual.DelayedCall(modularProjectile.projectileComponent.startDelay, () =>{
                    modularProjectile.gameObject.SetActive(true);
                    //Fire the projectile parent. This is what actually controls travelling.
                    modularProjectile.Fire();

                    //Slightly dangerous. Assumes projectile is a direct child of its container, and the first child.
                    /*
                    * What's really happening here is the direct child holds the reference to the hitbox. 
                    * Q: Should the child hold the hitbox reference? Why can't the parent hold the hitbox references?
                    * A: Well, what if we want the child component to move? (Could be wrong revisit)
                    * 
                    * Q: 
                    * A: 
                    */
                    if(GetAIAgentStatus())
                    {
                        modularProjectile.transform.GetChild(0).GetComponent<Projectile>().hitBox.layers[0] = LayerMask.GetMask("PlayerHurtBox");
                    }
                    modularProjectile.transform.GetChild(0).GetComponent<Projectile>().SetProjectileSummonerReference(GetPlayerReference());
                    modularProjectile.transform.GetChild(0).GetComponent<Projectile>().Fire();
                });

                if(modularMeleeAttackComponent.GetProjectile().GetVFX() != null)
                {
                   Instantiate(modularMeleeAttackComponent.GetProjectile().GetVFX(), GetMeleeSpawnPosition().position, GetMeleeSpawnPosition().rotation);
                }
            }
        }
    }

    public void TriggerHitBox(GameObject modularAbilityInstance, bool isActive)
    {
        if(modularAbilityInstance.TryGetComponent<ModularAttackElement>(out ModularAttackElement modularMeleeAttackComponent))
        {
            HitBox hitBox = modularMeleeAttackComponent.hitBox;
            if(hitBox != null)
            {
                print("Hitbox Triggered");
                
                if(GetAIAgentStatus())
                {
                    Debug.Log("AI Status: " + GetAIAgentStatus());
                    //If an AI Agent spawned this meleeAttack, set the layer it can collide with to the PlayerHurtBox
                    int playerHurtBoxLayer = LayerMask.NameToLayer("PlayerHurtBox");
                    hitBox.layers[0] = playerHurtBoxLayer;
                }

                if(!isActive)
                {
                    Debug.Log("Spell Instance Name: " + modularAbilityInstance.name);
                    Debug.Log("Hitbox instance name: " + hitBox.gameObject.name);
                    EventsManager.instance.OnTriggerHitBox(hitBox.gameObject, GetPlayerReference(), false, 
                    modularMeleeAttackComponent.GetMeleeAttackComponent().hitBoxStartDelay, modularMeleeAttackComponent.GetMeleeAttackComponent().hitBoxDuration);
                }
                else
                {
                    EventsManager.instance.OnTriggerHitBox(hitBox.gameObject, GetPlayerReference(), true, 0, modularMeleeAttackComponent.GetMeleeAttackComponent().hitBoxDuration);
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
        /* Debug logic for pausing the game after a certain melee attack is used
        if(this.gameObject.name.Contains("ReverseSlash"))
        {
            Debug.Break();
        }*/

        //Reset Camera
        /*GetPlayerReference().GetComponent<CameraController>().RouteToCameraDisengage(GetPlayerReference(), cameraSettingsInstance);*/

        //Reset Player Movement
        if(GetPlayerMovementController() != null)
        {
            GetPlayerMovementController().DisableSteeringAndMovement();
        }

        //Unlock player movement
        //Animation Controller Coroutine
        try
        {
            GetAnimationController().PlayerAnimationLock(movementLockDuration, GetPlayerReference().GetComponent<PlayerMovementController>(), isMobileMelee);
        }
        catch (System.Exception)
        {
            //throw;
        }

        //Update ThirdPerson Camera for melee attacks
        //We check to make sure this is a player before manipulating camera
        if(this.GetPlayerReference().GetComponent<PlayerMovementController>() != null)
        {
            if(cameraSettingsInstance != null)
            {
                this.GetPlayerReference().GetComponent<CameraController>().ResetThirdPersonCameraOffset(GetPlayerReference());   
            }
        }
        
        cooldownTriggered = true;
        Destroy(this.gameObject);
    }

    //This function unparents a spell component
    public IEnumerator MeleeAttackComponentStickToPlayerCoroutine(float duration, Transform meleeAttackComponent)
    {
        yield return new WaitForSeconds(duration);
        meleeAttackComponent.SetParent(null);
    }

    //Getters & Setters Code
    public void SetPlayerReference(GameObject playerReference)
    {
        this.playerReference = playerReference;
    }

    public void SetPlayerAnimationController(AnimationController animationController)
    {
        this.animationController = animationController;
    }

    public void SetBodyReference(Body body)
    {
        this.bodyReference = body;
    }

    public void SetMeleeSpawnPoint(Transform meleeSpawnPosition)
    {
        this.meleeSpawnPosition = meleeSpawnPosition;
    }

    public void SetAIAgentStatus(bool status)
    {
        Debug.Log("Setting AI Status to " + status);
        isAIAgent = status;
    }

    public bool GetAIAgentStatus()
    {
        Debug.Log("Returning AI Status: " + isAIAgent);
        return isAIAgent;
    }

    public GameObject GetPlayerReference()
    {
        return this.playerReference;
    }

    public PlayerMovementController GetPlayerMovementController()
    {
        return this.playerMovementController;
    }

    public AnimationController GetAnimationController()
    {
        return this.animationController;
    }

    public Transform GetMeleeSpawnPosition()
    {
        return this.meleeSpawnPosition;
    }

    //Helpers
    public void SetAbilityConnected(bool hasConnected)
    {
        this.abilityConnected = hasConnected;
    }

    public bool GetAbilityConnected()
    {
        return this.abilityConnected;
    }

    public void CollisionLogic(GameObject targetInstance, GameObject hurtBoxInstance, GameObject summonerInstance, AbilityComponent meleeAttackComponent)
    {
        if(GetPlayerReference() != summonerInstance)
        {
            Debug.Log(this.meleeAttackInstance.name + " is not the same game object as " + summonerInstance.gameObject);
            return;
        }

        //MeleeAttack connected, let's allow the player to press the melee button again to trigger another melee attack
        GetPlayerReference().GetComponent<MeleeAttackController>().SetPlayerInputCanInterruptCombo(true);

        //Hitstop
        float hitStopDuration = 0;
        if(meleeAttackInstance.TryGetComponent<ModularAttackElement>(out ModularAttackElement modularAttackElement))
        {
            if(modularAttackElement.GetMeleeAttackComponent().animationComponent.applyHitStop)
            {
                hitStopDuration = modularAttackElement.GetMeleeAttackComponent().animationComponent.hitStopDuration;
                StartHitStop();
            }
        }

        this.targetInstance = targetInstance.transform;
        SetAbilityConnected(true);

        Debug.Log(this.gameObject.name +  "Processing Collision Logic");

        //Push the player back according to attackRecoil
        if(meleeAttackComponent.collisionComponent.recoilDirection != CollisionComponent.RecoilDirection.None)
        {
            float recoilAmount = meleeAttackComponent.collisionComponent.recoilAmount;
            float recoilDuration = meleeAttackComponent.collisionComponent.recoilDuration;

            //Kill all tweens currently playing
            GetPlayerReference().transform.DOKill();
            switch (meleeAttackComponent.collisionComponent.recoilDirection)
            {
                case CollisionComponent.RecoilDirection.Forward:
                    GetPlayerReference().transform.DOMove(GetPlayerReference().transform.position + (GetPlayerReference().transform.forward * recoilAmount), recoilDuration);
                    break;
                case CollisionComponent.RecoilDirection.Backward:
                    DOVirtual.DelayedCall(hitStopDuration, () => {
                        GetPlayerReference().transform.DOMove(GetPlayerReference().transform.position + (-GetPlayerReference().transform.forward * recoilAmount), recoilDuration);
                    });
                    break;
                default:
                    break;
            }
        }
    }

    public void StartHitStop()
    {
        //Set Animation speed to 0 to sell freeze
        GetPlayerReference().GetComponent<Animator>().speed = 0;
        Debug.Log(GetPlayerReference().name + " Start Hitstop: " + Time.time);
        //Get the current playback time of the sprite
        float stoppedAnimationTime = GetPlayerReference().GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime;
        //Store animation animation that is playing. Return to this animation after hit stop.
        string animationStateBeforeReset = GetAnimationController().GetCurrentState();

        //Couroutine used for hitstop. Can be interrupted by user input
        GetAnimationController().HitStop(meleeAttackInstance.GetComponent<ModularAttackElement>().GetMeleeAttackComponent().animationComponent.hitStopDuration, stoppedAnimationTime, animationStateBeforeReset);
    }

    public IEnumerator hitStopDelay(float duration, float stoppedAnimationTime, string animationStateBeforeReset)
    {
        Debug.Log("HitStopDelay: Running HitStop Delay Coroutine " + Time.time);
        yield return new WaitForSeconds(duration);
        Debug.Log(GetPlayerReference().name + " Stop Hitstop: " + Time.time);
        GetAnimationController().ResetAnimationState();
        GetAnimationController().ChangeAnimationState(GetPlayerReference().GetComponent<Animator>(), animationStateBeforeReset, stoppedAnimationTime - .05f);
        GetPlayerReference().GetComponent<Animator>().speed = 1;
    }

    public void CancelHitStop()
    {
        GetAnimationController().CancelHitStop();
        Destroy(this.gameObject);
    }
}