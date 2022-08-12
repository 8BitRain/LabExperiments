using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine;


public class MenuController : MonoBehaviour
{
    private bool menuOpen = false;
    public RectTransform menu;

    public enum MenuElementType
    {
        TEXT,
        BLUR_EFFECT
    }

    public MenuElementType menuElementType;

    public InputActionReference startInput;
    public GameObject playerReference;

    private Camera camera;
    private Canvas menuCanvas;

    void Start()
    {
        if(playerReference != null)
        {
            camera = playerReference.GetComponent<CameraController>().GetCameraInstance().GetComponent<Camera>();
            Debug.Log("Camera is: " + camera.name);
        }


        switch (menuElementType)
        {
            case MenuElementType.TEXT:
                menuCanvas = menu.GetComponentInChildren<Canvas>();
                menuCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
                break;
            case MenuElementType.BLUR_EFFECT:
                menuCanvas = menu.GetComponentInChildren<Canvas>();
                menuCanvas.renderMode = RenderMode.ScreenSpaceCamera;
                menuCanvas.worldCamera = camera;
                menuCanvas.planeDistance = 1f;
                break;
            default:
                break;
        }
    }

    void Update()
    {
        if(startInput.action.triggered && !menuOpen)
        {
            Debug.Log("Open Window");
            OpenMenu();
            return;
        }
        
        if(startInput.action.triggered && menuOpen)
        {
            Debug.Log("Close Window");
            CloseMenu();
            return;
        }
    }

    void OpenMenu()
    {
        if(menu != null)
        {
            menu.gameObject.SetActive(true);
            Time.timeScale = 0f;
            menuOpen = true;
        }
    }

    void CloseMenu()
    {
        if(menu != null)
        {
            menu.gameObject.SetActive(false);
            Time.timeScale = 1f;
            menuOpen = false;
        }
    }
    
}
