using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

public class WithinCombatRange : Conditional
{
    public enum CombatRangeType
    {
        SeekRange,
        MeleeRange,
        ZoneAttackRange,
        EscapeRange
    }

    public CombatRangeType combatRangeType;
    // Set the target variable when a target has been found so the subsequent tasks know which object is the target
    public SharedGameObject targetGameObject;

    public SharedFloat seekRange;
    public SharedFloat meleeRange;
    public SharedFloat zoneAttackRange;
    public SharedFloat escapeRange;

    private Transform currentTarget;

    public override void OnAwake()
    {
    }

    public override TaskStatus OnUpdate()
    {
        var currentGameObject = GetDefaultGameObject(targetGameObject.Value);
        currentTarget = currentGameObject.GetComponent<Transform>();
        if(IsWithinRange(currentTarget))
        {
            return TaskStatus.Success;
        }
        else
        {
            return TaskStatus.Failure;
        }
    }


    public bool IsWithinRange(Transform targetTranform)
    {
        Vector3 direction = targetTranform.position - transform.position;
        float totalDistance = direction.magnitude;
        if(totalDistance < GetRangeTypeValue())
        {
            return true;
        }
        if(totalDistance > GetRangeTypeValue())
        {
            return false;
        }
        return totalDistance < GetRangeTypeValue();
    }

    public float GetRangeTypeValue()
    {
        switch (combatRangeType)
        {
            case CombatRangeType.SeekRange:
                return seekRange.Value;
            case CombatRangeType.MeleeRange:
                return meleeRange.Value;
            case CombatRangeType.ZoneAttackRange:
                return zoneAttackRange.Value;
            case CombatRangeType.EscapeRange:
                return escapeRange.Value;
            default:
                return -1;
        }
    }
}