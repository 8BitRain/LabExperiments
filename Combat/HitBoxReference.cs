using UnityEngine;
using System.Collections;
using System; //This allows the IComparable Interface

//This is the class you will be storing
//in the different collections. In order to use
//a collection's Sort() method, this class needs to
//implement the IComparable interface.
public class HitBoxReference : IComparable<HitBoxReference>
{
    public string name;
    public HitBox hitBox;

    public HitBoxReference(string newName, HitBox hitBox)
    {
        this.name = newName;
        this.hitBox = hitBox;
    }

    //This method is required by the IComparable
    //interface. 
    public int CompareTo(HitBoxReference other)
    {
        if(other == null)
        {
            return 1;
        }

        //Return the difference in power.
        return this.name.CompareTo(other);
    }
}