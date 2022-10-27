using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine;
using DG.Tweening;

public class HUD : MonoBehaviour
{
    public HealthBar healthBar;
    public StaminaBar staminaBar;
    public GameObject TargetArrow;
    public UnityEvent onActivateTargetArrow;
    public UnityEvent onDeactivateTargetArrow;
    public GameObject AbilityWindow;
    public CooldownBar cooldownBarA;
    public CooldownBar cooldownBarB;
    public CooldownBar cooldownBarC;
    public CooldownBar cooldownBarD;

    public Canvas dutchedLetterBox;
    private Canvas dutchedLetterBoxInstance;

    private bool canUpdateTargetPosition = true;

    // Start is called before the first frame update
    void Start()
    {
        if(AbilityWindow != null)
        {
            AbilityWindow.GetComponent<CanvasGroup>().alpha = 0;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetupHealthBar(float maxHealth, GameObject ownerInstance)
    {
        healthBar.SetMaxHealth(maxHealth);
        healthBar.SetHealth(maxHealth);
        healthBar.SetOwnerInstance(ownerInstance);
        Debug.Log("Setting maxhealth and health to: " + maxHealth);
    }

    public void SetupStaminaBar(float maxStamina, bool startStaminaEmpty, GameObject ownerInstance)
    {
        if(!startStaminaEmpty)
        {
            staminaBar.SetMaxStamina(maxStamina);
            staminaBar.SetStamina(maxStamina);
            staminaBar.SetOwnerInstance(ownerInstance);
            Debug.Log("Setting maxStamina and stamina to: " + maxStamina);
        }
        else
        {
            staminaBar.SetMaxStamina(maxStamina);
            staminaBar.SetStamina(0);
            staminaBar.redChunk.value = 0;
            staminaBar.SetOwnerInstance(ownerInstance);
            Debug.Log("Setting maxStamina and stamina to: " + maxStamina);
        }
    }

    public void ActivateDutchedLetterBox()
    {
        dutchedLetterBoxInstance = Instantiate(dutchedLetterBox);
    }

    public void DeactivateDutchedLetterBox()
    {
        Destroy(dutchedLetterBoxInstance.gameObject);
    }

    public void ActivateTargetArrow(GameObject instance)
    {
        if(this.gameObject != instance)
            return;

        onActivateTargetArrow.Invoke();
        //TargetArrow.SetActive(true);
        Debug.Log("Enable Target Arrow");
    }

    public void DeactivateTargetArrow(GameObject instance)
    {
        if(this.gameObject != instance)
            return;

        onDeactivateTargetArrow.Invoke();
        //TargetArrow.SetActive(false);
        Debug.Log("Disable Target Arrow");
    }

    public void updateTargetPosition(GameObject instance, GameObject target, Camera cam)
    {
        if(this.gameObject != instance)
            return;
        
        if(canUpdateTargetPosition)
        {
            canUpdateTargetPosition = false;        
            TargetArrow.transform.position = cam.WorldToScreenPoint(target.GetComponent<Body>().TargetLock.transform.position);
            TargetArrow.transform.DOMove(cam.WorldToScreenPoint(target.GetComponent<Body>().TargetLock.transform.position), 0f).OnComplete(() => {
                canUpdateTargetPosition = true;
            });
        }
        Debug.Log("Updating Target arrow position from: " + TargetArrow.transform.position + " to: " + target.GetComponent<Body>().TargetLock.transform.position);
        
    }

    public void InitializeCooldownBar(float time, CooldownController.State cooldownState, Skill.CooldownBar cooldownBarPosition)
    {
        switch (cooldownState)
        {
            case CooldownController.State.Increment:
                if(cooldownBarPosition == Skill.CooldownBar.A)
                    cooldownBarA.SetMaxCooldown(time, 0);
                if(cooldownBarPosition == Skill.CooldownBar.B)
                    cooldownBarB.SetMaxCooldown(time, 0);
                if(cooldownBarPosition == Skill.CooldownBar.C)
                    cooldownBarC.SetMaxCooldown(time, 0);
                if(cooldownBarPosition == Skill.CooldownBar.D)
                    cooldownBarD.SetMaxCooldown(time, 0);
                
                break;
            case CooldownController.State.Decrement:
                if(cooldownBarPosition == Skill.CooldownBar.A)
                    cooldownBarA.SetMaxCooldown(time, time);
                if(cooldownBarPosition == Skill.CooldownBar.B)
                    cooldownBarB.SetMaxCooldown(time, time);
                if(cooldownBarPosition == Skill.CooldownBar.C)
                    cooldownBarC.SetMaxCooldown(time, time);
                if(cooldownBarPosition == Skill.CooldownBar.D)
                    cooldownBarD.SetMaxCooldown(time, time);
                break;
            default:
                break;
        }
    }

    public void UpdateCooldownBar(float time, CooldownController.State cooldownState, Skill.CooldownBar cooldownBarPosition)
    {
        switch (cooldownState)
        {
            case CooldownController.State.Increment:
                if(cooldownBarPosition == Skill.CooldownBar.A)
                {
                    time = cooldownBarA.GetMaxCooldown() - time;
                    cooldownBarA.SetCooldownBar(time);
                }
                if(cooldownBarPosition == Skill.CooldownBar.B)
                {
                    time = cooldownBarB.GetMaxCooldown() - time;
                    cooldownBarB.SetCooldownBar(time);
                }
                if(cooldownBarPosition == Skill.CooldownBar.C)
                {
                    time = cooldownBarC.GetMaxCooldown() - time;
                    cooldownBarC.SetCooldownBar(time);
                }
                if(cooldownBarPosition == Skill.CooldownBar.D)
                {
                    time = cooldownBarD.GetMaxCooldown() - time;
                    cooldownBarD.SetCooldownBar(time);
                }

                break;
            case CooldownController.State.Decrement:
                if(cooldownBarPosition == Skill.CooldownBar.A)
                    cooldownBarA.SetCooldownBar(time);
                if(cooldownBarPosition == Skill.CooldownBar.B)
                    cooldownBarB.SetCooldownBar(time);
                if(cooldownBarPosition == Skill.CooldownBar.C)
                    cooldownBarC.SetCooldownBar(time);
                if(cooldownBarPosition == Skill.CooldownBar.D)
                    cooldownBarD.SetCooldownBar(time);
                break;
            default:
                break;
        }
    }
}
