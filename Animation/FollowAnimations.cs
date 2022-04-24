using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowAnimations : MonoBehaviour
{
    private Animator animator;
    private AnimationController parentAnimationController;
    private AnimationController animationController;

    private string currentAnimation;

    // Start is called before the first frame update
    void Start()
    {
        animator = this.GetComponent<Animator>();
        parentAnimationController = this.transform.GetComponentInParent<AnimationController>();
        animationController = this.GetComponent<AnimationController>(); 
    }

    // Shadow parent animation
    // Note, this system may not work with bools
    void Update()
    {
        if(currentAnimation != parentAnimationController.GetCurrentState())
        {
            Debug.Log("currentAnimation");
            currentAnimation = parentAnimationController.GetCurrentState();
            animationController.ChangeAnimationState(animator, currentAnimation);
        }
    }
}
