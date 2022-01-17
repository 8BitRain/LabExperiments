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
}
