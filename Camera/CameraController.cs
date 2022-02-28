using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using Cinemachine;
using System;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Camera Camera;
    private Camera cameraInstance;
    public GameObject cameraGroup;
    private GameObject cameraGroupInstance;

    public Transform thirdPersonCameraTargetShoulder;
    public Transform thirdPersonCameraTargetBack;

    [Header("Target Lock Camera Settings")]
    public CinemachineVirtualCamera dynamicTargetLockCam;
    public bool adjustCameraDistance; 
    public bool adjustCameraDamping = false;
    public float maxCamDistance = 10;
    public float minCamDistance = 3;
    public float maxTargetDistance = 15;

    [Header("Combat Camera(s) Settings")]
    public CinemachineVirtualCamera combatCamKick;
    public CinemachineSmoothPath combatCamKickDolly;

    //Events
    public static event Action<GameObject, Transform> onEnableThirdPersonCamera;
    public static event Action<GameObject, float> onEnableThirdPersonCameraRetargeting;
    public static event Action<Camera> onCameraLoaded;
    //public static event Action<GameObject, Transform> onEnableLockOnCamera;

    private void OnEnable()
    {
        TargetingController.onEnableLockOnCamera += EngageDynamicTargetLock;
        TargetingController.onDisableLockOnCamera += DisengageDynamicTargetLock;
        TargetingController.onUpdateLockOnCameraTarget += UpdateDynamicTargetLock;
    }

    private void OnDisable()
    {
        TargetingController.onEnableLockOnCamera -= EngageDynamicTargetLock;
        TargetingController.onDisableLockOnCamera -= DisengageDynamicTargetLock;
        TargetingController.onUpdateLockOnCameraTarget -= UpdateDynamicTargetLock;
    }


    // Start is called before the first frame update
    void Start()
    {
        cameraGroupInstance = Instantiate(cameraGroup);
        onEnableThirdPersonCamera.Invoke(cameraGroupInstance, thirdPersonCameraTargetShoulder);

        GameObject mainCameraExists = GameObject.FindGameObjectWithTag("MainCamera");
        if(mainCameraExists != null)
            mainCameraExists.SetActive(false);

        cameraInstance = Instantiate(Camera);
        cameraInstance.tag = "MainCamera";

        Debug.Log(cameraInstance);
        onCameraLoaded?.Invoke(cameraInstance);
    }

    // Update is called once per frame
    void Update()
    {
        //UpdateDynamicTargetLockCam();
    }

    public void EngageDynamicTargetLock(GameObject instance, Transform target, CameraSettings cameraSettings)
    {
        if(this.gameObject != instance)
            return;

        GameObject looker = instance;
        cameraGroupInstance.GetComponent<CameraGroup>().EnableLockOnCamera(cameraGroupInstance, looker, target, cameraSettings);
    }

    public void DisengageDynamicTargetLock(GameObject instance)
    {
        if(this.gameObject != instance)
            return;

        cameraGroupInstance.GetComponent<CameraGroup>().DisableLockOnCamera(cameraGroupInstance);
    }

    public void UpdateDynamicTargetLock(GameObject instance, Transform target)
    {
        if(this.gameObject != instance)
            return;
        
        cameraGroupInstance.GetComponent<CameraGroup>().UpdateDynamicTargetLock(cameraGroupInstance, target);

    }

    public void RecenterThirdPersonCam(float time)
    {
        onEnableThirdPersonCameraRetargeting.Invoke(cameraGroupInstance, time);
    }

    void UpdateDynamicTargetLockCam()
    {
        /*if(dynamicTargetLockCam.m_Priority == 12)
        { 
            if(adjustCameraDistance)
            {
                UpdateDynamicTargetLockCamDistance();
            }
       }*/
    
    }

    void UpdateDynamicTargetLockCamDistance()
    {
        //Equation. Player position is t. goes from 10 (maximum distance) to 3 minimum distance
        //TODO: Update Reference 
        //float distance = _playerScriptRef.GetDistanceToTarget();
        /*print("DynamicTargetLockCam: targetDistance is: " + distance);

        if(distance > maxTargetDistance)
        {
           distance = maxTargetDistance;
        }


        print("DynamicTargetLockCam: distance/maxTargetDistance is: " + distance/maxTargetDistance);
        float camDistance = Mathf.Lerp(minCamDistance, maxCamDistance, distance/maxTargetDistance);
        print("CAMDISTANCE_LERPVALUE: is: " + camDistance);

        //Update Horizontal And Vertical Damping
        //Get Player distance to target
        dynamicTargetLockCam.GetCinemachineComponent<CinemachineFramingTransposer>().m_CameraDistance = camDistance;


        //Update Target camera damping. The closer we get to target, the longer we want the camera to take to readjust.
        float[] calculatedDampingValues = TargetDampingCalculation(12, 7, distance/maxTargetDistance);
        AdjustDynamicTargetLockCamAimDamping(calculatedDampingValues[0], calculatedDampingValues[1]);*/


    }

    public float[] TargetDampingCalculation(float maxVerticalDamping, float maxHorizontalDamping, float camDistance)
    {
        float[] dampingValues = new float[2];

        float verticalDamping = Mathf.Lerp(maxVerticalDamping, .2f, camDistance);
        float horizontalDamping = Mathf.Lerp(maxHorizontalDamping, .2f, camDistance);

        dampingValues[0] = verticalDamping;
        dampingValues[1] = horizontalDamping;

        return dampingValues;
    }

    public void AdjustDynamicTargetLockCamAimDamping(float verticalDamping, float horizontalDamping)
    {
        if(!adjustCameraDamping)
            return;

        dynamicTargetLockCam.GetCinemachineComponent<CinemachineGroupComposer>().m_VerticalDamping = verticalDamping;
        dynamicTargetLockCam.GetCinemachineComponent<CinemachineGroupComposer>().m_HorizontalDamping = horizontalDamping;
    }

    public void ResetDynamicTargetLockAimDamping()
    {
        dynamicTargetLockCam.GetCinemachineComponent<CinemachineGroupComposer>().m_VerticalDamping = 1;
        dynamicTargetLockCam.GetCinemachineComponent<CinemachineGroupComposer>().m_HorizontalDamping = 1;
    }

    public void EnableCinematicKickCam()
    {
        //TODO: Replace playerScriptReference fields
        /*_playerScriptRef.freeLookCamera.GetComponent<CinemachineFreeLook>().m_Priority = 10;
        if(_playerScriptRef.GetCurrentTarget() != null)
        {
            dynamicTargetLockCam.m_Priority = 10;
        }*/

        combatCamKick.m_Priority = 12;

        //Adjust Dolly. 
        //First Waypoint position should be to the right of the player
        Vector3 playerPos = this.transform.position;
        Vector3 playerLegPos = combatCamKick.m_LookAt.position;
        combatCamKickDolly.m_Waypoints[0].position = new Vector3(playerLegPos.x + 3, playerLegPos.y, playerLegPos.z);
        combatCamKickDolly.m_Waypoints[1].position = new Vector3(playerLegPos.x + 3 + 3, playerLegPos.y, playerLegPos.z + 5);


    }

    public void DisableCinematicKickCam()
    {
        //_playerScriptRef.freeLookCamera.GetComponent<CinemachineFreeLook>().m_Priority = 12;
        dynamicTargetLockCam.m_Priority = 10;
        combatCamKick.m_Priority = 10;
    }

    public void resetCams()
    {
        print("Reset Cams");
        combatCamKick.m_Priority = 10;
        /*if(_playerScriptRef.GetCurrentTarget() != null)
        {
            dynamicTargetLockCam.m_Priority = 12;
            _playerScriptRef.freeLookCamera.GetComponent<CinemachineFreeLook>().m_Priority = 10;
        } 
        else*/
        //{
            dynamicTargetLockCam.m_Priority = 10;
            //_playerScriptRef.freeLookCamera.GetComponent<CinemachineFreeLook>().m_Priority = 12;
        //}
    }

    public Transform GetCameraInstance()
    {
        if (cameraInstance.transform == null)
            return GameObject.FindGameObjectWithTag("MainCamera").transform;
        
        return cameraInstance.transform;
    }
}
