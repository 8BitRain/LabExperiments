using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using DG.Tweening;
using Cinemachine;
using UnityEngine.Events;
using UnityEngine;

public class Bladeclubber : MonoBehaviour
{
    [Header("Bladeclubber Configuration")]
    public Transform Head;
    public Transform Body;

    public LayerMask Player;
    
    public BoxCollider hitBox;

    protected NavMeshAgent navMeshAgent;
    protected Transform target;

    public float attackCombo = 1;


    public Transform[] weaponsL;
    public Transform[] weaponsR;
    public bool hitBoxEnabled = false;

    public Animator animator;
    public float combatTimer = 0;
    public float seekPlayerRange = 15;

    public HealthBar healthBar;
    public PoiseMeter poiseMeter;

    public float hitStunDuration = 0.5f;
    public float shakeMultiplier = 0.03f;
    public float shakeSpeed = 40f;

    [Header("UI Controller")]
    public GameObject UIController;

    [Header("VFX Controller")]
    public GameObject VFXController;

    [Header("Special State GameObjects")]
    public GameObject floatingStateZone;
    private GameObject _floatingStateZoneInstance;

    [Header("Bladeclubber ON/OFF")]
    public bool targetDummy;

    [Header("Bladeclubber Combat Believability")]
    public bool applyHitStun = true;
    public float knockbackForce = 10.0f;

    //Events
    public UnityEvent<Bladeclubber> OnTrajectory;



    protected bool _floatState = false;
    protected bool _stunned = false;
    // Start is called before the first frame update
    void Start()
    {
        if(this.GetComponent<NavMeshAgent>() != null)
        {
            print("Assinged nav mesh");
            this.navMeshAgent = this.GetComponent<NavMeshAgent>();

            if(targetDummy == true)
            {
                transform.GetComponent<NavMeshAgent>().enabled = false;
            }
        }
        if(this.GetComponent<Animator>() != null)
        {
            this.animator = this.GetComponent<Animator>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(target == null)
        {
            AssignTarget();
            
        }

        if(target != null)
        {
            transform.LookAt(new Vector3(target.position.x, transform.position.y, target.position.z));
            //transform.DOLookAt(new Vector3)
        }

        if(!_floatState && !targetDummy)
        {
            if(target == null)
            {
                AssignTarget();
            }

            if(!this.animator.GetBool("Attacking") && !GetStunStatus())
            {
                SeekPlayer();   
            }
            
            if(!GetStunStatus())
            {
                EngageCombat();
            }
        } 
    }

    public void SeekPlayer()
    {
        if(this.navMeshAgent != null && target != null)
        {
            

            transform.LookAt(target);
            Vector3 position = transform.position + hitBox.center;
            float targetDistance = (position - target.position).magnitude;

            if(targetDistance <= seekPlayerRange)
            {
                //print("Navigate to player");
                navMeshAgent.isStopped = false;
                if(navMeshAgent.enabled)
                {
                    
                    animator.SetBool("Running", true);
                    navMeshAgent.SetDestination(target.transform.position);
                    
                }
                navMeshAgent.stoppingDistance = 5;
            }

            if(targetDistance < 15 && targetDistance > 5 && !this.animator.GetBool("Attacking")) 
            {
                //navMeshAgent.
                //print("Avoid player");
                //OrbitTarget();
            }

            if(targetDistance > seekPlayerRange)
            {
                navMeshAgent.isStopped = true;
                animator.SetBool("Running", false);
            }
        }
    }

    public void AssignTarget()
    {
        foreach(GameObject obj in GameObject.FindGameObjectsWithTag("P1"))
        {
            if(obj.name == "Eston")
            {
                target = obj.transform;
                //print("Found Eston, seeking her");
            }
        }
    }

    public void OrbitTarget()
    {
        transform.RotateAround(target.transform.position, Vector3.up, 60 * Time.deltaTime);
        //print("Orbiting Target: " + target.transform.position);
    }

    public void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Vector3 position = transform.position + this.hitBox.center;
        Gizmos.DrawWireSphere(position, seekPlayerRange);

    }

