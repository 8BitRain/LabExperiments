using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using System;

public class DetachFromParent : MonoBehaviour
{
    public static event Action<string, GameObject> onUpdateMeleeSpawnPoint;
    // Start is called before the first frame update
    void Awake()
    {
        //transform.parent = null;
    }

    // Update is called once per frame
    void Update()
    {
        //GetComponent<CinemachineBrain>().Cam
    }

    public void DetachOverlayCamera()
    {
        //Patch for a camera instance that has an overlay camera
        if(GetComponentInChildren<Camera>() != null)
        {
            Debug.Log("UNPARENTING CAMERA");
            GameObject overlayCam = transform.GetChild(0).gameObject;
            overlayCam.transform.SetParent(null);
            //Really messy approach to correctly position overlay camera enable for CameraTestLab
            //overlayCam.gameObject.transform.position = new Vector3(-169.24f, 34.63593f, 8.369999f);
            //Main game setting
            overlayCam.gameObject.transform.position = new Vector3(-169.6f, 40.61593f, 8.369999f);

            //rotx = -0.517
            //roty = 90.00001
            //rotz = 0;

            //This will only work for 1 player
            onUpdateMeleeSpawnPoint.Invoke("P1", this.gameObject);
        }
    }
}
