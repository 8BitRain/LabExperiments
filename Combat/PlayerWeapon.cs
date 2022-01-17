using System.Collections;
using System.Collections.Generic;
using UnityEngine.VFX;
using UnityEngine;

public class PlayerWeapon : MonoBehaviour
{
    public Transform weaponTip;

    public GameObject targetHitVFX;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SpawnEnemyHitVFX(Transform target)
    {
        //string[] locationArr = location.Split(',');
        //print("SpawnEnemyVFX: location" + locationArr[0] + "," + location[1] + "," + location[2]);
        //print("SpawnEnemyVFX: rotation" + locationArr[3] + "," + location[4] + "," + location[5]);

        //locationVector = new Vector3(locationArr[0]., locationArr[1], locationArr[2]);
        Instantiate(targetHitVFX, target.position + Vector3.up * 5, weaponTip.rotation);
    }
}
