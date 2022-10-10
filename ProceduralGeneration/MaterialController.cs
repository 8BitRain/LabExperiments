using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MaterialController : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject[] assets;
    void Start()
    {
        DoFadeIn();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void DoFadeIn()
    {
        foreach (GameObject asset in assets)
        {
            if(asset.TryGetComponent<Renderer>(out Renderer renderer))
            {
                try
                {
                    for (int i = 0; i < renderer.materials.Length; i++)
                    {
                        renderer.materials[i].DOFloat(1, "_OPACITY", 1f);    
                    }
                }
                catch (System.Exception)
                {
                    Debug.LogError("Material Controller attempted to fade " + renderer.name + "  and failed >_<!");
                }
            }
            else
            {
                Debug.Log("No renderer attached to " + asset.name);
            }
        }
    }
}
