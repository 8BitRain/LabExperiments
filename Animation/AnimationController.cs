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

    public void ChangeAnimationState(Animator animator, string newState)
    {
        if(currentState == newState)
        {
            return;
        }
        
        animator.Play(newState);

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

    public string GetCurrentState()
    {
        return currentState;
    }

    public void PlayerAnimationLock(float duration, PlayerMovementController player, bool isMobileSkill)
    {
        StartCoroutine(PlayerAnimationLockCoroutine(duration, player, isMobileSkill));
    }

    public IEnumerator PlayerAnimationLockCoroutine(float duration, PlayerMovementController player, bool isMobileSkill)
    {
        yield return new WaitForSeconds(duration);
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
}
