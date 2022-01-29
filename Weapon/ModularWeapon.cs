using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ModularWeapon : MonoBehaviour
{
    public WeaponComponent weaponComponent;
    // Start is called before the first frame update
    void Start()
    {
        transform.DOLocalMove((Vector3.up) * weaponComponent.floatIntensity , weaponComponent.floatDuration).SetEase(Ease.OutSine).SetLoops(-1, LoopType.Yoyo);

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
