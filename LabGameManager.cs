using System.Collections;
using System.Collections.Generic;
using Cinemachine;
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
    public UnityEvent playerSpawnEvent;

    public void Start()
    {
        if(playerSpawnEvent != null)
        {
            StartCoroutine(PlayerSpawnLogic(5f)); 
        }
    }


    public IEnumerator PlayerSpawnLogic(float time)
    {
        yield return new WaitForSeconds(time);
        playerSpawnEvent.Invoke();
    }    

}