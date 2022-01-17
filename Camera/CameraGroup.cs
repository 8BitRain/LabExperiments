using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using System;
using UnityEngine;

public class CameraGroup : MonoBehaviour
{
    public GameObject lockOnCamera;
    public GameObject thirdPersonCamera;

    private void OnEnable()
    {
        CameraController.onEnableThirdPersonCamera += EnableThirdPersonCamera;
    }

    private void OnDisable()
    {
        CameraController.onEnableThirdPersonCamera -= EnableThirdPersonCamera;
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

    public void EnableLockOnCamera(GameObject instance, GameObject looker, Transform target)
    {
        if(this.gameObject != instance)
            return;

        lockOnCamera.SetActive(true);
        lockOnCamera.GetComponent<CinemachineVirtualCamera>().m_Priority = 12;
        thirdPersonCamera.GetComponent<CinemachineFreeLook>().m_Priority = 10;

        lockOnCamera.GetComponent<CinemachineVirtualCamera>().m_Follow = looker.GetComponent<CameraController>().thirdPersonCameraTargetBack;

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
}
