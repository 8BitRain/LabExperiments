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
        Up
    }

    [Header("CollisionDetails")]
    public KnockBackDirection knockbackDirection;
    public float knockbackAmount;
    public float knockbackTime;
    public float spinVelocity;
    public float squashAndStretchAmount;
    public float squashAndStretchTime;
    public float hpDamage;
}
