using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Body : MonoBehaviour
{
    public Transform Head;
    public Transform BlockCore;
    public Transform Back;
    public Transform TargetLock;
    public GameObject hurtBox;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public HurtBox GetHurtBox()
    {
        return hurtBox.GetComponent<HurtBox>();
    }
}
