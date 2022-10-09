using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class FPSReticle : MonoBehaviour
{
    public Vector2 aimInput;
    public float aimSensitivty = 10;
    private RectTransform reticle;
    // Start is called before the first frame update
    void Start()
    {
        reticle = GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        Aim();
    }

    public void OnAim(InputAction.CallbackContext ctx) => aimInput = ctx.ReadValue<Vector2>();

    public void Aim()
    {
        float newPosX = reticle.anchoredPosition.x + (aimInput.x * aimSensitivty);
        float newPosY = reticle.anchoredPosition.y + (aimInput.y * aimSensitivty);

        if(newPosX > Screen.width/2)
        {
            newPosX = Screen.width/2;
        }
        if(newPosX < -Screen.width/2)
        {
            newPosX = -Screen.width/2;
        }
        if(newPosY > Screen.height/2)
        {
            newPosY = Screen.height/2;
        }
        if(newPosY < -Screen.height/2)
        {
            newPosY = -Screen.height/2;
        }

        reticle.anchoredPosition = new Vector2(newPosX, newPosY);


    }

    public Vector3 GetWorldPosition()
    {
        Vector3 reticleWorldPosition = new Vector3(reticle.anchoredPosition.x, reticle.anchoredPosition.y, Camera.main.nearClipPlane);
        Debug.Log("GetReticleWorldPosition: " + reticleWorldPosition);
        return reticleWorldPosition;
    }
}
