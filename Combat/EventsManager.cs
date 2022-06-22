using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Events;

public class EventsManager : MonoBehaviour
{
    public static EventsManager instance;
    public UnityEvent onParry;

    //VFX
    private Transform vfxTransform;

    //SFX
    private AudioClip parrySFX;

    //Singleton pattern
    private void Awake()
    {
        if(instance == null)
            instance = this;
        else
        {
            Destroy(this);
        }
    }

    public event Action<GameObject> AbilityWindowActiveLockInput;
    public event Action<GameObject> AbilityWindowInactiveUnlockInput;
    public event Action<GameObject, GameObject, bool, float, float> TriggerHitbox;

    public void OnAbilityWindowActiveLockInput(GameObject instance)
    {
        AbilityWindowActiveLockInput?.Invoke(instance);
    }

    public void OnAbilityWindowInactiveUnlockInput(GameObject instance)
    {
        AbilityWindowInactiveUnlockInput?.Invoke(instance);
    }

    public void OnTriggerHitBox(GameObject instance, GameObject summoner, bool isActive, float delayStart, float duration)
    {
        TriggerHitbox?.Invoke(instance, summoner, isActive, delayStart, duration);
    }

    public void OnParry(Transform vfxTransform, AudioClip parrySFX)
    {
        this.vfxTransform = vfxTransform;
        this.parrySFX = parrySFX;
        this.GetComponent<AudioSource>().clip = this.parrySFX;
        onParry.Invoke();
    }

    public void SpawnVFX(GameObject VFX)
    {
        GameObject spawnedVFX = Instantiate(VFX, this.vfxTransform.position, this.vfxTransform.rotation);
    }
}
