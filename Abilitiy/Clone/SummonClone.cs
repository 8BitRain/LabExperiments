using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class SummonClone : MonoBehaviour
{
    public GameObject clone;

    public static event Action<GameObject, Animator, AnimationController> onSendAnimationData;

    public GameObject InstantiateClone(Transform transform)
    {
        GameObject instance = Instantiate(clone, transform.position, transform.rotation);
        return instance;
    }

    public void UpdateSummonAnimation(GameObject instance, Animator sourceAnimator, AnimationController sourceAnimationController)
    {
        Debug.Log("GameObject instance: " + instance);
        Debug.Log("Source Animator: " + sourceAnimator);
        Debug.Log("Source Animation Controller: " + sourceAnimationController);
        onSendAnimationData?.Invoke(instance, sourceAnimator, sourceAnimationController);
    }
}
