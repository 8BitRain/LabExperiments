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

    [Header("SFX Settings")]
    public AudioSource runningSFX;
    public AudioSource fallingSFX;

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

        if(this.runningSFX != null && animator.GetBool("Running") || this.runningSFX != null && animator.GetBool("Chasing"))
        {
            Debug.Log(this.gameObject.name + " playing audio");
            if(!this.runningSFX.isPlaying)
            {
                this.runningSFX.Play();
            }
        }
        else
        {
            if(this.runningSFX != null && this.runningSFX.isPlaying)
            {
                this.runningSFX.Stop();
            }
        }

        FallingSFXListener();
    }
    
    public void FallingSFXListener()
    {
        if(this.fallingSFX != null && animator.GetBool("Falling"))
        {
            if(!this.fallingSFX.isPlaying)
            {
                this.fallingSFX.Play();
            }
        }
        else
        {
             if(this.fallingSFX != null && fallingSFX.isPlaying)
            {
                this.fallingSFX.Stop();
            }
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
