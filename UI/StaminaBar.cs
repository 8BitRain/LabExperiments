using System.Collections;
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
        //Stop the coroutine that Stamina
        if(refillCoroutine != null)
        {
            StopCoroutine(refillCoroutine);
        }

        //End the Stamina Refill tween, embeded in refill courotine, that refills stamina slider.
        refillTween.Kill();

        if(ownerInstance != null)
        {
            refillCoroutine = StartCoroutine(refillStaminaBar(ownerInstance.GetComponent<Status>().staminaRefillTime));
        }
        
        slider.value = stamina;

        if(redChunk != null)
        {
            if(ownerInstance != null)
            {
                StartCoroutine(redChunkDelayDecrease(ownerInstance.GetComponent<Status>().staminaRefillTime - 0.25f));
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
    }

    public void SetST(GameObject instance, float st)
    {
        if(this.ownerInstance != instance)
            return;
        
        Debug.Log("Status stamina changed, updating stamina status");
        
        SetStamina(st);
    }


    public IEnumerator refillStaminaBar(float time)
    {
        yield return new WaitForSeconds(time);
        refillTween = slider.DOValue(slider.maxValue, .5f).OnComplete( () => {
            //TODO may need to adjust this logic if bar increase doesn't look right
            redChunk.value = redChunk.maxValue;
            if(ownerInstance != null)
            {
                Debug.Log("Refill Tween value: " + refillTween);
                ownerInstance.GetComponent<Status>().stamina = GetMaxStamina();
            }
        });

    }

    public IEnumerator redChunkDelayDecrease(float time)
    {
        yield return new WaitForSeconds(time);
        redChunk.DOValue(slider.value, .5f);
    }
}
