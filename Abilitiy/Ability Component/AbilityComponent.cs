using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AbilityComponent", menuName = "ScriptableObjects/AbilityComponent", order = 1)]
[System.Serializable]
public class AbilityComponent : ScriptableObject
{
    public string componentName;
    public bool isMobile;
    [Header("Travel Settings")]
    public Vector3 maxTravelDistance;
    //How long until the ability should start traveling
    public float travelTimeDelay;
    public float travelSpeed;
    public float timeToTravel;
    [Header("Collision Settings")]
    public bool collision;
    [Header("Lifetime Settings")]
    public float keepAliveTime;
    [Header("Scale Settings")]
    public bool canScale;
    public Vector3 maxScaleVector;
    public Vector3 minScaleVector;
    public float scaleStrength;
    public float scaleTimeDelay;
    public float timeToScale;
    [Header("Player Settings")]
    public bool stickToPlayer;
     [Range(0f, 5.0f)]
    public float stickToPlayerTime;
    [Header("HitBox Settings")]
    public float hitBoxTime;
    [Header("Collision Component")]
    public CollisionComponent collisionComponent;
}
