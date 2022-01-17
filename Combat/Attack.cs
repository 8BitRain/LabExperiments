using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
    //Attack Configuration
    [Header("Attack Configuration")]
    public AttackEnums.Attacks attackName;
    public string animationName;
    public float cooldown = 0;
    public float damage = 0;
    public float knockback = 0;
    public VFX VFX;
    public float vfxDelay = 0;
    public AudioClip audioClip;

    //Movement Properties
    [Header("Movement Properties")]
    public bool movePlayer = false;
    public bool movePlayerTowardsTarget = false;
}