    public void EngageCombat()
    {
        //sentinel value
        float targetDistance = -1;
        if(target != null)
        {
            targetDistance = (transform.position - target.position).magnitude;
        }
        
        
        //Slide forward with combo
        navMeshAgent.SetDestination(new Vector3(0,0,3));
        if(this.animator.GetBool("Attacking"))
        {
            //Force the target into a combat state
            //TODO: Test with multiple combatants. How does the state respond?
            target.GetComponent<ThirdPersonMovement>().SetCombatState(true, this.transform);
        }
        else
        {
            //TODO: Test with multiple combatants. How does the state respond?
            //Concern:
            if(target != null)
            {
                target.GetComponent<ThirdPersonMovement>().SetCombatState(false, null);
            }
            //TODO: Enable this navmesh to have the agent reset to the track
            navMeshAgent.enabled = true;
            //this.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
        }

        //Start Combat String 1
        //Play the animation
        if(targetDistance < 5 && targetDistance != -1)
        {
            if(!this.animator.GetBool("Attacking"))
            {
                animator.SetBool("Running", false);
                animator.SetTrigger("Initiate-AttackString");
                animator.SetBool("Attacking", true);
                animator.SetBool("WindUp-AttackStringI", true);
                //animator.Play("Base Layer.AttackStringI");
                this.attackCombo = 0;   
            }
        }


        if(this.animator.GetCurrentAnimatorStateInfo(0).IsName("AttackStringI"))
        {
            //Get the time duration of AttackStringII
            AnimatorStateInfo combatAnimatorStateInfo = this.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0);
            float combatantAnimationTimeElapsed = combatAnimatorStateInfo.length;

            print("Combatant Time Elapsed: " + combatantAnimationTimeElapsed);

            print("Current Animation State is: Attack String I");
            weaponsR[0].GetComponent<Weapon>().EnableWeaponTrail();
            
            //Hitbox only activates when triggered by an event in the Bladeclubber animation. This way we have control over the timing without having to do timing calculations here.
            if(hitBoxEnabled)
            {
                weaponsR[0].GetComponent<BoxCollider>().enabled = true;
            } else 
            {
                weaponsR[0].GetComponent<BoxCollider>().enabled = false;
            }
            

            weaponsL[0].GetComponent<Weapon>().DisableWeaponTrail();
            weaponsL[0].GetComponent<BoxCollider>().enabled = false;

            if(animator.GetBool("WindUp-AttackStringI"))
            {
                navMeshAgent.SetDestination(target.transform.position);
                transform.LookAt(target.GetComponent<ThirdPersonMovement>().Body.transform.position);
                animator.SetBool("WindUp-AttackStringI", false);
                //animator.SetBool("WindUp-AttackStringII", true);
                //Cache wind up for second hit
            }
            //transform.LookAt(target.GetComponent<ThirdPersonMovement>().Body.transform.TransformPoint(target.GetComponent<ThirdPersonMovement>().Body.transform.position));
            print("Bladeclubbers's position: " + this.transform.position);
            print("Target's Body Position: " + target.GetComponent<ThirdPersonMovement>().Body.transform.position);
            print("Target's Box Collider Position: " + target.GetComponent<BoxCollider>().transform.position);
        }

        if(this.animator.GetCurrentAnimatorStateInfo(0).IsName("AttackStringII"))
        {
            //Get the time duration of AttackStringII
            AnimatorStateInfo combatAnimatorStateInfo = this.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0);
            float combatantAnimationTimeElapsed = combatAnimatorStateInfo.length;
            //if(!animator.GetBool())
            print("Current Animation State is: Attack String II");
            weaponsL[0].GetComponent<Weapon>().EnableWeaponTrail();

            //Hitbox only activates when triggered by an event in the Bladeclubber animation. This way we have control over the timing without having to do timing calculations here.
            if(hitBoxEnabled)
            {
                weaponsL[0].GetComponent<BoxCollider>().enabled = true;
            } else 
            {
                
                weaponsL[0].GetComponent<BoxCollider>().enabled = false;
            }

            weaponsR[0].GetComponent<Weapon>().DisableWeaponTrail();
            weaponsR[0].GetComponent<BoxCollider>().enabled = false;

