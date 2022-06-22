using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CinemachineScreenShake : MonoBehaviour
{
    public CinemachineFreeLook cinemachineCam;
    private float defaultFOV;

    public void Start()
    {
        defaultFOV = cinemachineCam.m_Lens.FieldOfView;
    }

    void Noise(float amplitude, float frequency)
    {
        cinemachineCam.GetRig(0).GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = amplitude;
        cinemachineCam.GetRig(0).GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_FrequencyGain = frequency;
        cinemachineCam.GetRig(1).GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = amplitude;
        cinemachineCam.GetRig(1).GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_FrequencyGain = frequency;
        cinemachineCam.GetRig(2).GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = amplitude;
        cinemachineCam.GetRig(2).GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_FrequencyGain = frequency;
    }

    void SetZoom(float zoom)
    {
        cinemachineCam.m_Lens.FieldOfView = zoom;
    }

    public void DoShake(ScreenShakeComponent screenShakeComponent)
    {
        Debug.Log("Starting Screenshake Camera");
        StartCoroutine(Shake(screenShakeComponent));
    }

    public IEnumerator FreezeScreen(ScreenShakeComponent screenShakeComponent)
    {
        Time.timeScale = screenShakeComponent.freezeScreenTimeScale;
        yield return new WaitForSecondsRealtime(screenShakeComponent.realtimeDelay);
        Time.timeScale = 1;
    }


    public IEnumerator Shake(ScreenShakeComponent screenShakeComponent)
    {
        StartCoroutine(FreezeScreen(screenShakeComponent));
        if(screenShakeComponent.useZoom) SetZoom(screenShakeComponent.zoom);
        Noise(screenShakeComponent.amplitude, screenShakeComponent.frequency);
        yield return new WaitForSeconds(screenShakeComponent.time);
        SetZoom(defaultFOV);
        Noise(0,0);
    }
}
