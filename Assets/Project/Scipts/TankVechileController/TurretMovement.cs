using System;
using Cysharp.Threading.Tasks;
using Project;
using UnityEngine;

public class TurretMovement : MonoBehaviour
{
    private float _turretTraverseSpeed = 45f;
    private float _gunTraverseSpeed = 45f;
    private int _maxGunAngle_elevation = 35;
    private int _minGunAngle_depression = 8;
    private bool _canUpdate;

    private GameObject _turretGameObject;
    private GameObject _gunGameObject;

    private Vector3 _targetPosition;
    private Quaternion _gunLocalRotation;
    private Quaternion _turretLocalRotation;

    public Vector3 TurretTargetPosition
    {
        get;
        set;
    }

    public float MinGunAngle_depression
    {
        get =>
            _minGunAngle_depression;
    }

    public float MaxGunAngle_elevation
    {
        get =>
            _maxGunAngle_elevation;
    }

    private void Awake()
    {
        _canUpdate = false;
    }

    private void Start()
    {
        _turretLocalRotation = Quaternion.identity;
    }

    public async void Setup(GameObject turretGameObject1, GameObject firePosition,
        TankBodySettings.TankMovementPreset tankMovementPreset)
    {
        _turretTraverseSpeed = tankMovementPreset.turretTraverseSpeed;
        _gunTraverseSpeed = tankMovementPreset.gunTraverseSpeed;
        _maxGunAngle_elevation = tankMovementPreset.maxGunAngle_elevation;
        _minGunAngle_depression = tankMovementPreset.minGunAngle_depression;

        _turretGameObject = turretGameObject1;
        _gunGameObject = firePosition;
        TurretTargetPosition = transform.position + (transform.forward * 2);
        _canUpdate = true;
        _targetPosition = TurretTargetPosition;

        await UniTask.Delay(TimeSpan.FromSeconds(0.1f));

        _canUpdate = true;
    }

    private void Update()
    {
        _targetPosition = TurretTargetPosition;

        if (_canUpdate)
        {
            MoveTurret();
        }
    }

    private void MoveTurret()
    {
        if (Input.GetMouseButton(1))
        {
            // Refactor later to inputcontroller
            Debug.Log("holding down");
        }
        else
        {
            // look to target
            Quaternion _lookAtTurret = Quaternion.LookRotation(_targetPosition - _turretGameObject.transform.position,
                gameObject.transform.up);
            Quaternion _lookAtGun = Quaternion.LookRotation(_targetPosition - _gunGameObject.transform.position,
                _gunGameObject.transform.up);
            Quaternion _turretRelativeRotTarget =
                Quaternion.Euler(gameObject.transform.eulerAngles - _lookAtTurret.eulerAngles);
            Quaternion _gunRelativeRotTarget =
                Quaternion.Euler(_turretGameObject.transform.eulerAngles - _lookAtGun.eulerAngles);
            float _angleBetweenTurretAndTarget = Vector3.Angle(_turretLocalRotation * Vector3.forward,
                _turretRelativeRotTarget * Vector3.forward);
            float _angleBetweenGunAndTarget = Vector3.Angle(_gunLocalRotation * Vector3.forward,
                _gunRelativeRotTarget * Vector3.forward);
            if (_angleBetweenGunAndTarget == 0 || float.IsNaN(_angleBetweenGunAndTarget))
            {
                return;
            }

            float _turretVelocity = 1 / _angleBetweenTurretAndTarget;
            float _gunVelocity = 1 / _angleBetweenGunAndTarget;
            float _horizontalSpeed = _turretTraverseSpeed;
            _horizontalSpeed *= _turretVelocity;
            _horizontalSpeed *= Time.deltaTime;
            float _verticalSpeed = _gunTraverseSpeed;
            _verticalSpeed *= _gunVelocity;
            _verticalSpeed *= Time.deltaTime;
            Quaternion _turretFinalRotation =
                Quaternion.Euler(gameObject.transform.eulerAngles - _lookAtTurret.eulerAngles);
            Quaternion _gunFinalRotation =
                Quaternion.Euler(_turretGameObject.transform.eulerAngles - _lookAtGun.eulerAngles);

            _turretLocalRotation = Quaternion.Lerp(_turretLocalRotation, _turretFinalRotation, _horizontalSpeed);
            _gunLocalRotation = Quaternion.Lerp(_gunLocalRotation, _gunFinalRotation, _verticalSpeed);

            if (float.IsNaN(_gunLocalRotation.eulerAngles.x))
            {
                return;
            }

            Quaternion _turretRot =
                Quaternion.Euler(gameObject.transform.eulerAngles - _turretLocalRotation.eulerAngles);
            Quaternion _gunRot =
                Quaternion.Euler(_turretGameObject.transform.eulerAngles - _gunLocalRotation.eulerAngles);
            _gunGameObject.transform.rotation = _gunRot;
            _turretGameObject.transform.rotation = _turretRot;
            Vector3 _newGunRotation = _gunGameObject.transform.localEulerAngles;
            Vector3 _newTurretRotation = _turretGameObject.transform.localEulerAngles;

            {
                float _max = 360 - _maxGunAngle_elevation;
                float _min = _minGunAngle_depression;
                float _currentAngle = _gunGameObject.transform.localEulerAngles.x;
                if (_currentAngle > 180)
                {
                    if (_currentAngle < _max) _newGunRotation.x = _max;
                }
                else
                {
                    if (_currentAngle > _min) _newGunRotation.x = _min;
                }
            }

            _newTurretRotation.x = 0;
            _newTurretRotation.z = 0;
            _newGunRotation.y = 0;
            _newGunRotation.z = 0;
            //apply local rotation
            _turretGameObject.transform.localRotation = Quaternion.Euler(_newTurretRotation);
            _gunGameObject.transform.localRotation = Quaternion.Euler(_newGunRotation);
            try
            {
            }
            catch
            {
                // ignored
            }
        }
    }
}