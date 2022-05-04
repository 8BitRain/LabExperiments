using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AbilityComponent", menuName = "ScriptableObjects/AbilityComponent", order = 1)]
[System.Serializable]
public class AbilityComponent : ScriptableObject
{
    public string componentName;
    public bool isMobile;
    public enum MovementDirection
    {
        Forward,
        Backward,
        ForwardDiagonalLeft,
        ForwardDiagonalRight,
        BackwardDiagonalRight,
        BackwardDiagonalLeft,
        Right,
        Left,
        Up,
        Down,
        None
    }
    [Header("Travel Settings")]
    public MovementDirection travelDirection;
    public Vector3 maxTravelDistance;
    public float travelAmount;
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
    public MovementDirection playerMovementDirection;
    public Vector3 playerMovementVector;
    public float playerMovementAmount;
    public float playerMovementTime;
    public float playerMovementDelay;
    public bool lookAtTarget;
    public bool lookAtTargetLockY;
    public float reTargetTime;
    [Header("Animation Settings")]
    public AnimationComponent animationComponent;
    [Header("HitBox Settings")]
    public float hitBoxStartDelay;
    public float hitBoxDuration;
    [Header("Collision Component")]
    public CollisionComponent collisionComponent;
    [Header("Audio Component")]
    public AudioComponent audioComponent;
    [Header("Camera Settings")]
    public CameraSettings cameraSettings;
    [Header("ScreenShake Settings")]
    public ScreenShakeComponent screenShakeComponent;
    [Header("VFX Settings")]
    public VFXSpawnLocation vfxSpawnLocation;
    public enum VFXSpawnLocation
    {
        DEFAULT,
        BACK,
    }
}
