using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Enemy : MonoBehaviour
{
    public void DeathFlash()
    {
        foreach (Transform child in GetComponentsInChildren<Transform>())
        {
            if(child.TryGetComponent<Renderer>(out Renderer renderer))
            {
                renderer.material.DOColor(Color.red, "_Tint", .2f).OnComplete(() => {
                    renderer.material.DOColor(Color.white, "_Tint", .2f);
                }).SetLoops(6);
            }
        }
    }

    public void OnDeath()
    {
        //Turn off hurtbox(s)
        HurtBox[] hurtBoxes = this.GetComponentsInChildren<HurtBox>();
        foreach (HurtBox hurtBox in hurtBoxes)
        {
            hurtBox.enabled = false;
        }

        DeathFlash();
        Destroy(this.gameObject, 5);
    }
}
