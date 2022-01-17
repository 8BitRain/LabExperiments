using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CooldownBar : MonoBehaviour
{

    public Slider slider;

    //private GameManager gameManager;
    //private Camera[] playerCameras;

    // Start is called before the first frame update
    public void SetCooldownBar(float cooldown)
    {
        slider.value = cooldown;
    }

    public void SetMaxCooldown(float cooldown, float initialValue)
    {
        slider.maxValue = cooldown;
        slider.value = initialValue;
    }

    public float GetMaxCooldown()
    {
        return slider.maxValue;
    }

    public float GetCooldown()
    {
        return slider.value;
    }
}
