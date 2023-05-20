using UnityEngine;
using Project.UI;

namespace Project
{
    public class PlayerCarController : CarBaseController
    {
        [SerializeField]
        private Transform[] _forwardWheels = null;
        [SerializeField]
        private Transform[] _backWheels = null;
        [SerializeField]
        private float _angleToEnableTrails = 5f;

        private float _angleX = 0f;
        private float _delta = 0f;
        private Vector3 _lastBackWheelsHeight = Vector3.zero;
        private Vector3 _lastForwardWheelsHeight = Vector3.zero;
        private bool _inAir = false;

        protected override void Start()
        {
            base.Start();

            SetCarMoveAllowed(true, false);
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            
            JoystickController.Clicked += JoystickController_Clicked;
            JoystickController.Dragged += JoystickController_Dragged;
            JoystickController.Released += JoystickController_Released;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            
            JoystickController.Clicked -= JoystickController_Clicked;
            JoystickController.Dragged -= JoystickController_Dragged;
            JoystickController.Released -= JoystickController_Released;
        }

        public override void Init()
        {
            base.Init();
            
            Rigidbody.constraints = RigidbodyConstraints.FreezeAll & ~(RigidbodyConstraints.FreezePositionY);
            
            foreach (var wc in gameObject.GetComponentsInChildren<WheelCollider>())
            {
                wc.enabled = false;
            }

            SetTrailsEnabled(false);
        }

        protected override Quaternion CalculateRotation()
        {
            var angle = GetTargetAngle();
            var oldRotation = Rigidbody.rotation;
            var newRotation = Quaternion.identity;

            CalculateXAngle();
            //SetTrailsEnabled(IsOnGround() && Mathf.Abs(angle) > _angleToEnableTrails);
            SetTrailsEnabled(Mathf.Abs(angle) > _angleToEnableTrails);

            var oldRotDir = oldRotation * MyForward;
            var forwardOnly = Vector3.ProjectOnPlane(oldRotDir, Vector3.up);
            float fromAngle = Vector3.SignedAngle(MyForward, forwardOnly, Vector3.up);
            float toAngle = Mathf.Clamp(angle, -_carConfig.MaxRotationAngle, _carConfig.MaxRotationAngle);

            float maxRotationSpeed = _carConfig.RotationSpeed;
            if (IsUser)
            {
                //the same comment like in CarMovementController
                //CarControlItem carControlItem = (CarControlItem)User.GetItem<CarControlItem>();
                maxRotationSpeed = RotationSpeed;// carControlItem.GetCarRotationSpeed();
            }

            if (Mathf.Abs(toAngle) > 0.01f)
            {
                // For speedUp rotateSpeed
                if (toAngle > 0 != fromAngle > 0)
                {
                    maxRotationSpeed *= 1.3f;
                }
            }
            if (_inAir)
            {
                //maxRotationSpeed /= 3f;
            }

            //oldRotDir = oldRotation * MyForward;
            oldRotDir = (Quaternion.Euler(oldRotation.eulerAngles.x, 0, 0)) * MyForward;
            var forwardOnlyX = Vector3.ProjectOnPlane(oldRotDir, Vector3.right);
            float fromAngleX = Vector3.SignedAngle(MyForward, forwardOnlyX, Vector3.right);
            fromAngleX = Mathf.Clamp(fromAngleX, -_carConfig.MaxRotationAngleX, _carConfig.MaxRotationAngleX);

            float toAngleX = Mathf.Clamp(_angleX, float.NegativeInfinity, _carConfig.MaxRotationAngleX);
            float newAngle = fromAngle;
            if ((MovePositionConstraints & RigidbodyConstraints.FreezeRotationY) == 0)
            {
                newAngle = Mathf.MoveTowardsAngle(fromAngle, toAngle, maxRotationSpeed * Time.deltaTime);
            }

            var rotationSpeedX = _carConfig.RotationSpeedX * Time.deltaTime;
            //Debug.Log($"{m_inAir} + from={fromAngleX:F2} >= to={toAngleX:F2} ; fromAngleX2={fromAngleX2:F2}");
            if (_inAir && fromAngleX >= toAngleX)
            {
                float targKoefficient = 1f - (Mathf.Abs(toAngleX) / (_carConfig.MaxRotationAngleX + 1)) / 3f;
                //Debug.Log(targKoefficient.ToString("F2"));
                rotationSpeedX *= targKoefficient;
            }
            else if (toAngleX < fromAngleX)
            {
                rotationSpeedX *= 5f;
            }

            float newAngleX = fromAngleX;
            if ((MovePositionConstraints & RigidbodyConstraints.FreezeRotationX) == 0)
            {
                newAngleX = Mathf.MoveTowards(fromAngleX, toAngleX, rotationSpeedX);
            }

            newRotation = Quaternion.Euler(Vector3.up * newAngle) * Quaternion.Euler(Vector3.right * newAngleX);
            return newRotation;
        }

