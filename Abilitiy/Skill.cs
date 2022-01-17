using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using DG.Tweening;
using UnityEngine;

public class Skill : MonoBehaviour
{
    public Skill.Name skillName;
    private GameObject playerReference;
    private AnimationController animationController;
    private CooldownController cooldownController;
    private Transform skillSpawnPosition;
    public bool skillUsed = false;

    public enum Name
    {
        MoonBolt,
        Darknet,
        ShadowLine
    }
    
    public enum CooldownBar
    {
        A,
        B,
        C,
        D
    }

    public enum SkillComponent
    {
        Beam,
        ChargeLine,
        ChargeParticles,
    }

    public virtual void UseSkill()
    {
        print("Generic superclass skill called");
    }

    public void SetPlayerReference(GameObject playerReference)
    {
        this.playerReference = playerReference;
    }

    public void SetPlayerAnimationController(AnimationController animationController)
    {
        this.animationController = animationController;
    }

    public void SetCooldownController(CooldownController cooldownController)
    {
        this.cooldownController = cooldownController;
    }

    public void SetSkillSpawnPoint(Transform skillSpawnPosition)
    {
        this.skillSpawnPosition = skillSpawnPosition;
    }

    public GameObject GetPlayerReference()
    {
        return this.playerReference;
    }

    public AnimationController GetAnimationController()
    {
        return this.animationController;
    }

    public CooldownController GetCooldownController()
    {
        return this.cooldownController;
    }

    public Transform GetSkillSpawnPosition()
    {
        return this.skillSpawnPosition;
    }

    public bool IsSkillInCooldown()
    {
        return this.skillUsed;
    }
}
