using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ragdoll : MonoBehaviour
{
    private Rigidbody rigidbody;
    public float force = 10;
    private bool appliedForce = false;
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
        //if(!appliedForce)
        //{
            rigidbody.AddForce(transform.forward * force);
            appliedForce = true;
        //}
    }
}
