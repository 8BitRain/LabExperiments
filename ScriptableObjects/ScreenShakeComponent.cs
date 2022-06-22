using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ScreenShakeComponent", menuName = "ScriptableObjects/ScreenShakeComponent", order = 9)]
[System.Serializable]
public class ScreenShakeComponent : ScriptableObject
{
    [Header("ScreenShake Details")]
    public float amplitude;
    public float frequency;
    public float time;
    public bool useZoom;
    [Range(1,100)]
    public float zoom;
    public bool freezeScreen;
    [Range(0,5)]
    public float freezeScreenTimeScale;
    [Range(0,5)]
    public float realtimeDelay;
}