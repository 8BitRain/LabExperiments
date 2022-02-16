using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MomentumGauge : MonoBehaviour
{
    public MovementComponent movementComponent;
    public GameEvent accelerationEvent;
    public GameEvent decelerationEvent;
    public GameEvent stationaryEvent;

    private float previousSpeed;

    // Start is called before the first frame update
    void Start()
    {
        previousSpeed = movementComponent.currentSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        if(movementComponent.currentSpeed > previousSpeed )
        {
            accelerationEvent.Raise();
        }

        if(movementComponent.currentSpeed < previousSpeed && movementComponent.currentSpeed != previousSpeed)
        {
            decelerationEvent.Raise();
        }

        if(movementComponent.currentSpeed == movementComponent.defaultSpeed || !movementComponent.isGrounded)
        {
            stationaryEvent.Raise();
        }

        previousSpeed = movementComponent.currentSpeed;
    }
}
