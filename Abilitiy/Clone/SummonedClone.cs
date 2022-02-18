using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class SummonedClone : MonoBehaviour
{
    private Animator animator;
    private AnimationController animationController;
    
    private void OnEnable()
    {
        SummonClone.onSendAnimationData += PlayAnimation;
    }

    private void OnDisable()
    {
        SummonClone.onSendAnimationData -= PlayAnimation;
    }

    void Awake()
    {
        animator = GetComponent<Animator>();
        animationController = GetComponent<AnimationController>();

        foreach (SkinnedMeshRenderer renderer in GetComponentsInChildren<SkinnedMeshRenderer>())
        {
            for(int i = 0; i<renderer.materials.Length; i++)
            {
                renderer.materials[i].DOFloat(0, "Vector1_4808F300", 5f);
            }
        }

        Destroy(this.gameObject, 6f);
    }

    void Start()
    {

    }

    void PlayAnimation(GameObject instance, Animator sourceAnimator, AnimationController sourceAnimationController)
    {
        Debug.Log("Playing animation: " + sourceAnimationController.GetCurrentState());
        if(this.gameObject != instance)
            return;
        
        float sourceCurrentAnimationTime = sourceAnimationController.GetCurrentAnimatorTime(sourceAnimator);

        animator.speed = 0;
        animator.Play(sourceAnimationController.GetCurrentState(), 0, sourceCurrentAnimationTime);
    }
}
