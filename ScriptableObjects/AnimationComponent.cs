using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AnimationComponent", menuName = "ScriptableObjects/AnimationComponent", order = 4)]
[System.Serializable]
public class AnimationComponent : ScriptableObject
{
    [Header("AnimationDetails")]
    public Animations.AnimationState animation;
    public float animationStartDelay;
    public float animationEndDelay;

    [Header("HitStop")]
    public bool applyHitStop = false;
    public float hitStopDuration = 0;

    [Header("LayerWeights")]
    public float[] layerWeights;
}
