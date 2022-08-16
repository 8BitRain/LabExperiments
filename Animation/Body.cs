using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

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
    public AudioSource jumpingSFX;
    public AudioSource voxSFX01;
    public AudioSource landSFX;

    [Header("Ragdoll Settings")]
    public GameObject ragdoll; 

    private bool resetJumpingSFX = false;
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
        JumpingSFXListener();
        LandSFXListener();
        VoxListener();
    }
    
    public void FallingSFXListener()
    {
        if(this.fallingSFX != null && animator.GetBool("Falling"))
        {
            if(!this.fallingSFX.isPlaying)
            {
                this.fallingSFX.Play();
                this.fallingSFX.volume = 0;
                fallingSFX.DOFade(1, 2.5f);
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

    public void JumpingSFXListener()
    {
        if(this.jumpingSFX != null && animator.GetBool("Jumping") && !resetJumpingSFX)
        {
            if(!this.jumpingSFX.isPlaying)
            {
                Debug.Log("SFX: " + "play jumping sfx");
                this.jumpingSFX.Play();
                resetJumpingSFX = true;
            }
        }

        //Added to ensure sfx only plays once
        if(resetJumpingSFX && !animator.GetBool("Jumping"))
        {
            this.resetJumpingSFX = false;
        }
    }

    public void LandSFXListener()
    {
        if(this.landSFX != null && animator.GetBool("Landing"))
        {
            if(!this.landSFX.isPlaying)
            {
                this.landSFX.Play();
            }
        }
        else
        {
             if(this.landSFX != null && landSFX.isPlaying)
            {
                this.landSFX.Stop();
            }
        }
    }

    public void VoxListener()
    {
        if(this.voxSFX01 != null && animator.GetBool("Jumping"))
        {
            if(!this.voxSFX01.isPlaying)
            {
                this.voxSFX01.Play();
            }
        }
        else
        {
             if(this.voxSFX01 != null && voxSFX01.isPlaying)
            {
                this.voxSFX01.Stop();
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

    public void InitiateRagdoll()
    {
        if(ragdoll != null)
        {
            Instantiate(ragdoll);
        }
    }
}