        private void CalculateXAngle()
        {
            // mind dist for touch ground is 0.1f
            if (IsRampActive && IsOnGround())
            {
                // TODO: also check back (not only front of car)
                // raycastHit - if cast - that mean we grounded by this side
                CalculateHighestHitPointFromWheel(_forwardWheels, ref _lastForwardWheelsHeight);
                CalculateHighestHitPointFromWheel(_backWheels, ref _lastBackWheelsHeight);

                var dir = (_lastForwardWheelsHeight - _lastBackWheelsHeight).normalized;
                var dir2D = new Vector2(dir.z, dir.y);
                _angleX = Vector2.SignedAngle(dir2D, Vector2.right);
                _inAir = false;
            }
            else
            {
                if (_inAir == IsOnGround())
                {
                    _inAir = !_inAir;
                }
                _lastForwardWheelsHeight = Vector3.zero;
                _lastBackWheelsHeight = Vector3.zero;
                _angleX = 0f;
            }
        }

        private bool IsOnGround()
        {
            float castDist = 1f + 0.1f;
            return Physics.Raycast(CarForwardPoint.position + Vector3.up, Vector3.down, castDist, _carConfig.RoadLayer);
        }

        protected override float GetTargetAngle(Vector3 position, float minForwardLeng, float targHorNormal)
        {
            float targAngle = base.GetTargetAngle(position, minForwardLeng, targHorNormal);
            var frontCarToTarg = new Vector3(targHorNormal * DeltaHorMax - CarForwardPoint.position.x, 0f, minForwardLeng * MyForward.z);
            float alternateAngle = Vector3.SignedAngle(MyForward, frontCarToTarg, Vector3.up);

            if (Mathf.Abs(alternateAngle) >= Mathf.Abs(targAngle))
            {
                alternateAngle = targAngle;
            }
            else
            {
                alternateAngle = (alternateAngle + targAngle) / 2f;
            }

            return alternateAngle;
        }

        private void CalculateHighestHitPointFromWheel(Transform[] wheels, ref Vector3 lastHitPoint)
        {
            var lastPos = Vector3.zero;

            foreach (var wheel in wheels)
            {
                float castDist = 1f + 0.1f;
                if (Physics.Raycast(wheel.position + Vector3.up * 0.1f, Vector3.down, out var hit, castDist, _carConfig.RoadLayer))
                {
                    if (hit.point.y > lastHitPoint.y)
                    {
                        lastHitPoint = hit.point;
                    }

                    lastPos = wheel.position + Vector3.up * 0.1f;
                }
            }

            Debug.DrawLine(lastPos, lastHitPoint, Color.blue);
        }

        protected override Vector3 CalculatePosition(Quaternion targRotation)
        {
            var pos = base.CalculatePosition(targRotation);

            var newForward = targRotation * MyForward;
            Vector3 targMovement = newForward * CurMoveSpeed;
            var deltaPos = targMovement * Time.fixedDeltaTime;

            if ((MovePositionConstraints & RigidbodyConstraints.FreezePositionZ) == 0)
                pos.z += deltaPos.z;
            if ((MovePositionConstraints & RigidbodyConstraints.FreezePositionY) == 0)
                pos.y += deltaPos.y;
            return pos;
        }

        protected override void ApplyMovement(Quaternion targRotation, Vector3 targPos)
        {
            base.ApplyMovement(targRotation, targPos);

            var angular = Rigidbody.angularVelocity;
            var targAngular = Vector3.zero;
            targAngular.x = angular.x;
            angular = Vector3.Lerp(angular, targAngular, Time.fixedDeltaTime * CurMoveSpeed);
            Rigidbody.angularVelocity = angular;

            var tran = transform;
            tran.rotation = targRotation;
            tran.position = targPos;
        }

        public override void SetAngleTargetX(float angleX)
        {
            base.SetAngleTargetX(angleX);
            _angleX = angleX;
        }
        
        private void GameManager_GameFinished()
        {
            SetCarMoveAllowed(false, false);
        }

        protected virtual void GameManager_GameStarted()
        {
            SetCarMoveAllowed(true, true);
        }
        
        private void JoystickController_Clicked()
        {
            //_delta = 0;
        }

        protected virtual void JoystickController_Dragged(Vector2 origin)
        {
            float screenWidth = Screen.width;
            _delta += origin.x * _dragPower;
            //Debug.Log($"{_delta} : {_delta / screenWidth}");
            //float delta = origin.x;
            float horizontalNormal = Mathf.Clamp(_delta / screenWidth, -1, 1);

            if (Mathf.Abs(Mathf.Abs(horizontalNormal) - Mathf.Abs(TargetHorNormal)) < _sensitivity)
            {
                return;
            }

            SetHorizontalNormal(horizontalNormal);
        }

        protected virtual void JoystickController_Released()
        {
            SetHorizontalNormal(_rigidbody.position.x / _deltaHorMax);
        }
    }
}