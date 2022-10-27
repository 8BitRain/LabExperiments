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

    public void DoFadeIn()
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
                    Debug.LogError("Material Controller attempted to fade in " + renderer.name + "  and failed >_<!");
                }
            }
            else
            {
                Debug.Log("No renderer attached to " + asset.name);
            }
        }
    }

    public void DoFadeInInstant()
    {
        foreach (GameObject asset in assets)
        {
            if(asset.TryGetComponent<Renderer>(out Renderer renderer))
            {
                try
                {
                    for (int i = 0; i < renderer.materials.Length; i++)
                    {
                        renderer.materials[i].DOFloat(1, "_OPACITY", 0f);    
                    }
                }
                catch (System.Exception)
                {
                    Debug.LogError("Material Controller attempted to fade in " + renderer.name + "  and failed >_<!");
                }
            }
            else
            {
                Debug.Log("No renderer attached to " + asset.name);
            }
        }
    }

    public void FadeOut()
    {
        foreach (GameObject asset in assets)
        {
            if(asset.TryGetComponent<Renderer>(out Renderer renderer))
            {
                try
                {
                    for (int i = 0; i < renderer.materials.Length; i++)
                    {
                        renderer.materials[i].DOFloat(0, "_OPACITY", 0f);    
                    }
                }
                catch (System.Exception)
                {
                    Debug.LogError("Material Controller attempted to fade out " + renderer.name + "  and failed >_<!");
                }
            }
            else
            {
                Debug.Log("No renderer attached to " + asset.name);
            }
        }
    }
}
