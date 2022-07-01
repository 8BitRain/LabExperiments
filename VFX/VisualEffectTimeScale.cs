using System;
using UnityEngine;
using UnityEngine.VFX;

//https://realtimevfx.com/t/is-there-a-way-to-speed-up-slow-down-an-entire-vfx-graph-system-as-a-whole/10711/10
[RequireComponent(typeof(VisualEffect))]
[ExecuteInEditMode]
public class VisualEffectTimeScale : MonoBehaviour{
    [Range(0.0f, 10000.0f)] public float SimulationTimeScale = 1.0f;

    private VisualEffect Graph;

    private void OnValidate()
    {
        Graph = gameObject.GetComponent<VisualEffect>();
    }

    private void Start()
    {
        Graph = gameObject.GetComponent<VisualEffect>();
    }

    private void Update()
    {
        Graph.playRate = SimulationTimeScale;
    }
}