using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

public class WithinCombatRange : Conditional
{
   // Set the target variable when a target has been found so the subsequent tasks know which object is the target
   public SharedGameObject targetGameObject;

   public float range;

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
       if(totalDistance < range)
       {
           return true;
       }
       if(totalDistance > range)
       {
           return false;
       }
       return totalDistance < range;
   }
}