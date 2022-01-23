using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class EventsManager : MonoBehaviour
{
    public static EventsManager instance;

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
    public event Action<GameObject, bool, float> TriggerHitbox;

    public void OnAbilityWindowActiveLockInput(GameObject instance)
    {
        AbilityWindowActiveLockInput?.Invoke(instance);
    }

    public void OnAbilityWindowInactiveUnlockInput(GameObject instance)
    {
        AbilityWindowInactiveUnlockInput?.Invoke(instance);
    }

    public void OnTriggerHitBox(GameObject instance, bool isActive, float delayTime)
    {
        TriggerHitbox?.Invoke(instance, isActive, delayTime);
    }
}
