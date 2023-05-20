using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Project
{
    public abstract class CarBaseController : MonoBehaviour
    {
        protected const float RotationSpeed = 20f;

        [FormerlySerializedAs("DragPower")]
        [SerializeField]
        protected float _dragPower = 1f;
        [FormerlySerializedAs("Sensitivity")]
        [SerializeField]
        protected float _sensitivity = 0f;

        [SerializeField]
        private Animator _animator = null;

        [SerializeField]
        private TrailRenderer[] _trailRenderers = null;
        [SerializeField]
        protected float _deltaHorMax = 2f;

        [Header("Components")]
        [SerializeField]
        protected Rigidbody _rigidbody = null;
        [SerializeField]
        private Transform _centerOfMass = null;
        [SerializeField]
        private Transform _carForwardPoint = null;
        [SerializeField]
        private Transform _backPoint = null;
        [SerializeField]
        private bool _globalForwardMovement = true;

        [Header("Config")]
        [SerializeField]
        protected CarConfig _carConfig = null;

        private float _currentForwardVelocity = 0f;
        private float _targetHorNormal = 0f;

        private Func<float> _getMoveSpeedOverride = null;

        private float _curDecSpeed = 0;
        private float _maxFrowardVelocity = 0f;

        private int _rampCounter = 0;

        private bool _someFlagForRampMoveDown = false;
        private bool _isTrailEnable = true;
        private bool _isRotationsEnabled = true;
        private bool _isRotationsEnabledResetRotation = false;
        private bool _decSpeedAllowed = true;
        private bool _isMoveForwardAllowed = false;
        private bool _clampPosX = true;

        private Quaternion _rotationDamp = Quaternion.identity;

        protected bool IsTraisEnable
        {
            get => _isTrailEnable;
        }

        public float CurrentForwardVelocity
        {
            get => _currentForwardVelocity;
        }

        public float DeltaHorMax
        {
            get => _deltaHorMax;
        }

        public Rigidbody Rigidbody
        {
            get => _rigidbody;
        }

        public float TargetHorNormal
        {
            get => _targetHorNormal;
        }
        public float ActualHorPos
        {
            get;
            private set;
        }

        public bool IsMoveForwardAllowed
        {
            get => _isMoveForwardAllowed;
        }

        public float CurMoveSpeed
        {
            get;
            private set;
        }

        public RigidbodyConstraints MovePositionConstraints
        {
            get;
            private set;
        } = RigidbodyConstraints.None;

        public Transform CarForwardPoint
        {
            get => _carForwardPoint;
        }

        public Transform BackPoint
        {
            get => _backPoint;
        }

        public bool IsRampActive
        {
            get => _rampCounter > 0 || _someFlagForRampMoveDown;
        }

        protected Vector3 MyForward
        {
            get => _globalForwardMovement ? Vector3.forward : Vector3.back;
        }

        public bool IsUser
        {
            get;
            private set;
        } = false;

        public bool DecSpeedAllowed
        {
            get => _decSpeedAllowed;
            private set => _decSpeedAllowed = value;
        }

        protected virtual void Awake()
        {

        }

        protected virtual void Start()
        {
            
        }

        protected virtual void OnEnable()
        {
            
        }

        protected virtual void OnDisable()
        {
            
        }

        public virtual void Init()
        {
            _targetHorNormal = 0f;
            _rigidbody.centerOfMass = _centerOfMass.position;
            _currentForwardVelocity = _carConfig.StartVelocity;

            _maxFrowardVelocity = _carConfig.MaxVelocity;
        }

        protected virtual void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Ramp"))
            {
                return;
            }
            _rampCounter++;
        }

        protected virtual void OnTriggerExit(Collider other)
        {
            if (!other.CompareTag("Ramp"))
            {
                return;
            }
            _rampCounter--;
        }

        private void FixedUpdate()
        {
            if (!_isMoveForwardAllowed)
            {
                return;
            }

            CurMoveSpeed = CalculateSpeed();

            Quaternion targRotation = Quaternion.identity;
            if (_isRotationsEnabled)
            {
                targRotation = CalculateRotation();
            }
            else if (!_isRotationsEnabledResetRotation)
            {
                targRotation = transform.rotation;
            }

            CalculateBlendingRotation(targRotation);

            var targPos = CalculatePosition(targRotation);

            ApplyMovement(targRotation, targPos);
        }

        private void CalculateBlendingRotation(Quaternion rot)
        {
            var rotation = rot.eulerAngles;
            float value = rotation.y;

            if (value > _carConfig.MaxRotationAngle + 1)
            {
                value -= 360;
            }

            value /= _carConfig.MaxRotationAngle;

            value = Mathf.Clamp(value, -1, 1);

            if (_animator)
            {
                _animator.SetFloat("Clamp", value);
            }
        }

        protected virtual void ApplyMovement(Quaternion targRotation, Vector3 targPos)
        {
            _rigidbody.MoveRotation(targRotation);
            _rigidbody.MovePosition(targPos);
            ActualHorPos = targPos.x;
        }

        protected virtual float CalculateSpeed()
        {
            _maxFrowardVelocity = Mathf.Max(_carConfig.MaxVelocity,
                _maxFrowardVelocity - _carConfig.BrakingToMaxVelocity * Time.deltaTime);

            float newVal = Mathf.Min(_maxFrowardVelocity,
                _currentForwardVelocity + _carConfig.AccelerationVelocity * Time.deltaTime);
            float decSpeedTarg = Mathf.Clamp(newVal + _curDecSpeed, _carConfig.StartVelocity, _carConfig.MaxVelocity);
            newVal = Mathf.Lerp(newVal, decSpeedTarg, Time.deltaTime * _carConfig.ChangeSpeed);
            _curDecSpeed = Mathf.Lerp(_curDecSpeed, 0f, Time.deltaTime);
            _currentForwardVelocity = newVal;
            return _getMoveSpeedOverride?.Invoke() ?? _currentForwardVelocity;
        }

        protected virtual Vector3 CalculatePosition(Quaternion targRotation)
        {
            var newForward = targRotation * MyForward;
            Vector3 targMovement = newForward * CurMoveSpeed;
            var deltaPos = targMovement * Time.fixedDeltaTime;
            deltaPos.x *= _carConfig.HorizontalSpeed;

            var horPosition = _rigidbody.position;
            float targetX = _targetHorNormal * DeltaHorMax;
            float deltaTargetX = targetX - horPosition.x;
            
            if (_globalForwardMovement && Mathf.Abs(deltaPos.x) > Mathf.Abs(deltaTargetX))
            {
                float sign = Mathf.Sign(deltaPos.x);
                float minDelta = Mathf.Min(Mathf.Abs(deltaPos.x), Mathf.Abs(deltaTargetX));
                deltaPos.x = sign * minDelta;
            }
            if ((MovePositionConstraints & RigidbodyConstraints.FreezePositionX) == 0)
            {
                horPosition.x += deltaPos.x;

                if (_clampPosX)
                {
                    horPosition.x = Mathf.Clamp(horPosition.x, -DeltaHorMax, DeltaHorMax);
                }
            }
            return horPosition;
        }

        public void AddHorizontalNormal(float deltaPart)
        {
            SetHorizontalNormal(_targetHorNormal + deltaPart);
        }

        public void SetHorizontalNormal(float targetHor)
        {
            if (_clampPosX)
            {
                _targetHorNormal = Mathf.Clamp(targetHor, -1, 1);
            }
            else
            {
                _targetHorNormal = targetHor;
            }
        }

        public void SetClampPos(bool needClamp = true)
        {
            _clampPosX = needClamp;
        }

        public void SetCarMoveAllowed(bool isAllowed, bool isPhysicsAllowed)
        {
            _isMoveForwardAllowed = isAllowed;
            _rigidbody.useGravity = isPhysicsAllowed;
            _rigidbody.isKinematic = !isPhysicsAllowed;
        }

        public void SetTrailsEnabled(bool isEnable)
        {
            if (isEnable == _isTrailEnable)
            {
                return;
            }

            foreach (var trail in _trailRenderers)
            {
                trail.emitting = isEnable;
            }

            _isTrailEnable = isEnable;
        }

        public void DecreaseSpeed(float decSpeed)
        {
            if (!_decSpeedAllowed)
            {
                return;
            }
            if (decSpeed < 0)
            {
                _maxFrowardVelocity -= decSpeed;
                return;
            }

            _curDecSpeed -= decSpeed;
            if (_currentForwardVelocity + _curDecSpeed < _carConfig.StartVelocity)
            {
                _curDecSpeed = _carConfig.StartVelocity - _currentForwardVelocity;
            }
        }
        public float CalculateActualHorNormal()
        {
            return ActualHorPos / DeltaHorMax;
            //return Mathf.Clamp(m_carForwardPoint.position.x / DeltaHorMax, -1, +1);
        }

        public void SetSpeedOverride(Func<float> getMovementSpeed)
        {
            _getMoveSpeedOverride = getMovementSpeed;
        }

        /// <summary>
        /// Return Target angle depended on TargetHorNormal and front point of car
        /// </summary>
        /// <returns></returns>
        protected float GetTargetAngle()
        {
            return GetTargetAngle(Rigidbody.position, _carForwardPoint.localPosition.z, _targetHorNormal);
        }

        protected virtual float GetTargetAngle(Vector3 position, float minForwardLeng, float targHorNormal)
        {
            // as MyForward (global)
            // var forwardXz = new Vector2(0, minForwardLeng);
            // var tarrgetHorXz = new Vector2(targHorNormal * DeltaHorMax - position.x, minForwardLeng);
            // var angle = Vector2.SignedAngle(tarrgetHorXz, forwardXz);
            var frontCarToTarg = new Vector3(targHorNormal * DeltaHorMax - position.x, 0f, minForwardLeng * MyForward.z);
            float angle = Vector3.SignedAngle(MyForward, frontCarToTarg, Vector3.up);
            return angle;
        }
        
        public void SetRotationsEnabled(bool b, bool resetRotation = true)
        {
            _isRotationsEnabled = b;
            _isRotationsEnabledResetRotation = resetRotation;
        }

        public virtual void SetAngleTargetX(float angleX)
        {

        }

        protected abstract Quaternion CalculateRotation();

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            // Targets
            Gizmos.color = Color.blue;
            var targAngle = GetTargetAngle();
            var targRot = Quaternion.Euler(Vector3.up * targAngle);
            var targDir = targRot * MyForward;
            Gizmos.DrawLine(transform.position, transform.position + targDir);
            Gizmos.DrawSphere(Rigidbody.position + (targDir * (_carForwardPoint.localPosition.z + 0.1f)), 0.03f);
            Gizmos.color = new Color(1f, .1f, 1f, .8f);
            Gizmos.DrawLine(Rigidbody.position, Rigidbody.position + targDir);
            Gizmos.DrawSphere(Rigidbody.position + (targDir * (_carForwardPoint.localPosition.z + 0.15f)), 0.03f);

            // Currents
            Gizmos.color = Color.white;
            Gizmos.DrawLine(transform.position, transform.position + transform.forward);
            Gizmos.color = Color.gray;
            Gizmos.DrawLine(Rigidbody.position, Rigidbody.position + (Rigidbody.rotation * MyForward));
            Gizmos.color = Color.black;
            var pos = Rigidbody.position;
            var pos2 = pos;
            pos2.x = TargetHorNormal * DeltaHorMax;
            Gizmos.DrawLine(pos, pos2);
            Gizmos.DrawSphere(pos2, 0.03f);
        }

        [ContextMenu("Find Wheel Trails")]
        private void FindWheelTrails()
        {
            _trailRenderers = new TrailRenderer[0];
            _trailRenderers = GetComponentsInChildren<TrailRenderer>();
        }
#endif
    }
}