using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Cinemachine;
using UnityEngine.Events;

public class EventsManager : MonoBehaviour
{
    public static EventsManager instance;
    [Header("Parry Settings")]
    public UnityEvent onParry;
    public ScreenShakeComponent parryScreenShake;

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
    public event Action<GameObject, GameObject> Parried;

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

    public void OnParry(Transform hitBoxInstance, GameObject hitBoxSummonerA, GameObject hitBoxSummonerB, AudioClip parrySFX)
    {
        this.vfxTransform = hitBoxInstance;
        this.parrySFX = parrySFX;
        this.GetComponent<AudioSource>().clip = this.parrySFX;
        onParry.Invoke();
        CameraShake(hitBoxSummonerA, parryScreenShake);
        CameraShake(hitBoxSummonerB, parryScreenShake);
        Parried?.Invoke(hitBoxSummonerA, hitBoxSummonerB);
    }

    void CameraShake(GameObject summoner, ScreenShakeComponent screenShakeComponent)
    {
        if(screenShakeComponent != null)
        {
            Debug.Log("Trigger Parry Screenshake");
            //We check to see if the current instance has a cameraController attached. 
            if(summoner.TryGetComponent<CameraController>(out CameraController cameraControllerB))
            {
                GameObject virtualCam = cameraControllerB.GetCameraInstance().GetComponent<CinemachineBrain>().ActiveVirtualCamera.VirtualCameraGameObject;
                Debug.Log("Parry VirtualCam name: " + virtualCam.name);
                virtualCam.GetComponent<CinemachineScreenShake>().DoShake(screenShakeComponent);
            }
        }
    }

    public void SpawnVFX(GameObject VFX)
    {
        GameObject spawnedVFX = Instantiate(VFX, this.vfxTransform.position, this.vfxTransform.rotation);
    }
}
