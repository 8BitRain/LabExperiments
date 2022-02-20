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
    }
    public GameObject lockOnCamera;
    public GameObject thirdPersonCamera;

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
        thirdPersonCamera.GetComponent<CinemachineFreeLook>().m_Priority = 10;

        //lockOnCamera.GetComponent<CinemachineVirtualCamera>().m_Follow = looker.GetComponent<CameraController>().thirdPersonCameraTargetBack;
        if(cameraSettings != null)
        {
            DefineGameObjectToFollow(looker, target, cameraSettings.frameShotStyle);
            lockOnCamera.GetComponent<CinemachineCameraOffset>().m_Offset = cameraSettings.cameraOffset;
            lockOnCamera.GetComponent<CinemachineVirtualCamera>().m_Lens.Dutch = cameraSettings.dutch;
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
            default:
                break;
        }
    }
}
