using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CollisionComponent", menuName = "ScriptableObjects/CollisionComponent", order = 3)]
[System.Serializable]
public class CollisionComponent : ScriptableObject
{
    
    public enum KnockBackDirection
    {
        Forward,
        Backward,
        Lateral,
        Up,
        None
    }

    public enum RecoilDirection
    {
        Forward,
        Backward,
        Lateral,
        Up,
        Down,
        None
    }

    [Header("CollisionDetails")]
    public KnockBackDirection knockbackDirection;
    public float knockbackAmount;
    public float knockbackTime;
    public float spinVelocity;
    public float squashAndStretchAmount;
    public float squashAndStretchTime;
    public float hpDamage;
    public float staminaCost = 10;
    public bool damageSelf = false;
    public bool canParryAttack = false;

    [Header("RecoilDetails")]
    public RecoilDirection recoilDirection;
    public float recoilAmount;
    public float recoilDuration;

    [Header("SFXDetails")]
    public AudioClip parrySFX;
}
