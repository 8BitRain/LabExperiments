using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Events;

public class HUDController : MonoBehaviour
{
    public Canvas HUD;
    private Canvas HUDInstance;
    
    private void OnEnable()
    {
        TargetingController.onEnableTargetArrow += ActivateTargetArrow;
        TargetingController.onDisableTargetArrow += DeactivateTargetArrow;
        TargetingController.onUpdateTargetArrowPosition += updateTargetPosition;
        CooldownController.onUpdateCooldownIcon += UpdateCooldownIcon;
        AbilityController.onActivateAbilityWindow += ToggleAbilityWindow;
    }

    private void OnDisable()
    {
        TargetingController.onEnableTargetArrow -= ActivateTargetArrow;
        TargetingController.onDisableTargetArrow -= DeactivateTargetArrow;
        TargetingController.onUpdateTargetArrowPosition -= updateTargetPosition;
        CooldownController.onUpdateCooldownIcon -= UpdateCooldownIcon;
        AbilityController.onActivateAbilityWindow -= ToggleAbilityWindow;
    }

    // Start is called before the first frame update
    void Start()
    {
        HUDInstance = Instantiate(HUD);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ActivateDutchedLetterBox()
    {
        HUDInstance.gameObject.GetComponent<HUD>().ActivateDutchedLetterBox();
    }

    public void DeactivateDutchedLetterBox()
    {
        HUDInstance.gameObject.GetComponent<HUD>().DeactivateDutchedLetterBox();
    }

    public void ActivateTargetArrow(GameObject instance)
    {
        if(this.gameObject != instance)
            return;
        
        HUDInstance.gameObject.GetComponent<HUD>().ActivateTargetArrow(HUDInstance.gameObject);
    }

    public void DeactivateTargetArrow(GameObject instance)
    {
        if(this.gameObject != instance)
            return;
        
        HUDInstance.GetComponent<HUD>().DeactivateTargetArrow(HUDInstance.gameObject);
    }

    public void updateTargetPosition(GameObject instance, GameObject target, Camera cam)
    {
        if(this.gameObject != instance)
            return;

       HUDInstance.GetComponent<HUD>().updateTargetPosition(HUDInstance.gameObject, target, cam);
        
    }

    public void UpdateCooldownIcon(GameObject instance, float cooldownTime, CooldownController.State cooldownState, Cooldown cooldown)
    {
        if(this.gameObject != instance)
        {
            print("This gameobject: " + this.gameObject.name + "This instance: " + instance.name);
            return;
        }
        
        
        print("Update Cooldown Icon: " + cooldownTime);
        switch (cooldownState)
        {
            case CooldownController.State.Initialize:
                HUDInstance.GetComponent<HUD>().InitializeCooldownBar(cooldownTime, CooldownController.State.Increment, cooldown.cooldownBarPosition);
                break;
            case CooldownController.State.Decrement:
                HUDInstance.GetComponent<HUD>().UpdateCooldownBar(cooldownTime, CooldownController.State.Decrement, cooldown.cooldownBarPosition);
                break;
            case CooldownController.State.Increment:
                HUDInstance.GetComponent<HUD>().UpdateCooldownBar(cooldownTime, CooldownController.State.Increment, cooldown.cooldownBarPosition);
                break;
            default:
                break;
        }
    }

    public void ToggleAbilityWindow(GameObject instance, bool toggleValue)
    {
       if(toggleValue)
       {
            HUDInstance.GetComponent<HUD>().AbilityWindow.GetComponent<CanvasGroup>().alpha = 1;
       }
       else
       {
            HUDInstance.GetComponent<HUD>().AbilityWindow.GetComponent<CanvasGroup>().alpha = 0;
       }
    }    
}
