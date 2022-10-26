using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using DG.Tweening;
public class HealthOrb : MonoBehaviour
{
    public Powerup powerup;
    public float speed;
    public GameObject recievingEntity;

    public static event Action<GameObject, float> onPickupHealth;
    // Start is called before the first frame update
    void Start()
    {
        //Logic for testing
        //FlyToRecievingEntity();
    }


    private void OnTriggerEnter(Collider other)
    {
        if(recievingEntity == null)
        {
            Debug.Log("Recieving Entity is null");
            return;
        }

        if(other.gameObject == recievingEntity)
        {
            Debug.Log("health pickup collided with " + recievingEntity.name);
            onPickupHealth.Invoke(recievingEntity, powerup.healthIncrease);
            Destroy(this.gameObject);
        }
    }

    public void SetRecievingEntity(GameObject entity)
    {
        recievingEntity = entity;
    }

    public void FlyToRecievingEntity()
    {
        transform.LookAt(recievingEntity.transform);
        transform.DOMove(transform.position + transform.up*speed, 1.0f).OnComplete( () => {
            transform.DOMove(recievingEntity.transform.position, .5f);
        });
    }
}
