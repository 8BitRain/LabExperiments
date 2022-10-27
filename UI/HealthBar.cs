using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class HealthBar : MonoBehaviour
{

    public Slider slider;
    public Slider redChunk;

    private GameManager gameManager;

    private GameObject ownerInstance;

    private void OnEnable()
    {
        Status.onHealthStatusChange += SetHP;
    }

    private void OnDisable()
    {
        Status.onHealthStatusChange -= SetHP;
    }
    
    public void SetHealth(float health)
    {
        if(slider.value < health)
        {
            StartCoroutine(healthIncrease(.1f, health));
        }

        if(slider.value > health)
        {
            slider.value = health;
            if(redChunk != null)
            {
                StartCoroutine(redChunkDelayDecrease(1.5f));
            }
        }
    }

    public void SetMaxHealth(float health)
    {
        slider.maxValue = health;
        slider.value = health;

        if(redChunk != null)
        {
            redChunk.maxValue = health;
            redChunk.value = health;
        }
    }

    public float GetHealth()
    {
        return slider.value;
    }

    public void SetOwnerInstance(GameObject ownerInstance)
    {
        this.ownerInstance = ownerInstance;
    }

    public void SetHP(GameObject instance, float hp)
    {
        if(this.ownerInstance != instance)
            return;
        
        Debug.Log("Status health changed, updating health status");
        
        SetHealth(hp);
    }

    public IEnumerator redChunkDelayDecrease(float time)
    {
        yield return new WaitForSeconds(time);
        redChunk.DOValue(slider.value, .5f);
    }

    public IEnumerator healthIncrease(float time, float health)
    {
        yield return new WaitForSeconds(time);
        slider.DOValue(health, .5f);
    }


    //TODO: Remove reference. Relies on old Game Manager logic Update enemy health bar
    /*public void PositionEnemyHealthBar(Camera playerCam, Transform enemy, Transform player)
    {
        RectTransform healthBarRectTransform = this.GetComponent<RectTransform>();
        healthBarRectTransform.position = playerCam.WorldToScreenPoint(new Vector3(enemy.position.x,enemy.GetComponent<Bladeclubber>().Head.position.y,enemy.position.z) + new Vector3(0,3,0));
        //healthBarRectTransform.position = playerCam.WorldToScreenPoint(enemy.transform.position + new Vector3(0,8,0));
        //healthBarRectTransform.position = playerCam.WorldToScreenPoint(enemy.transform.position);

        Renderer enemy_renderer = enemy.GetComponent<Bladeclubber>().Body.GetComponent<Renderer>();
        if(enemy_renderer.isVisible)
        {
            Canvas healthBarCanvas = this.GetComponentInParent<Canvas>();
            healthBarCanvas.enabled = true;

        } else
        {
            Canvas healthBarCanvas = this.GetComponentInParent<Canvas>();
            healthBarCanvas.enabled = false;

        }
    }*/
}
