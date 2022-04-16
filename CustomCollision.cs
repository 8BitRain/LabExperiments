using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class CustomCollision : IComparable<CustomCollision>
{
    public String colliderName;
    public GameObject collisionHit;

    public CustomCollision(String colliderName, GameObject collisionHit){
        this.colliderName = colliderName;
        this.collisionHit = collisionHit;
    }

    public int CompareTo(CustomCollision other){
        //Test this
        return this.colliderName.CompareTo(other.colliderName);
    }
}
