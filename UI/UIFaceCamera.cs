using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIFaceCamera : MonoBehaviour
{
    public GameObject TrackingObject;
    public Camera mainCamera;
    private RectTransform rectTransform;
    Vector2 pos;
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
            //pos = RectTransformUtility.WorldToScreenPoint (mainCamera, TrackingObject.transform.position);
            rectTransform.position = pos;
        }
        else
        {
            Debug.LogError (this.gameObject.name + ": No Object Attached (TrackObject)");
        }

        /*Vector3 targPos = TrackingObject.transform.position;
        Vector3 camForward = mainCamera.transform.forward;
        Vector3 camPos = mainCamera.transform.position + camForward;
        float distInFrontOfCamera = Vector3.Dot(targPos - camPos, camForward);
        if (distInFrontOfCamera < 0f)
        {
            targPos -= camForward * distInFrontOfCamera;
        }
       pos = RectTransformUtility.WorldToScreenPoint (mainCamera, targPos);*/
    }

    public void SetTrackingObject(GameObject objectToTrack)
    {
        TrackingObject = objectToTrack;
    }
}