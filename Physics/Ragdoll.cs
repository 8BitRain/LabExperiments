using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ragdoll : MonoBehaviour
{
    private Rigidbody rigidbody;
    public float force = 10;
    private bool appliedForce = false;
    private bool initialized = false;
    private Transform collisionPoint;

    // Start is called before the first frame update
    void Start()
    {
        if(TryGetComponent<Rigidbody>(out Rigidbody rigidbody))
        {
            this.rigidbody = rigidbody;
        }
    }

    void FixedUpdate()
    {
        if(!appliedForce)
        {
            if(initialized)
            {
                Debug.Log(this.gameObject.name + ": applying ragdoll force ");
                if(collisionPoint != null)
                {
                    Debug.Log(this.gameObject.name + ": applying ragdoll force at collision point");
                    rigidbody.AddForce(collisionPoint.forward + collisionPoint.up * force);
                    appliedForce = true;
                }
                else
                {
                    Debug.Log(this.gameObject.name + ": applying ragdoll force in forward direction");
                    rigidbody.AddForce(transform.forward * force);
                    appliedForce = true;
                }
            }
        }
    }

    public void Initialize(Transform contactPoint)
    {
        Debug.Log(contactPoint.gameObject.name + "contact point");
        collisionPoint = contactPoint;
        initialized = true;
    }


}
