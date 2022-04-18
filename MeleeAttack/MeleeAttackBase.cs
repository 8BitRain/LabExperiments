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
    
    [Header("Attack Components")]
    public GameObject attackComponent;
    

    [Header("Melee Settings")]
    public bool isMobileMelee = false;
    public bool isHeldMelee = false;

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

    private void OnEnable()
    {
        HitBox.collision += CollisionLogic;
    }

    private void OnDisable()
    {
        HitBox.collision -= CollisionLogic;
    }

    public virtual void Melee()
    {
        //If the skill is current in use, return, we don't need to activate teh skill again
        if(fired)
            return;

        if(isMobileMelee)
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
        meleeAttackInstance = Instantiate(attackComponent, GetMeleeSpawnPosition().position, GetMeleeSpawnPosition().rotation);
        meleeAttackInstance.transform.LookAt(Camera.main.transform.forward);
        
        //Iterate through ability instances modular components
        this.meleeCoroutine = StartCoroutine(PlayModularComponents());

        fired = true;
    }

    public virtual IEnumerator PlayModularComponents()
    {
        foreach (Transform modularComponent in meleeAttackInstance.GetComponentsInChildren<Transform>())
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
                            FireModularProjectile(modularComponent.gameObject);
                            yield return new WaitUntil(() => GetAbilityConnected());
                        }
                        else
                        {
                            yield return new WaitForSeconds(abilityComponent.animationComponent.animationEndDelay);
                            FireModularProjectile(modularComponent.gameObject);
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
        StopCoroutine(meleeCoroutine);
        Destroy(this.gameObject);
    }

    public void PlayModularComponent(GameObject modularAbilityInstance, AbilityComponent abilityComponent)
    {

        TriggerHitBox(modularAbilityInstance, true);

        //Does the modular component have ambient vfx to play?
        SummonVFX(modularAbilityInstance);

        //Use multi viewpoint camera to render action scene
        /*if(abilityComponent.cameraSettings != null && this.targetInstance != null)
        {
            //I would prefer this to route to a more generic method, that then branches out to Engaging Dynamic TargetLock or Engages a special third person camera
            //GetPlayerReference().GetComponent<CameraController>().EngageDynamicTargetLock(GetPlayerReference(), this.targetInstance, abilityComponent.cameraSettings);
            GetPlayerReference().GetComponent<CameraController>().RouteToCameraEngage(GetPlayerReference(), this.targetInstance, abilityComponent.cameraSettings);
            cameraSettingsInstance = abilityComponent.cameraSettings;
        }*/

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
        
        Vector3 diagonalVector = new Vector3(0,0,0);
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
                diagonalVector = (GetPlayerReference().transform.forward * abilityComponent.playerMovementAmount) + (GetPlayerReference().transform.right * abilityComponent.playerMovementAmount);
                GetPlayerReference().transform.DOMove(GetPlayerReference().transform.position - diagonalVector, abilityComponent.playerMovementTime);
                break;
            case AbilityComponent.MovementDirection.ForwardDiagonalRight:
                diagonalVector = (GetPlayerReference().transform.forward * abilityComponent.playerMovementAmount) + (GetPlayerReference().transform.right * abilityComponent.playerMovementAmount);
                GetPlayerReference().transform.DOMove(GetPlayerReference().transform.position + diagonalVector, abilityComponent.playerMovementTime);
                break;
            case AbilityComponent.MovementDirection.Up:
                float distanceToTarget = 0;
                /*if(targetInstance != null)
                {
                    distanceToTarget = (GetPlayerReference().transform.position - targetInstance.transform.position).magnitude; 
                    GetPlayerReference().transform.DOMove(GetPlayerReference().transform.position + GetPlayerReference().transform.up*abilityComponent.playerMovementAmount, abilityComponent.playerMovementTime);
                } 
                else
                {
                    GetPlayerReference().transform.DOMove(GetPlayerReference().transform.position + GetPlayerReference().transform.up*abilityComponent.playerMovementAmount, abilityComponent.playerMovementTime);
                }*/
                GetPlayerReference().transform.DOMove(GetPlayerReference().transform.position + GetPlayerReference().transform.up*abilityComponent.playerMovementAmount, abilityComponent.playerMovementTime);
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

        //TODO check if player can turn while using this skill
        if(targetInstance != null && abilityComponent.lookAtTarget)
        {
            if(abilityComponent.lookAtTargetLockY)
            {
                GetPlayerReference().transform
                .DOLookAt(new Vector3(targetInstance.transform.position.x, GetPlayerReference().transform.position.y, targetInstance.transform.position.z), .75f);
            }
            else
            {
                GetPlayerReference().transform.DOLookAt(targetInstance.transform.position, .75f);
            }
            
           //GetPlayerReference().GetComponent<CameraController>().RecenterThirdPersonCam(abilityComponent.reTargetTime);
        }

        //Does the modular component play an audio effect?
        if(abilityComponent.audioComponent != null)
        {
            if(modularAbilityInstance.TryGetComponent( out AudioSource audioSource))
            {
                audioSource.clip = abilityComponent.audioComponent.ambient;
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
        if(modularAbilityInstance.TryGetComponent<ModularAbilityComponent>(out ModularAbilityComponent modularAbilityComponent))
        {
            if(modularAbilityComponent.GetVFX() != null)
            {
                Body summonerBody = GetPlayerReference().GetComponent<Body>();
                switch (modularAbilityComponent.abilityComponent.vfxSpawnLocation)
                {
                    case AbilityComponent.VFXSpawnLocation.DEFAULT:
                        Instantiate(modularAbilityComponent.GetVFX(), GetPlayerReference().transform.position, GetPlayerReference().transform.rotation);
                        break;
                    case AbilityComponent.VFXSpawnLocation.BACK:
                        Instantiate(modularAbilityComponent.GetVFX(), summonerBody.Back.position, summonerBody.Back.rotation);
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
        if(modularAbilityInstance.TryGetComponent<ModularAbilityComponent>(out ModularAbilityComponent modularAbilityComponent))
        {
            if(modularAbilityComponent.GetProjectile() != null)
            {
                Projectile modularProjectile = Instantiate(modularAbilityComponent.GetProjectile(), GetMeleeSpawnPosition().position, GetMeleeSpawnPosition().rotation);
                modularProjectile.transform.SetParent(GetPlayerReference().transform);
                if(modularAbilityComponent.GetProjectile().GetVFX() != null)
                {
                    Instantiate(modularAbilityComponent.GetProjectile().GetVFX(), GetMeleeSpawnPosition().position, GetMeleeSpawnPosition().rotation);
                }
            }
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
                    EventsManager.instance.OnTriggerHitBox(hitBox.gameObject, this.meleeAttackInstance, false, modularAbilityComponent.GetAbilityComponent().hitBoxDuration);
                }
                else
                {
                    EventsManager.instance.OnTriggerHitBox(hitBox.gameObject, this.meleeAttackInstance, true, 0);
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
        //Reset Camera
        /*GetPlayerReference().GetComponent<CameraController>().RouteToCameraDisengage(GetPlayerReference(), cameraSettingsInstance);*/

        //Unlock player movement
        //Animation Controller Coroutine
        GetAnimationController().PlayerAnimationLock(movementLockDuration, GetPlayerReference().GetComponent<PlayerMovementController>(), isMobileMelee);
        
        cooldownTriggered = true;
        Destroy(this.gameObject);
    }

    //This function unparents a spell component
    public IEnumerator AbilityComponentStickToPlayerCoroutine(float duration, Transform abilityComponent)
    {
        yield return new WaitForSeconds(duration);
        abilityComponent.SetParent(null);
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

    public GameObject GetPlayerReference()
    {
        return this.playerReference;
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

    //Collision Code
    public void CollisionLogic(GameObject targetInstance, GameObject hurtBoxInstance, GameObject summonerInstance, AbilityComponent abilityComponent)
    {
        if(this.meleeAttackInstance != summonerInstance)
            return;
        StopCoroutine(animationCoroutine);

        this.targetInstance = targetInstance.transform;
        SetAbilityConnected(true);

        //Use multi viewpoint camera to render action scene
        //GetPlayerReference().GetComponent<CameraController>().EngageDynamicTargetLock(GetPlayerReference(), this.targetInstance, CameraGroup.FrameShotStyle.WIDESHOT);
    }



}