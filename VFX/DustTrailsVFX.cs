using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DustTrailsVFX : MonoBehaviour
{
    public MovementComponent movementComponent;
    public GameEvent displayEvent;
    public GameEvent hideEvent;
    public GameObject vfx;

    private bool isAcceleratingBool;
    private bool isDeceleratingBool;


    void Update()
    {
        if(movementComponent.currentSpeed > 30)
        {
            //displayEvent.Raise();
        }
        if(movementComponent.currentSpeed < 30)
        {
            //if(displayEvent)
        }
    }
}
