using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.Events;
using DG.Tweening;
using UnityEngine;



public class Adversary : MonoBehaviour
{
    [SerializeField]private Animator animator;
    [SerializeField]private AnimationController animationController;
    private Body bodyController;
    protected Transform target;
    public float seekPlayerRange;
    private float hitStunTimer = 1.0f;
    private float seekTransitionTimer = 1.0f;
    private float blockStunTimer = 0f;
    private bool actionWindow = true;
    public float moveSpeed;

    public enum State{IDLE, ATTACKING, DEFENDING, SEEKING, ORBITING, STUNNED}
    public GameObject followTarget; 
    private GameObject followTargetInstance;
    private State currentState;  

    public static event Action<GameObject, string, Camera> onUpdateDistanceUI;
    public static event Action<GameObject, string, Camera> onUpdateCurrentStateUI;
    public static event Action<GameObject, string, Camera> onUpdateBlockTimerUI;
    public static event Action<GameObject, string, Camera> onUpdateActionWindowUI;

    private void OnEnable()
    {
        HurtBox.recievedCollision += ApplyDamagedLogic;
        CombatController.onEnterTargetAttackState += StartDefense;
        CombatController.onCompleteTargetAttackState += EndDefense;
    }

    private void OnDisable()
    {
        HurtBox.recievedCollision -= ApplyDamagedLogic;
        CombatController.onEnterTargetAttackState -= StartDefense;
        CombatController.onCompleteTargetAttackState -= EndDefense;
    }

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        animationController = GetComponent<AnimationController>();
        bodyController = GetComponent<Body>();
    }

    // Update is called once per frame
    void Update()
    {
        
        /*if(hitStunTimer > 0 && animator.GetBool("HitStunned"))
        {
            hitStunTimer -= Time.deltaTime;
        }

        if(hitStunTimer <= 0 && animator.GetBool("HitStunned"))
        {
            animator.SetBool("HitStunned", false);
        }

        if(!animator.GetBool("HitStunned") && !animator.GetBool("Chasing"))
        {
            animationController.ChangeAnimationState(animator, "Player_idle");
        }*/

        if(target == null)
        {
            AssignTarget();
        }

        if(target != null)
        {
            onUpdateDistanceUI.Invoke(this.gameObject, "Current Distance: " + GetTargetDistance(), target.GetComponent<CameraController>().GetCameraInstance().GetComponent<Camera>());
            onUpdateCurrentStateUI.Invoke(this.gameObject, "Current State: " + currentState.ToString(), target.GetComponent<CameraController>().GetCameraInstance().GetComponent<Camera>());
            onUpdateBlockTimerUI.Invoke(this.gameObject, "Current Block Timer: " + blockStunTimer, target.GetComponent<CameraController>().GetCameraInstance().GetComponent<Camera>());
            onUpdateActionWindowUI.Invoke(this.gameObject, "Action Window Status: " + actionWindow, target.GetComponent<CameraController>().GetCameraInstance().GetComponent<Camera>());

        }


        switch (this.currentState)
        {
            case State.IDLE:
                animator.speed = 1;
                transform.LookAt(new Vector3(GetTargetPosition().x, transform.position.y, GetTargetPosition().z));
                animationController.ChangeAnimationState(animator, "Player_idle");
                if(GetTargetDistance() < 7)
                {
                    //ChangeState(State.ORBITING);
                }
                if(actionWindow)
                {
                    if(GetTargetDistance() < 7)
                    {
                        //Percent chance to attack
                        int attackChance = UnityEngine.Random.Range(-10, 10);
                        if(attackChance >= 0)
                            //TODO Change update reference
                            //Slide to target
                            transform.DOMove(transform.position + transform.forward*5, .5f);
                            GetComponent<HitBoxController>().ActivateHitBoxes(AttackEnums.Attacks.ATTACK001);
                            ChangeState(State.ATTACKING);
                            GetComponent<SFXController>().PlaySFX("attack001");
                        break;
                    }
                }
                if(GetTargetDistance() <= seekPlayerRange && GetTargetDistance() > 7)
                {
                    StartCoroutine(TransitionCoroutine(1.0f, State.SEEKING));
                }
                break;
            case State.ATTACKING:
                animationController.ChangeAnimationState(this.animator, "Player_melee_kick_001");
                break;
            case State.DEFENDING:

                if(blockStunTimer == 0)
                {
                    transform.LookAt(new Vector3(GetTargetPosition().x, transform.position.y, GetTargetPosition().z));
                    int defendIndex = UnityEngine.Random.Range(-10, 10);
                    if(defendIndex > 0)
                        this.GetComponent<VFXController>().PlayVFX(VFXController.VFX.Block, bodyController.BlockCore, bodyController.BlockCore.position, bodyController.BlockCore.eulerAngles, 1f);
                        animationController.ChangeAnimationState(animator, "Block_001");
                    if(defendIndex <= 0)
                        this.GetComponent<VFXController>().PlayVFX(VFXController.VFX.Block, bodyController.BlockCore, bodyController.BlockCore.position, bodyController.BlockCore.eulerAngles, 1f);
                        animationController.ChangeAnimationState(animator, "Block_002");
                    
                    //Freeze block animation
                    animator.speed = 0;
                }

                if(blockStunTimer < 1.0)
                    blockStunTimer += Time.deltaTime;
                    return;
                if(blockStunTimer >= 1.0)
                {
                    animator.speed = 1;
                    blockStunTimer = 0;
                    ChangeState(State.IDLE);
                }
                break;
            case State.ORBITING:
                CircleTarget();
                animationController.ChangeAnimationState(animator, "Player_run");
                //this.GetComponent<Body>().Head.parent.LookAt(new Vector3(10.015f, GetTargetPosition().y, GetTargetPosition().z));
                //float targetAngle = Mathf.Atan2(GetTargetPosition().x, GetTargetPosition().z) * Mathf.Rad2Deg;
                break;
            case State.SEEKING:

                if(target != null && GetTargetDistance() < seekPlayerRange && GetTargetDistance() > 7)
                {
                    SeekPlayer();
                }
                animationController.ChangeAnimationState(animator, "Player_run");

                if(GetTargetDistance() > seekPlayerRange ||  GetTargetDistance() < 7)
                {
                    StopSeekingPlayer();
                }
                break;
            case State.STUNNED:
                animationController.ChangeAnimationState(animator, "Player_evade_001");
                if(hitStunTimer > 0)
                    hitStunTimer -= Time.deltaTime;
                if(hitStunTimer <= 0)
                    ChangeState(State.IDLE);
                break;
            default :
                break;
        }
        
    }

    public void ApplyDamage(float damage)
    {       
        Debug.Log("Applying " + damage + " to " + this.gameObject.name);
    }

    public void ApplyKnockback(float knockbackAmount)
    {
        Debug.Log("Applying " + knockbackAmount + " to " + this.gameObject.name);
        transform.DOMove(-transform.forward * knockbackAmount, .5f);
    }

    public void AssignTarget()
    {
        foreach(GameObject obj in GameObject.FindGameObjectsWithTag("P1"))
        {
            target = obj.transform;
        }
    }

    public void ApplyHitStun(float hitStunAmount)
    {
        if(currentState == State.DEFENDING)
            return;

        hitStunTimer = hitStunAmount;
        hitStunTimer -= Time.deltaTime;
        animator.SetBool("HitStunned", true);
        GetComponent<SFXController>().PlaySFX("stunned");
        ChangeState(State.STUNNED);
    }

    public void ApplyDamagedLogic(GameObject instance, GameObject bodyPartCollidedWith, float damage, float knockback)
    {
        if(instance != this.gameObject){
            return;
        }
        
        Debug.Log(this.gameObject.name + " recieved Collision on : " + bodyPartCollidedWith);
        ApplyDamage(damage);
        if(currentState == State.DEFENDING)
        {
            //Unfreeze block animation
            animator.speed = 1;

            //Play ShieldBlock VFX
            this.GetComponent<VFXController>().PlayVFX(VFXController.VFX.HitShield, null, bodyPartCollidedWith.transform.position, new Vector3(-90,0,0), 3f);
            
            //Apply Knockback to target (based on tranform)
            ApplyKnockback(knockback);

            //PlayBlockSFX
            GetComponent<SFXController>().PlaySFX("block");
        }
        /*
            YoraiZor
            _scale?.Kill(true);
            _scale = _transform.DOPunchScale(Random.insideUnitSphere * _scaleStrength, _duration);
        */
        //_scale?.Kill(true);
        transform.DOPunchScale(UnityEngine.Random.insideUnitSphere * .5f, .5f);
        ApplyHitStun(1.0f);
    }

    public void StartDefense(GameObject instance)
    {
        if(target.gameObject != instance)
            return;

        blockStunTimer = 0;

        //RandomChance to block
        int defendChance = UnityEngine.Random.Range(-10, 10);
        
        if(defendChance >= 0)
        {
            ChangeState(State.DEFENDING);
        }
    }

    public void EndDefense(GameObject instance)
    {
        if(target.gameObject != instance)
            return;

        //blockStunTimer = 0;
        ChangeState(State.IDLE);
    }

    public void AttackComplete()
    {
        //Chance to combo 
        //Chance to defend
        //Chance to return to idle
        ChangeState(State.IDLE);

        GetComponent<HitBoxController>().DeactivateHitBoxes(AttackEnums.Attacks.ATTACK001);
        if(actionWindow){
            float delay = UnityEngine.Random.Range(4.5f, 8);
            StartCoroutine(ActionDelay(delay));
        }

    }

    public float GetTargetDistance()
    {
        if(target == null)
            return -1;
        
        Vector3 position = transform.position;
        float targetDistance = (position - target.position).magnitude;
        return targetDistance;
    }

    public Vector3 GetTargetPosition()
    {
        if (target == null)
            return new Vector3(0,0,0);
        
        return target.position;
    }

    public void SeekPlayer()
    {
        transform.LookAt(target);
        Vector3 position = transform.position;
        float targetDistance = (position - target.position).magnitude;

        Tweener seekTween = transform.DOMove(target.position - transform.forward*5, moveSpeed).SetSpeedBased().OnComplete(StopSeekingPlayer);
        seekTween.OnUpdate(delegate() {
            if(GetTargetDistance() < seekPlayerRange && GetTargetDistance() > 7)
            {
                Debug.Log(this.gameObject.name + " is in Tween Seek Player Update Loop");
                seekTween.ChangeEndValue(GetTargetPosition() - transform.forward*5, true).OnComplete(StopSeekingPlayer);
                transform.DOKill();
            }
        });
    }

    public void StopSeekingPlayer()
    {
        
        Debug.Log("Adversary Seeking Logic: Target now in close quarters, stop chasing " + transform.DOKill());
        //StartCoroutine(TransitionCoroutine(1.0f, State.IDLE));
        ChangeState(State.IDLE);
        
    }

    void CircleTarget()
    {
        if(followTargetInstance == null){
            followTargetInstance = Instantiate(followTarget, followTarget.transform.position, followTarget.transform.rotation);
            followTargetInstance.transform.RotateAround(GetTargetPosition(), Vector3.up, 60 * Time.deltaTime);
            Vector3 lookAtVector = new Vector3(followTargetInstance.transform.position.x, 0, followTargetInstance.transform.position.z);
            transform.LookAt(lookAtVector);
        } else {
            followTargetInstance.transform.RotateAround(GetTargetPosition(), Vector3.up, 60 * Time.deltaTime);
            
        }
        
        transform.RotateAround(GetTargetPosition(), Vector3.up, 60 * Time.deltaTime);
    }

    void MoveTorwardsTarget(Transform target, float duration)
    {
        transform.DOMove(TargetOffset(target.transform, new Vector3(0,0,0), 5), duration).OnComplete(StopSeekingPlayer).OnKill(StopSeekingPlayer);
    }

    public Vector3 TargetOffset(Transform target, Vector3 offset, float stoppingDistance)
    {
        Vector3 position;
        position = target.position;
        
        return Vector3.MoveTowards(position + offset + -transform.forward*stoppingDistance, transform.position, .95f);
    }

    public void ChangeState(State newState)
    {
        if (this.currentState == newState)
            return;

        this.currentState = newState;
    }

    IEnumerator TransitionCoroutine (float time, State state) 
    {
        float elapsedTime = 0;
        
        while (elapsedTime < time)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        ChangeState(state);
    }

    IEnumerator ActionDelay (float time)
    {
        actionWindow = false;

        float elapsedTime = 0;
        
        while (elapsedTime < time)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        actionWindow = true;

    }
}
