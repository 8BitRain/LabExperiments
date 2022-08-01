using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

public class WithinSight : Conditional
{
   // How wide of an angle the object can see
   public float fieldOfViewAngle;
   // The tag of the targets
   public string targetTag;
   // Set the target variable when a target has been found so the subsequent tasks know which object is the target
   public SharedGameObject targetGameObject;
  

   public float range;
   public SharedFloat seekRange;

   // A cache of all of the possible targets
   private Transform[] possibleTargets;
   private Transform currentTarget;

   public override void OnAwake()
   {
      // Cache all of the transforms that have a tag of targetTag
      var targets = GameObject.FindGameObjectsWithTag(targetTag);
      possibleTargets = new Transform[targets.Length];
      for (int i = 0; i < targets.Length; ++i) {
         possibleTargets[i] = targets[i].transform;
      }
   }

   public override TaskStatus OnUpdate()
   {
      // Return success if a target is within sight
      for (int i = 0; i < possibleTargets.Length; ++i) {
        if (IsWithinSight(possibleTargets[i], fieldOfViewAngle)) {
            // Return success if a target is also within range
            if(IsWithinRange(possibleTargets[i]))
            {
                // Set the target so other tasks will know which transform is within sight
                var currentGameObject = GetDefaultGameObject(targetGameObject.Value);
                currentTarget = currentGameObject.GetComponent<Transform>();
                currentTarget = possibleTargets[i];
                return TaskStatus.Success;
            }
        }
      }
      return TaskStatus.Failure;
   }

   // Returns true if targetTransform is within sight of current transform
   public bool IsWithinSight(Transform targetTransform, float fieldOfViewAngle)
   {
      Vector3 direction = targetTransform.position - transform.position;
      // An object is within sight if the angle is less than field of view
      return Vector3.Angle(direction, transform.forward) < fieldOfViewAngle;
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