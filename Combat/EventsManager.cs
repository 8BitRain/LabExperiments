using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Cinemachine;
using DG.Tweening;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

public class EventsManager : MonoBehaviour
{
    public static EventsManager instance;
    [Header("Parry Settings")]
    public UnityEvent onParry;
    public ScreenShakeComponent parryScreenShake;
    [Header("Player Defated Settings")]
    public UnityEvent onPlayerDefeated;
    //AI
    [Header("AI Settings")]
    public AIDirector AIDirector;

    [Header("UI Settings")]
    public Canvas ContinueGameScreen;

    [Header("Game Data")]
    public GameData gameData;
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
            Debug.Log("Destroying EventsManager instance");
            Destroy(this);
        }

        UpdateGameData();
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

    public void OnSlowTime(GameObject instance)
    {
        Debug.Log("SlowTime: Attack Dodged");
        Time.timeScale = .075f;
        PlayerMovementController movementController = instance.GetComponent<PlayerMovementController>();
        AbilityController abilityController = instance.GetComponent<AbilityController>();
        Animator animator = instance.GetComponent<Animator>();

        float speed = movementController.speed;
        //Speed up animation and movement speed to account for time slowed. 
        DOVirtual.DelayedCall(1f, () => {
            Time.timeScale = .1f;
            instance.GetComponent<Animator>().speed = (.1f*60);
            movementController.speed = speed * (.1f*60);

            //End the dodge movement tween so we can move the player in witch time
            if(abilityController.dodgeMovementTween != null)
            {
                abilityController.dodgeMovementTween.Kill();
            }
            //Manually override dodge status and enable movement
            animator.SetBool("Dodging", false);
            animator.SetBool("WitchTime", true);

            movementController.EnableMovement();

            //Enable normal animation and movement speed
            DOVirtual.DelayedCall(1.5f, () => {
                instance.GetComponent<Animator>().speed = 1;
                movementController.speed = speed;
                Time.timeScale = 1f;
                animator.SetBool("WitchTime", false);
            });
        });
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

    public void OnUpdateAIController(GameObject AIUnit)
    {
        AIDirector.spawnedWave.Remove(AIUnit);
    }

    public void LoadContinueScreen()
    {
        SceneManager.LoadScene("Continue");
    }

    public void UpdateGameData()
    {
        try
        {
            gameData.currentScene = SceneManager.GetActiveScene().name;
        }
        catch (System.Exception e)
        {
            Debug.LogError("There was an issue updating game data >_<. Here's why....\n" + e);
        }
    }
}
