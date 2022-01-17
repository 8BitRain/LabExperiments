using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using System;
using UnityEngine;

public class MobileAbilityTargetingSystem : MonoBehaviour
{
    //Ability should be able to move up/down left/right forward and backward, and only be stopped by gravity
    //Time should slow while using the ability
    
    public Vector2 movementInput;
    public GameObject playerReference;
    private bool moveUpInput;
    private bool moveDownInput;
    private bool castInput;

    [Header("Movement Settings")]
    public float speed = 6.0f;

    private float yDirection = 0;

    public static event Action<GameObject> onSkillCast;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(playerReference != null)
        {
            UseTargeter();
        } 
    }

    public void UseTargeter()
    {
        //Have player face skill
        playerReference.transform.LookAt(this.transform.position);

        //I could add a cieling to how high an ability skill can climb. 

        Vector3 direction = new Vector3(movementInput.x,0,movementInput.y).normalized;
        //Attempt to lock y position. Y seems to still be adjusted. I wonder if this has anything to do with translate being based on Camera.main.transform's orientation.
        direction.y = 0;
        transform.Translate(direction * speed *Time.deltaTime, Camera.main.transform);
        print("MainCameraTransform: " + Camera.main.transform.name);

        if(moveUpInput)
        {
            //yDirection += 1;
            direction.y += 1;
            transform.Translate(direction * speed *Time.deltaTime, Camera.main.transform);
        }

        if(moveDownInput)
        {
            //yDirection -= 1;
            direction.y -= 1;
            transform.Translate(direction * speed *Time.deltaTime, Camera.main.transform);
        }

        if(castInput)
        {
            onSkillCast.Invoke(this.gameObject);
        }
    }

    public void OnMove(InputAction.CallbackContext ctx) => movementInput = ctx.ReadValue<Vector2>();
    public void OnMoveUp(InputAction.CallbackContext ctx) => moveUpInput = ctx.ReadValueAsButton();
    public void OnMoveDown(InputAction.CallbackContext ctx) => moveDownInput = ctx.ReadValueAsButton();
    public void OnCastInput(InputAction.CallbackContext ctx) => castInput = ctx.ReadValueAsButton();
}
