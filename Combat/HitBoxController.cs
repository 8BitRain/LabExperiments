using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class HitBoxController : MonoBehaviour
{
    [Header("Hitboxes")]
    public HitBox[] hitBoxes;
    Dictionary<AttackEnums.Attacks, HitBox> hitBoxDictionary = new Dictionary<AttackEnums.Attacks, HitBox>();

    // Start is called before the first frame update
    void Start()
    {
        
        for(int i = 0; i < hitBoxes.Length; i++)
        {
            //hitBoxDictionary.Add(hitBoxes[i].GetComponent<HitBox>().attackName, hitBoxes[i]);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //Singular HitBox Activation
    public void ActivateHitBox(HitBox hitBox)
    {
        hitBox.ActivateHitBox();     
    }

    //Singular HitBox Deactivation
    public void DeactivateHitBox(HitBox hitBox)
    {
        hitBox.DeactivateHitBox();
    }

    public void ActivateHitBoxes(AttackEnums.Attacks attackName)
    {
        HitBox[] attackHitBoxes = GetComponent<HitBoxController>().GetHitBoxes(attackName);
        foreach (var hitbox in attackHitBoxes)
        {
            hitbox.ActivateHitBox();
        }
    }

    public void DeactivateHitBoxes(AttackEnums.Attacks attackName)
    {
        HitBox[] attackHitBoxes = GetComponent<HitBoxController>().GetHitBoxes(attackName);
        foreach (var hitbox in attackHitBoxes)
        {
            hitbox.DeactivateHitBox();
        }
    }


    public HitBox[] GetHitBoxes(AttackEnums.Attacks attackName)
    {
        List<HitBox> selectedHitBoxes = new List<HitBox>();
        //https://stackoverflow.com/questions/202813/adding-values-to-a-c-sharp-array
        foreach (var hitBox in hitBoxDictionary)
        {
            if(hitBox.Key == attackName)
            {
                //Debug.Log("HitBox found: " + hitBox.Value.name + "which belongs to : " + hitBox.Value.Agent.name + "'s " +  hitBox.Key);
                selectedHitBoxes.Add(hitBox.Value);
            }
        }

        if(selectedHitBoxes.ToArray().Length != 0)
        {
            //Debug.Log("Value added to HitBox array: " + selectedHitBoxes.ToArray()[0]);
        }
        return selectedHitBoxes.ToArray();
    }
}
