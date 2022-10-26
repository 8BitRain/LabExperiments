using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using DG.Tweening;
public class HealthOrb : MonoBehaviour
{
    public Powerup powerup;
    public float speed;
    private GameObject recievingEntity;

    public event Action<GameObject, float> onPickupHealth;
    // Start is called before the first frame update
    void Start()
    {
        
    }


    private void OnTriggerEnter(Collider other)
    {
        if(recievingEntity == null)
        {
            return;
        }

        if(other.gameObject == recievingEntity)
        {
            onPickupHealth.Invoke(recievingEntity, powerup.healthIncrease);
        }
    }

    public void SetRecievingEntity(GameObject entity)
    {
        recievingEntity = entity;
    }

    public void FlyToRecievingEntity()
    {
        transform.LookAt(recievingEntity.transform);
        transform.DOMove(transform.position + transform.forward*speed, 1.0f);
    }
}
