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

    public GameObject VFXBlock;
    public Transform VFXBlockSpawn;
    private GameObject VFXBlockInstance;
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

    public GameObject GetVFXBlock()
    {
        return VFXBlockInstance;
    }

    public void DisplayVFXBlock()
    {
        if(this.GetVFXBlock() == null)
        {
            this.VFXBlockInstance = Instantiate(VFXBlock, VFXBlockSpawn.position, VFXBlockSpawn.rotation);
        }
    }

    public void HideVFXBlock()
    {
        if(GetVFXBlock() != null)
        {
            Destroy(GetVFXBlock());
        }
    }
}
