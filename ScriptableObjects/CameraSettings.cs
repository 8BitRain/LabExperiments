using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CameraSettings", menuName = "ScriptableObjects/CameraSettings", order = 10)]
[System.Serializable]
public class CameraSettings : ScriptableObject
{
    [Header("Camera Settings")]
    public CameraGroup.FrameShotStyle frameShotStyle;
    public float dutch;
    public Vector3 cameraOffset;
    public float cameraBlendTime;
}