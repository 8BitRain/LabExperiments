using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using System;
using UnityEngine;

public class CooldownController : MonoBehaviour
{
    public List<Cooldown> activeCooldowns;
    public List<Cooldown> expiredCooldowns;

    public enum State
    {
        Initialize,
        Increment,
        Decrement
    }

    public static event Action<GameObject, float, CooldownController.State, Cooldown> onUpdateCooldownIcon;
    public static event Action<GameObject, Cooldown,int> onUpdateCooldown;

    void Start()
    {
        activeCooldowns = new List<Cooldown>();
    }

    void Update()
    {
        if(activeCooldowns.Count != 0)
        {
            foreach (var cooldown in activeCooldowns)
            {
                //Lower time of each cooldown
                cooldown.time -= Time.deltaTime;
                //Debug.Log("Decreasing cooldown time of " + cooldown.name + ": " + cooldown.activeSkill.name + " " + cooldown.time);
                onUpdateCooldownIcon.Invoke(this.gameObject, cooldown.time, CooldownController.State.Increment, cooldown);
                if(cooldown.time <= 0)
                {
                    QueueCooldownForRemoval(cooldown);
                }
            }
        }

        if(expiredCooldowns.Count != 0)
        {
            foreach (var expiredCooldown in expiredCooldowns)
            {
                activeCooldowns.Remove(expiredCooldown);
                Destroy(expiredCooldown.gameObject);
            }
            expiredCooldowns.Clear();
        }
    }

    public void StartCooldown(Cooldown cooldown)
    {
        //print("Adding cooldown " + cooldown.name + ": " + cooldown.activeSkill.name + " to list of active cooldowns.");
        activeCooldowns.Add(cooldown);
        onUpdateCooldownIcon.Invoke(this.gameObject, cooldown.time, CooldownController.State.Initialize, cooldown);
        onUpdateCooldown.Invoke(this.gameObject, cooldown, 1);
        //print("Destroying Cooldown: " + cooldown.name + ": " + cooldown.activeSkill.name);
    }

    public void QueueCooldownForRemoval(Cooldown cooldown)
    {
        expiredCooldowns.Add(cooldown);
        //Send message back to the owner to remove the skill usage limit
        onUpdateCooldown.Invoke(this.gameObject, cooldown, 0);
        //Concern. Is there a case, where the activeSkill object is destroyed, before the cooldown completes?
    }


}
