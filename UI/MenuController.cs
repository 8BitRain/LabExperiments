using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine;


public class MenuController : MonoBehaviour
{
    private bool menuOpen = false;
    public RectTransform menu;

    public InputActionReference startInput;

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
