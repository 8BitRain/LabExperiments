using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    public GameObject healthPickupVFX;
    public AudioSource healthPickupSFX;
    private void OnEnable()
    {
        HealthOrb.onPickupHealth += PickupHealth;
    }

    private void OnDisable()
    {
        HealthOrb.onPickupHealth -= PickupHealth;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PickupHealth(GameObject instance, float health)
    {
        Status status = this.GetComponent<Status>();
        status.HPPickup(health);
        Transform spawnPos = this.GetComponent<Body>().Head;
        Vector3 spawnPos2 = transform.TransformPoint(this.GetComponent<BoxCollider>().center - new Vector3(0, 10, 0));
        //Instantiate(healthPickupVFX, spawnPos.position, spawnPos.rotation);
        Instantiate(healthPickupVFX, spawnPos2, Quaternion.identity);
        healthPickupSFX.Play();
    }
}
