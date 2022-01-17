using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class FaceCamera : MonoBehaviour
{
    public GameObject Character; 
    public GameObject TrackingObject;
    public Camera mainCamera;
    private RectTransform rectTransform;
    Vector2 pos;

    private void OnEnable()
    {
        Adversary.onUpdateDistanceUI += UpdateDistanceUI;
        Adversary.onUpdateCurrentStateUI += UpdateCurrentStateUI;
        Adversary.onUpdateBlockTimerUI += UpdateBlockTimerUI;
        Adversary.onUpdateActionWindowUI += UpdateActionWindowUI;
    }

    private void OnDisable()
    {
        Adversary.onUpdateDistanceUI -= UpdateDistanceUI;
        Adversary.onUpdateCurrentStateUI -= UpdateCurrentStateUI;
        Adversary.onUpdateBlockTimerUI -= UpdateBlockTimerUI;
        Adversary.onUpdateActionWindowUI -= UpdateActionWindowUI;
    }

    // Start is called before the first frame update
    void Start()
    {
        if(mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        rectTransform = this.GetComponent<RectTransform>();

        
    }

    // Update is called once per frame
    void Update()
    {
    
        if (TrackingObject)
        {
    
            
            Vector3 targPos = TrackingObject.transform.position;
            Vector3 camForward = mainCamera.transform.forward;
            Vector3 camPos = mainCamera.transform.position + camForward;
            float distInFrontOfCamera = Vector3.Dot(targPos - camPos, camForward);
            if (distInFrontOfCamera < 0f)
            {
                targPos -= camForward * distInFrontOfCamera;
            }
            pos = RectTransformUtility.WorldToScreenPoint (mainCamera, targPos);
            rectTransform.position = pos;
        }
        else
        {
            Debug.LogError (this.gameObject.name + ": No Object Attached (TrackObject)");
        }

    }

    void UpdateDistanceUI(GameObject instance, string text, Camera camera)
    {
        if(Character != instance)
            return;

        //TODO: This has the potential to cause a nasty bug for splitscreen.
        //You would effectively need two UI Texts generated. One for each player.
        //Consider the case where you update the main camera to a new target's camera
        this.mainCamera = camera;
    }

    void UpdateCurrentStateUI(GameObject instance, string text, Camera camera)
    {
        if(Character != instance)
            return;

        //TODO: This has the potential to cause a nasty bug for splitscreen.
        //You would effectively need two UI Texts generated. One for each player.
        //Consider the case where you update the main camera to a new target's camera
        this.mainCamera = camera;
    }

    void UpdateBlockTimerUI(GameObject instance, string text, Camera camera)
    {
        if(Character != instance)
            return;

        //TODO: This has the potential to cause a nasty bug for splitscreen.
        //You would effectively need two UI Texts generated. One for each player.
        //Consider the case where you update the main camera to a new target's camera
        this.mainCamera = camera;
    }

    void UpdateActionWindowUI(GameObject instance, string text, Camera camera)
    {
        if(Character != instance)
            return;

        //TODO: This has the potential to cause a nasty bug for splitscreen.
        //You would effectively need two UI Texts generated. One for each player.
        //Consider the case where you update the main camera to a new target's camera
        this.mainCamera = camera;
    }
}
