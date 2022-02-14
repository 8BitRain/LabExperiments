using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MovementComponent", menuName = "ScriptableObjects/MovementComponent", order = 7)]
[System.Serializable]
public class MovementComponent : ScriptableObject
{
    [Header("Movement Details")]
    public float currentSpeed = 6.0f;
    public float defaultSpeed = 6.0f;
    public float topSpeed = 15.0f;
    public float acceleration = .1f;
    public float friction = .025f;
    public bool moveCharacter;
    public float gravity = -9.81f;
    public bool applyGravity = true;
    public bool move = true;


}
