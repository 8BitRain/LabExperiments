using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using Cinemachine;
using UnityEngine.Events;
using System;
using UnityEngine;

[System.Serializable]
public class VFXEvent : UnityEvent <Transform> {}

public class HitBox : MonoBehaviour
{
    public LayerMask[] layers;
    public AbilityComponent abilityComponent;

    public CinemachineImpulseSource screenShakeImpulse;

    private bool rumble = true;

    private ArrayList targetsHit = new ArrayList();

    //Events
    public static event Action<GameObject, GameObject, float, float> collidedWithTarget;
    public static event Action<GameObject, GameObject, AbilityComponent> collision;

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
        print("HitBox OnTrigger Enter");
        foreach (var layer in layers)
        {
            //Collision w/ defined layer(s)
            //Layer check is unnesceary. Could just directly call TryGetComponent<HurtBox>()
            if(layer == (layer | (1 << other.gameObject.layer)))
            {
                if(HasHitBoxCollidedWithTarget(other.gameObject))
                {
                    print("HitBox: we already collided with target return");
                    return;
                }

                Gamepad.current.SetMotorSpeeds(0.25f,0.55f);
                StartCoroutine(RumbleCountdown(.2f));
                rumble = false;
                
                //screenShakeImpulse.GenerateImpulse();

                //Scriptable Object collison
                collision.Invoke(other.gameObject.GetComponent<HurtBox>().Agent, other.gameObject, abilityComponent);
                
            }
        }
    }

    public void TriggerHitbox(GameObject instance, bool isActivated, float delay)
    {
        if(instance != this.gameObject)
            return;

        
        if(isActivated)
        {
            ActivateHitBox();
        }
        else
        {
            if(delay == 0)
            {
                DeactivateHitBox();
            }
            else
            {
                print("Remove Hitbox");
                StartCoroutine(HitBoxDeactivationDelay(delay));
            }
        }
    }

    public void ActivateHitBox()
    {
        this.GetComponent<BoxCollider>().enabled = true;
        
    }

    public void DeactivateHitBox()
    {
        this.GetComponent<BoxCollider>().enabled = false;
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
