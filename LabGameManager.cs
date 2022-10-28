using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;
using UnityEngine.UI;
using DG.Tweening;

public class LabGameManager : MonoBehaviour
{
    [Header("Spawned Players")]
    public Transform[] spawnedPlayers;

    [Header("Spawn Player Event Logic")]
    public bool lockSpawnEvent = false;
    public UnityEvent playerSpawnEvent;

    public void Start()
    {
        if(playerSpawnEvent != null && lockSpawnEvent != true)
        {
            Debug.Log("CINEMATIC INTRO FOR 5 SECONDS");
            StopAllCoroutines();
            StartCoroutine(PlayerSpawnLogic(5f)); 
        }
    }


    public IEnumerator PlayerSpawnLogic(float time)
    {
        Debug.Log("CINEMATIC COROUTINE START COMPLETE, Load Player");
        yield return new WaitForSecondsRealtime(time);
        Debug.Log("CINEMATIC INTRO COMPLETE, Load Player");
        playerSpawnEvent.Invoke();
        yield return new WaitForSecondsRealtime(.5f);
        TriggerPlayerStaminaRegen();
        yield break;
    }

    public void TriggerPlayerStaminaRegen()
    {
        if(spawnedPlayers[0].GetComponent<Status>().startStaminaEmpty)
        {
            spawnedPlayers[0].GetComponent<Status>().SetStamina(spawnedPlayers[0].gameObject, 1);
        }
    }

}