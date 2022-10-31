using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableRandomVFX : MonoBehaviour
{
    public GameObject[] VFX;
    // Start is called before the first frame update
    void Start()
    {
        int vfxChoice = Random.Range(0, VFX.Length);
        VFX[vfxChoice].SetActive(true);
    }
}
