using UnityEngine;

//Attach this script to a GameObject to rotate around the target position.
public class RotateAround : MonoBehaviour
{
    //Assign a GameObject in the Inspector to rotate around
    public GameObject target;
    public float rotateSpeed = 20;

    void Update()
    {
        // Spin the object around the target at 20 degrees/second.
        transform.RotateAround(target.transform.position, Vector3.up, rotateSpeed * Time.deltaTime);
    }
}