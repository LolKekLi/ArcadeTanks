using System.Collections.Generic;
using UnityEngine;

namespace Project
{
    public static class TrajectoryExtensions
    {
        private const int StepMultiplier = 30;
        private const float Angle = 30f;
        
        private static void BackwardEuler(float h, Vector3 currentPosition, Vector3 currentVelocity, out Vector3 newPosition, out Vector3 newVelocity)
        {
            Vector3 accelerationFactor = Physics.gravity;

            newVelocity = currentVelocity + h * accelerationFactor;

            newPosition = currentPosition + h * newVelocity;
        }

        public static float CalculateTimeToHitTarget(Vector3 currentPosition, Vector3 targetPosition, Vector3 velocity)
        {
            //Init values
            Vector3 finalPos = targetPosition;
            Vector3 currentVelocity = velocity;

            Vector3 newPosition = Vector3.zero;
            Vector3 newVelocity = Vector3.zero;

            //The total time it will take before we hit the target
            float time = 0f;
            float h = Time.fixedDeltaTime;
            //Limit to 30 seconds to avoid infinite loop if we never reach the target
            for (time = 0f; time < 30f; time += h)
            {
                BackwardEuler(h, currentPosition, currentVelocity, out newPosition, out newVelocity);

                //If we are moving downwards and are below the target, then we have hit
                if (newPosition.y < currentPosition.y && newPosition.y < finalPos.y)
                {
                    //Add 2 times to make sure we end up below the target when we display the path
                    time += h * 2f;

                    break;
                }

                currentPosition = newPosition;
                currentVelocity = newVelocity;
            }

            return time;
        }

        public static Vector3[] GetTrajectoryPath(Vector3 currentPosition, Vector3 targetPosition, Vector3 velocity)
        {
            //How long did it take to hit the target?
            float timeToHitTarget = CalculateTimeToHitTarget(currentPosition, targetPosition, velocity);

            float h = Time.fixedDeltaTime * StepMultiplier;
            //How many segments we will have
            int maxIndex = Mathf.RoundToInt(timeToHitTarget / h);

            //Start values
            Vector3 newPosition = Vector3.zero;
            Vector3 newVelocity = Vector3.zero;

            List<Vector3> positions = new List<Vector3>();

            //Build the trajectory line
            // NOTE: последняя точка должна быть targetPosition без смещений
            for (int index = 0; index < maxIndex - 1; index++)
            {
                //Calculate the new position of the bullet
                BackwardEuler(h, currentPosition, velocity, out newPosition, out newVelocity);

                currentPosition = newPosition;
                velocity = newVelocity;
                
                positions.Add(currentPosition);
                
            }
            
            positions.Add(targetPosition);

            return positions.ToArray();
        }
        
        public static Vector3 GetVelocity(Transform transform, Vector3 finalPosition)
        {
            Vector3 projectilePos = transform.position;
            // rotate the object to face the target
            transform.LookAt(finalPosition);
            // shorthands for the formula
            float distance = Vector3.Distance(projectilePos, finalPosition);
            float gravity = Physics.gravity.y;
            // NOTE: надо менять угол
            float tanAlpha = Mathf.Tan(Angle * Mathf.Deg2Rad);
            float height = finalPosition.y - projectilePos.y;
            // calculate the local space components of the velocity 
            // required to land the projectile on the target object 
            float velocityZ = Mathf.Sqrt(gravity * distance * distance / (2.0f * (height - distance * tanAlpha)));
            float velocityY = tanAlpha * velocityZ;
            // create the velocity vector in local space and get it in global space
            Vector3 localVelocity = new Vector3(0f, velocityY, velocityZ);
            Vector3 globalVelocity = transform.TransformDirection(localVelocity);
            return globalVelocity;
        }
    }
}