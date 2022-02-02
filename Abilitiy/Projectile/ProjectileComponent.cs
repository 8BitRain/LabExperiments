using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ProjectileComponent", menuName = "ScriptableObjects/ProjectileComponent", order = 6)]
[System.Serializable]
public class ProjectileComponent : ScriptableObject
{
    [Header("Movement Details")]
    public bool canTravel;
    public float velocity;
    public float time;
    public float keepAliveTime;

    [Header("Rotation Details")]
    public Vector3 initialRotation;
    public bool canRotate;
    [Range(-1,10)]
    public int rotationLoops;
    public float rotationTime;
    public Vector3 rotationVector;
}
