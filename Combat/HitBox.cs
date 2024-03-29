﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using Cinemachine;
using UnityEngine.Events;
using System;
using UnityEngine;
using DG.Tweening;

[System.Serializable]
public class VFXEvent : UnityEvent <Transform> {}

public class HitBox : MonoBehaviour
{
    [Header("Hit Box Settings")]
    public LayerMask[] layers;
    public AbilityComponent abilityComponent;
    private GameObject Summoner;
    private GameObject entityGameObjectParent;

    [Header("Audio Settings")]
    public AudioComponent audioComponent;

    [Header("Parry Settings")]
    public ScreenShakeComponent screenShakeComponent;
    private bool parried = false;

    private bool rumble = true;

    private ArrayList targetsHit = new ArrayList();

    //Events
    public static event Action<GameObject, GameObject, float, float> collidedWithTarget;
    public static event Action<GameObject, GameObject, GameObject, AbilityComponent> collision;

    private void OnEnable()
    {
        EventsManager.instance.TriggerHitbox += TriggerHitbox;
    }

    private void OnDisable()
    {
        EventsManager.instance.TriggerHitbox -= TriggerHitbox;
    }

    private void OnDestroy()
    {
        EventsManager.instance.TriggerHitbox -= TriggerHitbox;
    }

    // Start is called before the first frame update
    void Start()
    {
        //enemyHit.AddListener(Agent.GetComponent<WeaponController>().GetEquippedWeapon().SpawnEnemyHitVFX);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        //Parry
        if(GetSummoner() != null && other.TryGetComponent<HitBox>(out HitBox otherHitBox))
        {
            Debug.Log("Colliding with a hitbox, " + "the gameObject causing collision is: " + otherHitBox.transform.parent);
            if(otherHitBox.abilityComponent.collisionComponent.canParryAttack && this.abilityComponent.collisionComponent.canParryAttack)
            {
                Debug.Log("Parry");
                EventsManager.instance.OnParry(this.transform, GetSummoner(), otherHitBox.GetSummoner(), this.abilityComponent.collisionComponent.parrySFX);
                //EventsManager summon SFX
                //EventsManager summon VFX position
                
                otherHitBox.Summoner.GetComponent<MeleeAttackController>().DestroyAttackInstance();
                this.Summoner.GetComponent<MeleeAttackController>().DestroyAttackInstance();
                this.parried = true;
            }
            return;
        }

        //Make sure the summoner that summoned the hitbox doens't have its own hurtbox take a collision from this instance
        if(GetSummoner() != null && other.gameObject.GetComponent<HurtBox>() != null && !parried)
        {
            Debug.Log("Summoner info: " + GetSummoner().name);
            
            //So Janky ugh this is to ensure hitboxes don't collide w/ their own hurtbox. 
            try
            {
                if(GetSummoner() == other.gameObject.GetComponent<HurtBox>().Agent)
                {
                    Debug.Log("Hitbox owner collided with itself, ignore this");
                    return;
                }
            }
            catch (System.Exception e)
            {
                Debug.Log("HitBox Error: "  + "HitBox Summoner" + GetSummoner() + "\n" + e);
                Debug.Log("HitBox Summoner" + GetSummoner() + " parent is " + GetSummoner().transform.parent.name);
                Debug.Log("HitBox Summoner's parent" + GetSummoner().transform.parent.name + " parent is " + GetSummoner().transform.parent.parent.name);
                /*Debug.Log("Hurtbox: " + other.gameObject.GetComponent<HurtBox>().gameObject.name + " belongs to " + other.gameObject.GetComponent<HurtBox>().Agent.name);
                try
                {
                    if(GetSummoner().transform.parent.gameObject == other.gameObject.GetComponent<HurtBox>().Agent)
                    {
                        Debug.Log("Hitbox owner collided with itself, ignore this");
                        return;
                    }
                }
                catch (System.Exception f)
                {
                    Debug.Log("HitBox Error: "  + "HitBox Summoner" + GetSummoner() + "\n" + f);
                    Debug.Log("HitBox Summoner" + GetSummoner() + " parent is " + GetSummoner().transform.parent.name);
                    Debug.Log("HitBox Summoner's parent" + GetSummoner().transform.parent.name + " parent is " + GetSummoner().transform.parent.parent.name);
                    Debug.Log("Hurtbox: " + other.gameObject.GetComponent<HurtBox>().gameObject.name + " belongs to " + other.gameObject.GetComponent<HurtBox>().Agent.name);
                }*/
            }
        }

        if(!parried)
        {
            print("HitBox belonging to: " + this.transform.parent.name + " OnTrigger Enter");
            foreach (var layer in layers)
            {
                //Collision w/ defined layer(s)
                //Layer check is unnesceary. Could just directly call TryGetComponent<HurtBox>()
                if(layer == (layer | (1 << other.gameObject.layer)))
                {
                    //Multi hit logic can go here. HasHitBoxCollidedWithTarget could return 0 (false), 1 (true), 2+ a multiple meaning multiple hits
                    if(HasHitBoxCollidedWithTarget(other.gameObject))
                    {
                        print("HitBox: we already collided with target return");
                        return;
                    }


                    Gamepad.current.SetMotorSpeeds(0.25f,0.55f);
                    StartCoroutine(RumbleCountdown(.2f));
                    rumble = false;


                    //Play Audio
                    if(audioComponent != null)
                    {
                        GetComponent<AudioSource>().clip = audioComponent.connected;
                        GetComponent<AudioSource>().Play();
                    }

                    //Scriptable Object collison
                    if(GetSummoner() != null)
                    {
                        try
                        {
                            //TODO interrogate the abilityComponent attached. Remember its based on the hitbox's abilityComponent reference
                            //Also pass screenshake component, or attach it to the ability component --<
                            collision.Invoke(other.gameObject.GetComponent<HurtBox>().Agent, other.gameObject, GetSummoner(), abilityComponent);
                        }
                        catch (System.Exception exception)
                        {
                            Debug.Log("Attempted to invoke collision but it failed: " + exception);
                            throw;
                        }
                    }

                }
            }
        }
    }

