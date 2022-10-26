using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loot : MonoBehaviour
{
    // Start is called before the first frame update
    public HealthOrb healthOrb;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SpawnHealthOrb(Vector3 position)
    {
        Instantiate(healthOrb, position, Quaternion.identity);
    }
}
