using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCHUD : MonoBehaviour
{
    public GameObject HealthBar;
    private LabGameManager gameManager;

    private GameObject healthBarInstance;

    private void OnEnable()
    {
        Debug.Log("enabled");
        Debug.Log(EventsManager.instance);
        CameraController.onCameraLoaded += SpawnHealthBar;
    }

    private void OnDisable()
    {
        CameraController.onCameraLoaded -= SpawnHealthBar;
    }

    private void OnDestroy()
    {
       CameraController.onCameraLoaded -= SpawnHealthBar;
    }

    public void Start()
    {
        gameManager = GameObject.FindGameObjectWithTag("LabGameManager").GetComponent<LabGameManager>();
    }

    public void SpawnHealthBar(Camera cameraInstance)
    {
        Debug.Log("Happens");
        healthBarInstance = Instantiate(HealthBar, this.transform.position, this.transform.rotation);
        healthBarInstance.transform.SetParent(this.transform);
        
        //HealthBar is a scrip that is attached to the GameObject that is a child of the HealthBarCanvas
        if(healthBarInstance.transform.GetChild(0).TryGetComponent<HealthBar>(out HealthBar healthBar))
        {
            Debug.Log("Setting owner instance to: " + this.gameObject.name);
            healthBar.SetOwnerInstance(this.gameObject);

            //Set Healthbar max health
            if(TryGetComponent<Status>(out Status status))
            {
                healthBar.SetMaxHealth(status.hp);
                healthBar.SetHealth(status.hp);
            }
            else
            {
                Debug.Log("No Status script attached to: " + this.gameObject.name);
            }
            
        }

        //UIFaceCamera is a script that is attached to the GameObject that is a child of the HealthBarCanvas    
        if(healthBarInstance.transform.GetChild(0).TryGetComponent<UIFaceCamera>(out UIFaceCamera UIComponent))
        {
            UIComponent.mainCamera = cameraInstance;
            if(TryGetComponent<Body>(out Body bodyComponent))
            {
                UIComponent.SetTrackingObject(bodyComponent.Head.gameObject);
            }
            else
            {
                UIComponent.SetTrackingObject(this.gameObject);
            }
        } 
    }
}
