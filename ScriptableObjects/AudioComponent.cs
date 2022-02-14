using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AudioComponent", menuName = "ScriptableObjects/AudioComponent", order = 8)]
[System.Serializable]
public class AudioComponent : ScriptableObject
{
    [Header("Audio Details")]
    public AudioClip connected;
    public AudioClip missed;
    public AudioClip ambient;
}