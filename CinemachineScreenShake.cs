using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CinemachineScreenShake : MonoBehaviour
{
    private CinemachineVirtualCamera cinemachineVirtualCamera;
    private CinemachineFreeLook cinemachineFreeLookCam;
    private float defaultFOV;

    public void Start()
    {
        //Attempt to grab thirdperson camera reference. If not available, grab Cinemachine Virtual Camera reference
        if(TryGetComponent<CinemachineFreeLook>(out CinemachineFreeLook cinemachineFreeLookCam))
        {
            this.cinemachineFreeLookCam = cinemachineFreeLookCam;
            defaultFOV = cinemachineFreeLookCam.m_Lens.FieldOfView;
        }
        else if(TryGetComponent<CinemachineVirtualCamera>(out CinemachineVirtualCamera cinemachineVirtualCamera))
        {
            this.cinemachineVirtualCamera = cinemachineVirtualCamera;
            defaultFOV= cinemachineVirtualCamera.m_Lens.FieldOfView;
        }
    }

    void Noise(float amplitude, float frequency)
    {
        if(cinemachineFreeLookCam != null)
        {
            cinemachineFreeLookCam.GetRig(0).GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = amplitude;
            cinemachineFreeLookCam.GetRig(0).GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_FrequencyGain = frequency;
            cinemachineFreeLookCam.GetRig(1).GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = amplitude;
            cinemachineFreeLookCam.GetRig(1).GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_FrequencyGain = frequency;
            cinemachineFreeLookCam.GetRig(2).GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = amplitude;
            cinemachineFreeLookCam.GetRig(2).GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_FrequencyGain = frequency;
        }
        else if( cinemachineVirtualCamera != null)
        {
            cinemachineVirtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = amplitude;
            cinemachineVirtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_FrequencyGain = frequency;
        }
    }

    void SetZoom(float zoom)
    {
        if(cinemachineFreeLookCam != null)
        {
            cinemachineFreeLookCam.m_Lens.FieldOfView = zoom;
        } 
        else if (cinemachineVirtualCamera != null)
        {
            cinemachineVirtualCamera.m_Lens.FieldOfView = zoom;
        }
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
