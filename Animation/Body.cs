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

    private Animator animator;

    [Header("VFX Settings")]
    public GameObject VFXBlock;
    public Transform VFXBlockSpawn;
    private GameObject VFXBlockInstance;

    [Header("UI Settings")]
    public Transform healthBarSpawn;
    public Transform staminaBarSpawn;

    [Header("Weapon Settings")]
    public Transform[] Weapons;
    
    // Start is called before the first frame update
    void Start()
    {
        this.animator = this.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if(animator.GetBool("Attacking"))
        {
            EnableWeaponTrails();
        }
        else
        {
            DisableWeaponTrails();
        }
    }

    public void EnableWeaponTrails()
    {
        foreach (Transform weapon in Weapons)
        {
            weapon.gameObject.GetComponentInChildren<TrailRenderer>().enabled = true;
        }
    }

    public void DisableWeaponTrails()
    {
        foreach (Transform weapon in Weapons)
        {
            weapon.gameObject.GetComponentInChildren<TrailRenderer>().enabled = false;
        }
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
            GetVFXBlock().transform.SetParent(VFXBlockSpawn);
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