    public void TriggerHitbox(GameObject instance, GameObject summoner, bool isActivated, float delayStart, float duration)
    {
        if(instance != this.gameObject)
        {
            Debug.Log("Attempting to TriggerHitbox, but " + instance.name + " is not equivalent to: " + this.gameObject.name);
            return;
        }
        
        SetSummoner(summoner);
        
        if(isActivated)
        {
            //Enable hitbox after delay
            if(delayStart != 0)
            {
                Debug.Log("Delay Start for hitbox belonging to: " + GetSummoner().name + "for " + delayStart);
                DOVirtual.DelayedCall(delayStart, () => {
                    Debug.Log("Activate hitbox belonging to: " + GetSummoner().name);
                    ActivateHitBox();
                    StartCoroutine(HitBoxDeactivationDelay(duration));
                });
            }
            //Enable hitbox 
            else
            {
                ActivateHitBox();
                StartCoroutine(HitBoxDeactivationDelay(duration));
            }
        }
        else
        {
            ActivateHitBox();
        }
    }

    public void ActivateHitBox()
    {
        if(this.GetComponent<BoxCollider>() != null)
        {
            Debug.Log("Activating Box collider on " + this.transform.parent.name + " Hitbox");
            this.GetComponent<BoxCollider>().enabled = true; 
        }
        
        if(this.GetComponent<MeshCollider>() != null)
        {
            Debug.Log("Activating Mesh Collider collider on " + this.transform.parent.name + " Hitbox");
            this.GetComponent<MeshCollider>().enabled = true;
        }
    }

    public void DeactivateHitBox()
    {
        if(this.GetComponent<BoxCollider>() != null)
        {
            this.GetComponent<BoxCollider>().enabled = false; 
        }
        else if(this.GetComponent<MeshCollider>() != null)
        {
            this.GetComponent<MeshCollider>().enabled = false;
        }
        ClearTargetCollisions();
    }

    /*Checks if target has already been collided with during the hitbox's current active state. If yes, return false, if no add the enemy and return true */
    /*A smarter fire once so a swing can hit multiple enemies without triggering doubleHits on each enemy*/
    public bool HasHitBoxCollidedWithTarget(GameObject targetCollision)
    {
        foreach (GameObject target in targetsHit)
        {
            if(targetCollision == target)
            {
                return true;
            }
        }

        targetsHit.Add(targetCollision);
        return false;
    }

    public void ClearTargetCollisions()
    {
        targetsHit.Clear();
    }

    public void SetSummoner(GameObject summoner)
    {
        this.Summoner = summoner;
    }

    public GameObject GetSummoner()
    {
        return this.Summoner;
    }


    /*public string GetTargetPosition(GameObject target)
    {
        string x = target.transform.position.x.ToString();
        string y = target.transform.position.y.ToString();
        string z = target.transform.position.z.ToString();

        string rotX = target.transform.eulerAngles.x.ToString();
        string rotY = target.transform.eulerAngles.y.ToString();
        string rotZ = target.transform.eulerAngles.z.ToString();
        return x + "," + y + "," + z + "," + rotX + "," + rotY + "," + rotZ;
    }*/

    void OnDrawGizmos()
    {
        Gizmos.color = new Color(1, 0, 0, 1f);
        Gizmos.matrix = transform.localToWorldMatrix;
        if(this.GetComponent<BoxCollider>() != null)
        {
            if(this.GetComponent<BoxCollider>().enabled)
            {
                Gizmos.DrawWireCube(this.GetComponent<BoxCollider>().center, this.GetComponent<BoxCollider>().size);
            }
        }
        else
        {
            if(this.GetComponent<MeshCollider>() != null)
            {
                if(this.GetComponent<MeshCollider>().enabled)
                {
                    Gizmos.DrawWireMesh(this.GetComponent<MeshCollider>().sharedMesh, -1);
                }
            }
        }
    }

    IEnumerator RumbleCountdown (float seconds) 
    {
        int counter = 1;
        while (counter > 0) {
            yield return new WaitForSeconds (seconds);
            counter--;
        }
        Gamepad.current.SetMotorSpeeds(0,0);
        rumble = true;
    }

    IEnumerator HitBoxDeactivationDelay(float time)
    {
        
        yield return new WaitForSeconds (time);
        DeactivateHitBox();
    }
}