            if(animator.GetBool("WindUp-AttackStringII"))
            {
                //Currently Bladeclubber looks a bit above the player character. This causes the Axe swing in AttackStringII to uppercut the player to the sky
                GameObject aboveTarget = new GameObject();
                //aboveTarget.transform.position = 
                transform.LookAt(target.transform.position + transform.up);
                animator.SetBool("WindUp-AttackStringII", false);
            }
        }

        if(!this.animator.GetCurrentAnimatorStateInfo(0).IsName("AttackStringI") && !this.animator.GetCurrentAnimatorStateInfo(0).IsName("AttackStringII"))
        {

            animator.SetBool("Attacking", false);
            //print("Not attacking");
            weaponsL[0].GetComponent<Weapon>().DisableWeaponTrail();
            weaponsR[0].GetComponent<Weapon>().DisableWeaponTrail();
            //animator.ResetTrigger("Initiate-AttackString");

            weaponsL[0].GetComponent<BoxCollider>().enabled = false;
            weaponsR[0].GetComponent<BoxCollider>().enabled = false;

            weaponsL[0].GetComponent<Weapon>().ResetHitCounters();
            weaponsR[0].GetComponent<Weapon>().ResetHitCounters();
        }
        
        //Create a random chance for the combo string to happen
        
        //Transition to combat string 2
        //Play the animation
    }

    public void DisengageCombat()
    {
        animator.SetBool("Attacking", false);
        //weaponsL[0].GetComponent<Weapon>().DisableWeaponTrail();
        //weaponsR[0].GetComponent<Weapon>().DisableWeaponTrail();
    }

    

    public void EnableHitBoxes()
    {
        Debug.Log("HitBoxEnabled");
        hitBoxEnabled = true;
    }

    public void DisableHitBoxes()
    {
        Debug.Log("HitBoxDisabled");
        hitBoxEnabled = false;
    }

    public void HandleHitStun()
    {

        if(!applyHitStun)
            return;

        //Shake the target
        Debug.Log("Combat: Poise UP add some hitstun shake");
        Debug.Log("Combat: NavMesh Value: " + transform.GetComponent<NavMeshAgent>().enabled);

        if(targetDummy != false)
        {
            transform.GetComponent<NavMeshAgent>().enabled = false;
        }
        transform.GetComponent<Rigidbody>().isKinematic = false;
        transform.GetComponent<Rigidbody>().useGravity = false;

        Debug.Log("Combat: NavMesh Value: " + transform.GetComponent<NavMeshAgent>().enabled);
        animator.SetBool("Attacking", false);
        PlayHitStunAnimation();
        SetStunStatus(true);
        StartCoroutine(HitStunEnumerator(hitStunDuration));
        
    }

    public void ApplyKnockBack(Transform hitBoxContactPoint)
    {
        /* If applying a force to an active rigidbody, or wanting to ensure the rigidbody does not interfere. Set the following values*/
        transform.GetComponent<Rigidbody>().isKinematic = true;
        transform.GetComponent<Rigidbody>().useGravity = false;

        //Non Physics Implementation I.
        /*Vector3 forward = -hitBoxContactPoint.transform.forward * knockbackForce;
        Vector3 forwardVectorWithY = Quaternion.AngleAxis(90, transform.up) * forward;
        transform.DOMove(transform.position + transform.up*2 + forwardVectorWithY, .5f);*/

        //Non Physics Implementation II. (Apply a vector movement in the upwards direction as well)
        /*transform.DOMove(transform.position + Vector3.up*2 + hitBoxContactPoint.transform.forward * knockbackForce, .5f);*/

        //Non Physics Implementation III 
        transform.DOMove(transform.position + hitBoxContactPoint.transform.forward * knockbackForce, .5f);



        //transform.GetComponent<Rigidbody>().AddRelativeForce(-hitBoxContactPoint.transform.forward * knockbackForce);
        //var moveDirection = transform.position - hitBoxContactPoint.transform.position;
        //transform.GetComponent<Rigidbody>().AddForce(moveDirection * knockbackForce);
        PlayHitStunAnimation();

        //transform.GetComponent<Rigidbody>().isKinematic = true;
        

        StartCoroutine(KnockBackEnumerator(1.1f));

        IEnumerator KnockBackEnumerator (float time){
            float elapsedTime = 0;
            while (elapsedTime < time) {
                elapsedTime+= Time.deltaTime;
                yield return null;
            }
            animator.SetTrigger("EndHitStun");

            //transform.DORotate(new Vector3(0,0,0), .2f);

            /* If applying a force to an active rigidbody, or wanting to ensure the rigidbody does not interfere. Reset the following values*/
            transform.GetComponent<Rigidbody>().isKinematic = false;
            transform.GetComponent<Rigidbody>().useGravity = true;
        }

        //Physics Implementation
        
    }

    public bool GetStunStatus()
    {
        return _stunned;
    }

    public void SetStunStatus(bool isStunned)
    {
        _stunned = isStunned;
    }

    public void PlayHitStunAnimation()
    {
        int animationIndex = Random.Range(0,2);

        switch (animationIndex)
        {
            case 0:
                animator.Play("rig|Hit");
                transform.DOPunchRotation(new Vector3(45,0,0), 1.0f, 5, 1);
                break;
            case 1:
                animator.Play("rig|Hit2");
                transform.DOPunchRotation(new Vector3(0,-45,0), 1.0f, 5, 1);
                break;
            case 2:
                animator.Play("rig|Hit3");
                transform.DOPunchRotation(new Vector3(0,45,0), 1.0f, 5, 1);
                break;
            default:
                break;
        }
    }

    IEnumerator HitStunEnumerator (float time) 
    {
        
        float elapsedTime = 0;
        while (elapsedTime < time) {
            float shakeFactor = Mathf.Sin(Time.time * shakeSpeed) * shakeMultiplier;
            Debug.Log("HitStun: " + transform.name + " Time: " + elapsedTime + " Timestamp: " + Time.time);
            Vector3 objectShakeVector = new Vector3(transform.transform.position.x + shakeFactor , transform.transform.position.y + shakeFactor, transform.transform.position.z + shakeFactor);
            transform.transform.position = objectShakeVector;
            Debug.Log("HitStun: Shake vector: " + objectShakeVector);
            elapsedTime+= Time.deltaTime;
            yield return null;
        }
        
        if(targetDummy != false)
        {
            transform.GetComponent<NavMeshAgent>().enabled = false;
        }
        transform.GetComponent<Rigidbody>().isKinematic = true;
        animator.SetTrigger("EndHitStun");
        SetStunStatus(false);



    }

    public void CreateFloatingStateZone(GameObject target)
    {
        GameObject instance = Instantiate(floatingStateZone, new Vector3(target.transform.position.x, target.transform.position.y - 3, target.transform.position.z), transform.rotation);
        instance.GetComponent<FloatingAttack>().SetAffectedTarget(target);
        instance.GetComponent<FloatingAttack>().SetFloatingEntity(this.gameObject);
        _floatingStateZoneInstance = instance;
    }

    public void HandleFloatState(GameObject aggresor)
    {
        Debug.Log("Combat: Floating Target");

        //Create a floating state zone. This allows the player to attack the enemy with a special move
        if(!_floatState)
        {
            //UIController.GetComponent<UIController>().CreateFloatingStateZoneUI(this.gameObject);
            CreateFloatingStateZone(this.gameObject);
        }
        
        if(targetDummy != false)
        {
            transform.GetComponent<NavMeshAgent>().enabled = false;
        }
        //throwing in if block to limit accidental triggers
        if(!_floatState)
        {
            StartCoroutine(FloatStateEnumerator(7, aggresor.GetComponent<ThirdPersonMovement>()));
        }
    }

    IEnumerator FloatStateEnumerator (float time, ThirdPersonMovement aggresor) 
    {
        _floatState = true;

        Vector3 startingPos  = transform.position + (Vector3.up * 5);
        Vector3 finalPos = transform.position + (Vector3.up * 15);
        float elapsedTime = 0;
        
        Debug.Log("Combat: Floating Target");

        //knock the target up.
        Debug.Log("Combat: Float Force " + 300);
        transform.GetComponent<Rigidbody>().useGravity = false;
        transform.GetComponent<Rigidbody>().isKinematic = true;
        transform.GetComponent<Rigidbody>().useGravity = true;
        //transform.GetComponentInParent<Rigidbody>().AddForce((Vector3.up) * 200);
        transform.rotation = Quaternion.Euler(0,0,90);


        Vector3 startingRotation = new Vector3(0,0,0);
        Vector3 endingRotation = new Vector3(0,30,0);
        
        //Rise
        while (elapsedTime < time)
        {
            transform.position = Vector3.Lerp(startingPos, finalPos, (elapsedTime / time));
            
            if(targetDummy != false)
            {
                transform.GetComponent<NavMeshAgent>().enabled = false;
            }
            

            //Sping the target as its being knocked up
            transform.Rotate(Vector3.Lerp(endingRotation, startingRotation, (elapsedTime / time))); 
            //transform.Rotate(endingRotation); 
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        //Fall
        elapsedTime = 0;
        time = 2;
        finalPos = transform.position;
        while (elapsedTime < time)
        {
            transform.position = Vector3.Lerp(finalPos, startingPos, (elapsedTime / time));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        //tilt the target so it looks like its spinning.
        transform.GetComponentInParent<Rigidbody>().useGravity = true;
        transform.GetComponent<Rigidbody>().isKinematic = false;
        
        if(targetDummy != false)
        {
            transform.GetComponent<NavMeshAgent>().enabled = false;
        }

        _floatState = false;

        //Destroy floating state zone at the end of floating state
        if(_floatingStateZoneInstance != null)
        {
            Destroy(_floatingStateZoneInstance);
        }

    }

    public Vector3 TargetOffset(Transform target)
    {
        Vector3 position;
        position = target.position;
        return Vector3.MoveTowards(position, transform.position, .95f);
    }


    void OnPlayerTrajectory(EnemyScript target)
    {
        if (target == this)
        {
            //StopEnemyCoroutines();
            //isLockedTarget = true;
            //PrepareAttack(false);
            //StopMoving();
        }
    }
    
}
