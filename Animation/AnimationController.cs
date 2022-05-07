using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController : MonoBehaviour
{
    private string currentState;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void ChangeAnimationState(Animator animator, string newState, float normalizedTime = 0)
    {
        if(currentState == newState)
        {
            return;
        }
        
        if(normalizedTime == 0)
        {
            animator.Play(newState);
        }
        else
        {
            Debug.Log("Playing animation: " + newState + " starting at: " + normalizedTime);
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
        StartCoroutine(PlayerAnimationLockCoroutine(duration, player, isMobileSkill));
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
                //player.DisableSteering();
            } 
        }
        catch (System.Exception e)
        {
            Debug.Log("Exception: " + e);
        }
    }
}
