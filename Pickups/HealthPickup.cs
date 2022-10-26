using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    public GameObject healthPickupVFX;
    public AudioClip healthPickupSFX;
    private void OnEnable()
    {
        HealthOrb.onPickupHealth += CollisionLogic;
    }

    private void OnDisable()
    {
        HurtBox.gotCollision -= CollisionLogic;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PickupHealth()
    {
        Instantiate(health)
    }
}
