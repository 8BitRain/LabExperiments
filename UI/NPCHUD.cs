using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCHUD : MonoBehaviour
{
    public GameObject HealthBar;
    public GameObject StaminaBar;
    private LabGameManager gameManager;

    private GameObject healthBarInstance;
    private GameObject staminaBarInstance;

    private void OnEnable()
    {
        Debug.Log("enabled");
        Debug.Log(EventsManager.instance);
        CameraController.onCameraLoaded += SpawnHealthBar;
        CameraController.onCameraLoaded += SpawnStaminaBar;
    }

    private void OnDisable()
    {
        CameraController.onCameraLoaded -= SpawnHealthBar;
        CameraController.onCameraLoaded -= SpawnStaminaBar;
    }

    private void OnDestroy()
    {
       CameraController.onCameraLoaded -= SpawnHealthBar;
       CameraController.onCameraLoaded -= SpawnStaminaBar;
    }

    public void Start()
    {
        gameManager = GameObject.FindGameObjectWithTag("LabGameManager").GetComponent<LabGameManager>();
    }

    public void SpawnHealthBar(Camera cameraInstance)
    {
        Debug.Log("Healthbar for " + this.gameObject.name + " spawned!");
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
                UIComponent.SetTrackingObject(bodyComponent.healthBarSpawn.gameObject);
            }
            else
            {
                UIComponent.SetTrackingObject(this.gameObject);
            }
        } 
    }

    public void SpawnStaminaBar(Camera cameraInstance)
    {
        Debug.Log("Stamina Bar for " + this.gameObject.name + " spawned!");
        staminaBarInstance = Instantiate(StaminaBar, this.transform.position, this.transform.rotation);
        staminaBarInstance.transform.SetParent(this.transform);
        
        //HealthBar is a scrip that is attached to the GameObject that is a child of the HealthBarCanvas
        if(staminaBarInstance.transform.GetChild(0).TryGetComponent<StaminaBar>(out StaminaBar staminaBar))
        {
            Debug.Log("Setting owner instance to: " + this.gameObject.name);
            staminaBar.SetOwnerInstance(this.gameObject);

            //Set Healthbar max health
            if(TryGetComponent<Status>(out Status status))
            {
                staminaBar.SetMaxStamina(status.stamina);
                staminaBar.SetStamina(status.stamina);
            }
            else
            {
                Debug.Log("No Status script attached to: " + this.gameObject.name);
            }
            
        }

        

        //UIFaceCamera is a script that is attached to the GameObject that is a child of the StaminaBarCanvas    
        if(staminaBarInstance.transform.GetChild(0).TryGetComponent<UIFaceCamera>(out UIFaceCamera UIComponent))
        {
            UIComponent.mainCamera = cameraInstance;
            if(TryGetComponent<Body>(out Body bodyComponent))
            {
                UIComponent.SetTrackingObject(bodyComponent.staminaBarSpawn.gameObject);
            }
            else
            {
                UIComponent.SetTrackingObject(this.gameObject);
            }
        } 
    }
}
