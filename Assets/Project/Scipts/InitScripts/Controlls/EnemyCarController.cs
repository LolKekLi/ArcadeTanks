using UnityEngine;

namespace Project
{
    public class EnemyCarController : CarBaseController
    {
        protected override Quaternion CalculateRotation()
        {
            var angle = GetTargetAngle();

            var targRot = Quaternion.Euler(Vector3.up * angle);
            var oldRotation = _rigidbody.rotation;

            Quaternion newRotation = oldRotation;

            if ((MovePositionConstraints & RigidbodyConstraints.FreezeRotationY) == 0)
            {
                float carRotationSpeed = RotationSpeed; // carControlItem.GetCarRotationSpeed();
                newRotation = Quaternion.Lerp(oldRotation, targRot, carRotationSpeed * Time.deltaTime);
            }

            return newRotation;
        }
    }
}