﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class AnimationController : MonoBehaviour
{
    private string currentState;
    private Coroutine hitStopCoroutine;

    void Start()
    {
        
    }

    void OnDestroy()
    {
        StopAllCoroutines();
    }

    public void ChangeAnimationState(Animator animator, string newState, float normalizedTime = 0)
    {
        if(currentState == newState)
        {
            return;
        }
        
        if(normalizedTime == 0)
        {
            Debug.Log(this.gameObject.name + " playing animation: " + newState);
            animator.Play(newState);
        }
        else
        {
            Debug.Log(this.gameObject.name + " playing animation: " + newState + " starting at: " + normalizedTime);
            animator.Play(newState, -1, normalizedTime);
        }

        currentState = newState;
    }

    //Update the animaton state, but don't change the current animation. 
    //This is a helpful method for updating animation state in unison with animator.SetBool
    public void UpdateAnimationState(Animator animator, string newState)
    {
        if(currentState == newState)
            return;
        
        currentState = newState;
    }

    public void ResetAnimationState()
    {
        currentState = "resetAnimationState";
    }

    public string GetCurrentState()
    {
        return currentState;
    }

    public float GetCurrentAnimatorTime(Animator targetAnim, int layer = 0)
    {
        AnimatorStateInfo animState = targetAnim.GetCurrentAnimatorStateInfo(layer);
        float currentTime = animState.normalizedTime % 1;
        return currentTime;
    }

    public void PlayerAnimationLock(float duration, PlayerMovementController player, bool isMobileSkill)
    {
        try
        {
            StartCoroutine(PlayerAnimationLockCoroutine(duration, player, isMobileSkill));
        }
        catch (System.Exception e)
        {
            Debug.LogError("There was an error locking this objects animation >_<: " + e);
        }
    }

    public IEnumerator PlayerAnimationLockCoroutine(float duration, PlayerMovementController player, bool isMobileSkill)
    {
        yield return new WaitForSeconds(duration);

        try
        {
            if(isMobileSkill)
            {
                player.UnlockInputRightSideControllerFaceButtons(this.gameObject);
            }
            else
            {
                player.EnableMovement();
            } 
        }
        catch (System.Exception e)
        {
            Debug.Log("Exception: " + e);
        }
    }

    public void SetAnimatorWeight(Animator animator, int index, float value)
    {
        animator.SetLayerWeight(index, value);
    }


    public void HitStop(float duration, float stoppedAnimationTime, string animationStateBeforeReset)
    {
        this.hitStopCoroutine = StartCoroutine(HitStopDelay(duration, stoppedAnimationTime, animationStateBeforeReset));
    }

    private IEnumerator HitStopDelay(float duration, float stoppedAnimationTime, string animationStateBeforeReset)
    {
        Debug.Log("HitStopDelay: Running HitStop Delay Coroutine " + Time.time);
        Debug.Log("Wait Duration: " + duration);
        yield return new WaitForSecondsRealtime(duration);
        Debug.Log(this.name + " Stop Hitstop: " + Time.time);
        ResetAnimationState();
        ChangeAnimationState(this.GetComponent<Animator>(), animationStateBeforeReset, stoppedAnimationTime - .05f);
        this.GetComponent<Animator>().speed = 1;
    }

    public void CancelHitStop()
    {
        Debug.Log("CancelHitStop");

        //We are still attacking, let's make sure we stay in this state
        this.GetComponent<Animator>().SetBool("Attacking", true);
        //GoldLinkConfirmFlash();
        if(this.hitStopCoroutine != null)
        {
            StopCoroutine(this.hitStopCoroutine);
        }
        this.GetComponent<Animator>().speed = 1;
    }

    public void GoldLinkConfirmFlash()
    {
        foreach (Transform child in GetComponentsInChildren<Transform>())
        {
            if(child.TryGetComponent<Renderer>(out Renderer renderer))
            {
                foreach (var material in renderer.materials)
                {
                    //material.shader.
                    bool materialTextureNeedsReset = false;
                    try
                    {
                        if(material.HasProperty("USE_TEXTURE"))
                        {
                            if(material.GetFloat("USE_TEXTURE") == 1)
                            {
                                material.SetFloat("USE_TEXTURE", 0);
                                materialTextureNeedsReset = true;
                            }
                        }
                    }
                    catch (System.Exception e)
                    { 
                        Debug.Log("Caught error: " + e.Message);
                    }

                    if(material.HasProperty("Color_8E73BA40"))
                    {
                        Color ogColorA = material.GetColor("Color_8E73BA40");
                        Debug.Log("Color info: " + ogColorA.ToString());
                        material.DOColor(Color.yellow, "Color_8E73BA40", .2f).OnComplete(() => {
                            material.DOColor(ogColorA, "Color_8E73BA40", .2f);
                        }).SetLoops(0).OnComplete(() => {
                            if(materialTextureNeedsReset)
                            {
                                //material.DOColor(ogColorA, "Color_8E73BA40", .2f);
                                material.SetFloat("USE_TEXTURE", 1);
                            }
                        });
                    }

                    if(material.HasProperty("_Tint"))
                    {
                        Color ogColorB = material.GetColor("_Tint");
                        Debug.Log("Color info: " + ogColorB.ToString());
                        material.DOColor(Color.yellow, "_Tint", .2f).OnComplete(() => {
                            material.DOColor(ogColorB, "_Tint", .2f);
                        }).SetLoops(0);
                    }


                    /*Color ogColorB = material.GetColor("_Tint");
                    Debug.Log("Color info: " + ogColorB.ToString());
                    material.DOColor(Color.yellow, "_Tint", .2f).OnComplete(() => {
                        material.DOColor(ogColorB, "_Tint", .2f);
                    }).SetLoops(3);*/      
                }
            }
        }
    }
}
