﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class StaminaBar : MonoBehaviour
{

    public Slider slider;
    public Slider redChunk;

    private float maxStamina;

    private GameManager gameManager;

    private GameObject ownerInstance;
    private Status ownerInstanceStatus;

    private Coroutine refillCoroutine;
    private Tween refillTween;

    private void OnEnable()
    {
        Status.onStaminaStatusChange += SetST;
    }

    private void OnDisable()
    {
        Status.onStaminaStatusChange -= SetST;
    }
    
    public void SetStamina(float stamina)
    {

        //Stop the coroutine that refills the stamina bar
        if(refillCoroutine != null)
        {
            StopCoroutine(refillCoroutine);
        }

        //End the Stamina Refill tween, embeded in refill courotine, that refills stamina slider.
        refillTween.Kill();

        if(ownerInstance != null)
        {
            refillCoroutine = StartCoroutine(refillStaminaBar(ownerInstance.GetComponent<Status>().staminaRefillDelay, ownerInstance.GetComponent<Status>().staminaRefillTime));
        }
        
        slider.value = stamina;

        if(redChunk != null)
        {
            if(ownerInstance != null)
            {
                StartCoroutine(redChunkDelayDecrease(ownerInstance.GetComponent<Status>().staminaRefillDelay - 0.25f));
            }
        }
    }

    public void SetMaxStamina(float stamina)
    {
        maxStamina = stamina;
        slider.maxValue = stamina;
        slider.value = stamina;

        if(redChunk != null)
        {
            redChunk.maxValue = stamina;
            redChunk.value = stamina;
        }
    }

    public float GetMaxStamina()
    {
        return maxStamina;
    }

    public float GetStamina()
    {
        return slider.value;
    }

    public void SetOwnerInstance(GameObject ownerInstance)
    {
        this.ownerInstance = ownerInstance;
        this.ownerInstanceStatus = ownerInstance.GetComponent<Status>();
    }

    public Status GetStatus()
    {
        return this.ownerInstanceStatus;
    }

    public void SetST(GameObject instance, float st)
    {
        if(this.ownerInstance != instance)
            return;
        
        Debug.Log("Status stamina changed, updating stamina status");
        
        SetStamina(st);
    }


    public IEnumerator refillStaminaBar(float delayTime, float refillTimeConstant)
    {
        yield return new WaitForSeconds(delayTime);
        float refillTime = refillTimeConstant - (slider.value/slider.maxValue * refillTimeConstant);
        
        while(GetStatus().stamina < GetMaxStamina())
        {
            GetStatus().stamina += 1;
            refillTween = slider.DOValue(GetStatus().stamina, refillTime/GetMaxStamina());
            redChunk.value += 1;
            yield return new WaitForSeconds(refillTime/GetMaxStamina());
        }
    }

    public IEnumerator redChunkDelayDecrease(float time)
    {
        yield return new WaitForSeconds(time);
        redChunk.DOValue(slider.value, .5f);
    }
}
