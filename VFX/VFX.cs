using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class VFX : MonoBehaviour
{
    public VFXController.VFX VFXType;
    private ParticleSystem ps;
    private VisualEffect visualEffect;
    // Start is called before the first frame update
    void Start()
    {
        if(GetComponent<ParticleSystem>() != null)
        {
            ps = GetComponent<ParticleSystem>();
            ps.Play();
        }
        else if (GetComponentInChildren<ParticleSystem>() != null)
        {
            ps = GetComponentInChildren<ParticleSystem>();
            ps.Play();
        }
        

        if(GetComponent<VisualEffect>() != null)
        {
            visualEffect = GetComponent<VisualEffect>();
            Destroy(this.gameObject, 1f);
        }
        else if(GetComponentInChildren<VisualEffect>() != null)
        {
            visualEffect = GetComponentInChildren<VisualEffect>();
            Destroy(this.gameObject, 1f);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(ps != null && ps.particleCount == 0)
        {
            Destroy(this.gameObject);
        }
    }
}
