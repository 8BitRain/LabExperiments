using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using System;
using UnityEngine;

public class CameraGroup : MonoBehaviour
{
    public enum FrameShotStyle
    {
        NONE,
        WIDESHOT,
        ZoomToFace
    }

    public GameObject lockOnCamera;
    public GameObject thirdPersonCamera;
    public GameObject zoomToFaceCamera;

    public static event Action<GameObject> onShowLetterBox;
    public static event Action<GameObject> onHideLetterBox;

    private void OnEnable()
    {
        CameraController.onEnableThirdPersonCamera += EnableThirdPersonCamera;
        CameraController.onEnableThirdPersonCameraRetargeting += RecenterCamera;
    }

    private void OnDisable()
    {
        CameraController.onEnableThirdPersonCamera -= EnableThirdPersonCamera;
        CameraController.onEnableThirdPersonCameraRetargeting -= RecenterCamera;
    }

    // Start is called before the first frame update
    void Start()
    {
        lockOnCamera.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void RouteToCameraEngage(GameObject instance, GameObject looker, Transform target, CameraSettings cameraSettings)
    {
        if(this.gameObject != instance)
            return;

        switch (cameraSettings.frameShotStyle)
        {
            case FrameShotStyle.NONE:
                EnableLockOnCamera(instance, looker, target, cameraSettings);
                break;
            case FrameShotStyle.WIDESHOT:
                EnableLockOnCamera(instance, looker, target, cameraSettings);
                break;
            case FrameShotStyle.ZoomToFace:
                EnableZoomToFaceCamera(instance, looker, target, cameraSettings);
                break;
            default:
                break;
        }
    }

    public void EnableZoomToFaceCamera(GameObject instance, GameObject looker, Transform target, CameraSettings cameraSettings)
    {
        //If method is triggered from an event, rememeber to check instance
        /*
        if(this.gameObject != instance)
            return;
        */

        zoomToFaceCamera.SetActive(true);
        zoomToFaceCamera.GetComponent<CinemachineFreeLook>().m_Priority = 12;
        thirdPersonCamera.GetComponent<CinemachineFreeLook>().m_Priority = 10;
        lockOnCamera.GetComponent<CinemachineVirtualCamera>().m_Priority = 10;

        zoomToFaceCamera.GetComponent<CinemachineFreeLook>().m_Follow = looker.GetComponent<Body>().Head;
        zoomToFaceCamera.GetComponent<CinemachineFreeLook>().m_LookAt = looker.GetComponent<Body>().Head;

        if(cameraSettings != null)
        {
            Camera.main.GetComponent<CinemachineBrain>().m_DefaultBlend.m_Time = cameraSettings.cameraBlendTime;
            CinemachineFreeLook freeLook = zoomToFaceCamera.GetComponent<CinemachineFreeLook>();
            freeLook.m_Heading.m_Bias = cameraSettings.cameraBias;
            freeLook.GetComponent<CinemachineCameraOffset>().m_Offset = cameraSettings.cameraOffset;

            if(cameraSettings.freezeFrame)
            {
                StartCoroutine(AdjustTimeScaleAfterDelay(cameraSettings));
            }
        }
    }

    public void EnableThirdPersonCamera(GameObject instance, Transform target)
    {
        if(this.gameObject != instance)
            return;
        thirdPersonCamera.GetComponent<CinemachineFreeLook>().m_Follow = target;
        thirdPersonCamera.GetComponent<CinemachineFreeLook>().m_LookAt = target;
    }

    public void EnableLockOnCamera(GameObject instance, GameObject looker, Transform target, CameraSettings cameraSettings)
    {
        if(this.gameObject != instance)
            return;

        lockOnCamera.SetActive(true);
        lockOnCamera.GetComponent<CinemachineVirtualCamera>().m_Priority = 12;
        zoomToFaceCamera.GetComponent<CinemachineFreeLook>().m_Priority = 10;
        thirdPersonCamera.GetComponent<CinemachineFreeLook>().m_Priority = 10;

        //lockOnCamera.GetComponent<CinemachineVirtualCamera>().m_Follow = looker.GetComponent<CameraController>().thirdPersonCameraTargetBack;
        if(cameraSettings != null)
        {
            //Quick test of zooming to face
            /*if(cameraSettings.frameShotStyle == FrameShotStyle.ZoomToFace)
            {
                DisableLockOnCamera(looker);
                ZoomToTargetFace(cameraSettings);
                return;
            }*/
            DefineGameObjectToFollow(looker, target, cameraSettings.frameShotStyle);
            lockOnCamera.GetComponent<CinemachineCameraOffset>().m_Offset = cameraSettings.cameraOffset;
            lockOnCamera.GetComponent<CinemachineVirtualCamera>().m_Lens.Dutch = cameraSettings.dutch;
            Camera.main.GetComponent<CinemachineBrain>().m_DefaultBlend.m_Time = cameraSettings.cameraBlendTime;
        }
        else
        {
            DefineGameObjectToFollow(looker, target, CameraGroup.FrameShotStyle.NONE);
        }

       
        CinemachineTargetGroup targetGroup = lockOnCamera.GetComponentInChildren<CinemachineTargetGroup>();
        targetGroup.AddMember(looker.transform, 1, 2);

        if(target.GetComponent<Body>() != null)
        {
            Debug.Log("Enabling Target to look at Head");
            targetGroup.AddMember(target.GetComponent<Body>().Head, 1.5f, 4);
        }
        else
        {
            Debug.Log("Enabling Target to look at Body");
            targetGroup.AddMember(target.transform, 1.5f, 4);
        }
    }

    public void RouteToCameraDisengage(GameObject instance, CameraSettings cameraSettings)
    {
        if(this.gameObject != instance)
            return;

        switch (cameraSettings.frameShotStyle)
        {
            case FrameShotStyle.NONE:
                DisableLockOnCamera(instance);
                break;
            case FrameShotStyle.WIDESHOT:
                DisableLockOnCamera(instance);
                break;
            case FrameShotStyle.ZoomToFace:
                DisableZoomToFaceCamera(instance);
                break;
            default:
                break;
        }
    }

    public void DisableLockOnCamera(GameObject instance)
    {
        if(this.gameObject != instance)
            return;

        CinemachineTargetGroup targetGroup = lockOnCamera.GetComponentInChildren<CinemachineTargetGroup>();
        foreach (var targetElement in targetGroup.m_Targets)
        {
            print("DisengageDynamicTargetLock: " + "Removing: " + targetElement.target.name);
            targetGroup.RemoveMember(targetElement.target.transform);
        }

        lockOnCamera.GetComponent<CinemachineVirtualCamera>().m_Priority = 10;
        thirdPersonCamera.GetComponent<CinemachineFreeLook>().m_Priority = 11;

    }

    public void DisableZoomToFaceCamera(GameObject instance)
    {
        //If method is triggered from an event, rememeber to check instance
        /*
        if(this.gameObject != instance)
            return;
        */

        zoomToFaceCamera.GetComponent<CinemachineFreeLook>().m_Priority = 10;
        thirdPersonCamera.GetComponent<CinemachineFreeLook>().m_Priority = 11;
    }

    public void UpdateDynamicTargetLock(GameObject instance, Transform target)
    {
        if(this.gameObject != instance)
            return;
        CinemachineTargetGroup targetGroup = lockOnCamera.GetComponentInChildren<CinemachineTargetGroup>();
        
        //Update target at index 1 to the new target
        if(target.GetComponent<Body>() != null)
        {
            Debug.Log("Enabling Target to look at Head");
            targetGroup.m_Targets[1].target = target.GetComponent<Body>().Head;
        }
        else
        {
            Debug.Log("Enabling Target to look at Body");
            targetGroup.m_Targets[1].target = target.transform;
        }
    }

    public void RecenterCamera(GameObject instance, float time)
    {
        if(this.gameObject != instance)
            return;
        
        thirdPersonCamera.GetComponent<CinemachineFreeLook>().m_RecenterToTargetHeading.m_RecenteringTime = time;
        thirdPersonCamera.GetComponent<CinemachineFreeLook>().m_RecenterToTargetHeading.m_enabled = true;
        StartCoroutine(Delay(time*2));
    }

    public IEnumerator Delay(float time)
    {
        yield return new WaitForSeconds(time);
        thirdPersonCamera.GetComponent<CinemachineFreeLook>().m_RecenterToTargetHeading.m_enabled = false;
    }

    void DefineGameObjectToFollow(GameObject source, Transform target, FrameShotStyle frameShotStyle)
    {
        switch (frameShotStyle)
        {
            case FrameShotStyle.NONE:
                lockOnCamera.GetComponent<CinemachineVirtualCamera>().m_Follow = source.GetComponent<CameraController>().thirdPersonCameraTargetBack;
                break;
            case FrameShotStyle.WIDESHOT:
                lockOnCamera.GetComponent<CinemachineVirtualCamera>().m_Follow = target;
                break;
            case FrameShotStyle.ZoomToFace:
                break;
            default:
                break;
        }
    }

    /*Time Scale Adjustement*/
    public IEnumerator AdjustTimeScaleAfterDelay(CameraSettings cameraSettings)
    {
        yield return new WaitForSeconds(cameraSettings.freezeFrameDelay);
        onShowLetterBox.Invoke(this.gameObject);
        AdjustTimeScale(cameraSettings.freezeFrameScale);
        StartCoroutine(SetTimeScaleToDefault(cameraSettings.freezeDuration));
    }

    public IEnumerator SetTimeScaleToDefault(float time)
    {
        yield return new WaitForSecondsRealtime(time);
        AdjustTimeScale(1);
        onHideLetterBox.Invoke(this.gameObject);
    }

    public void AdjustTimeScale(float time)
    {
        Time.timeScale = time;
    }
}
