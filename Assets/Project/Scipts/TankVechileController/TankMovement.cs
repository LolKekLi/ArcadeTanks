using UnityEngine;
using System;
using Project;
using UnityEngine.Serialization;

[RequireComponent(typeof( Rigidbody), typeof(InputController))]
public class TankMovement : MonoBehaviour
{
    [SerializeField]
    private GrodedChecker _grodedChecker = null;
    
    [FormerlySerializedAs("leftWheelColliders")]
    [Header("Wheel Colliders")]
    public WheelCollider[] _leftWheelColliders;
    [FormerlySerializedAs("rightWheelColliders")]
    public WheelCollider[] _rightWheelColliders;
    
    private float _turnTorque = 1f;

    private float _driveTorque = 2f;
    private float _brakeStrength = 3f;
    private float _turningForceCoefficient = 0.7f;
    private float _forwardForceCoefficient = 12f;
    private float _movementSidewaysFriction = 2.2f;
    private float _stillSidewaysFriction = 0.8f;
    private float _centerOfMassYOffset = -1.0f;
    private float _curAngle;
    
    private WheelFrictionCurve[] _sFrictionLeft;
    private WheelFrictionCurve[] _sFrictionRight;
    
    private Rigidbody rigidBody;
    private InputController inputs;

  
    [SerializeField]
    private float velocityInKMH;
    
    public void Setup(TankBodySettings.TankMovementPreset preset)
    {
        _turnTorque = preset.TurnTorque;
        _driveTorque = preset.DriveTorque;
        _brakeStrength = preset.BrakeStrength;
        _turningForceCoefficient = preset.TurningForceCoefficient;
        _forwardForceCoefficient = preset.ForwardForceCoefficient;
        _movementSidewaysFriction = preset.MovementSidewaysFriction;
        _stillSidewaysFriction = preset.StillSidewaysFriction;
        _centerOfMassYOffset = preset.CenterOfMassYOffset;
        
        _turnTorque *= 1000000f;
        _driveTorque *= 100f;
        _brakeStrength *= 1000f;
    }
    
    private void Awake()
    {
        rigidBody = GetComponent<Rigidbody>();
        rigidBody.centerOfMass = new Vector3(0, _centerOfMassYOffset, 0);
        inputs = GetComponent<InputController>();

        _sFrictionLeft = new WheelFrictionCurve[_leftWheelColliders.Length];
        _sFrictionRight = new WheelFrictionCurve[_rightWheelColliders.Length];

        for (int i = 0; i < _leftWheelColliders.Length; i++)
        {
            _sFrictionLeft[i] = _leftWheelColliders[i].sidewaysFriction;
            _sFrictionLeft[i].stiffness = _stillSidewaysFriction;
            _leftWheelColliders[i].sidewaysFriction = _sFrictionLeft[i];
        }

        for (int i = 0; i < _rightWheelColliders.Length; i++)
        {
            _sFrictionRight[i] = _rightWheelColliders[i].sidewaysFriction;
            _sFrictionRight[i].stiffness = _stillSidewaysFriction;
            _rightWheelColliders[i].sidewaysFriction = _sFrictionRight[i];
        }
    }
    
    private void Update()
    {
        // Sets angular and km/h velocity values to variables
        GetVelocities();
    }

    private void FixedUpdate()
    {
        // Physically moves the tank
        MoveTank();
        // Get our current climb angle
        GetClimbAngle();
    }

    private void LateUpdate()
    {
        SetWheelColliderFriction();
    }

    private void MoveTank()
    {
        // Gets the forward/backward velocity
        float velocityInDirection = Vector3.Dot(rigidBody.velocity, transform.forward);
        float dragCoefficient = CalculateDragCoefficient(velocityInDirection, _forwardForceCoefficient);

        // Gets the turning angular velocity
        float angularVelocityInDirection = Vector3.Dot(rigidBody.angularVelocity, transform.up);
        float dragTurnCoefficient = CalculateDragCoefficient(angularVelocityInDirection, _turningForceCoefficient);
        ;

        float turnForwardVelocityCoefficient = 2 * dragCoefficient;
        turnForwardVelocityCoefficient = turnForwardVelocityCoefficient > 1f ? 1f : turnForwardVelocityCoefficient;

        if (_grodedChecker.IsGrounded)
        {
            // Turns tank using turnTorque * dragTurnCoefficient, which is a scaled value from 0..1 depending on angular velocity
            // First part makes sure direction and rotation of turning is correct
            Vector3 turningTorqueValue = transform.up * (dragTurnCoefficient * turnForwardVelocityCoefficient *
                _turnTorque * (inputs.TurnInput * Time.fixedDeltaTime));
            if (inputs.DriveInput < 0)
            {
                rigidBody.AddTorque(turningTorqueValue * -1f);
            }
            else
            {
                rigidBody.AddTorque(turningTorqueValue);
            }

            // For responsive feel, stops tank rotation when we do not want to turn, otherwise would continue turning 
            if (inputs.TurnInput == 0)
            {
                rigidBody.AddTorque(-angularVelocityInDirection * Time.fixedDeltaTime * _turnTorque * transform.up);
            }

            // If we switch from forwards to backwards, we want tank to move responsively instead of sliding
            if (inputs.DriveInput != 0f)
            {
                if (inputs.DriveInput < 0f && velocityInDirection > 0f)
                {
                    rigidBody.AddForce(2000f * _driveTorque * Time.fixedDeltaTime * -velocityInDirection *
                        transform.forward);
                }
                else if (inputs.DriveInput > 0f && velocityInDirection < 0f)
                {
                    rigidBody.AddForce(2000f * _driveTorque * Time.fixedDeltaTime * -velocityInDirection *
                        transform.forward);
                }

                // Moves tank using driveTorque * dragCoefficient, which is a scaled value from 0..1 depending on velocity
                // Disable brakes first so we can move
                SetBrakes(0);
                SetLeftTrackTorque(inputs.DriveInput * _driveTorque * dragCoefficient);
                SetRightTrackTorque(inputs.DriveInput * _driveTorque * dragCoefficient);
            }
            else if (inputs.TurnInput != 0f)
            {
                // Moves tank slightly forward if we are only turning, otherwise friction would not allow AddTorque to function properly
                // Disable brakes (idk if necessary here)
                SetBrakes(0);
                SetLeftTrackTorque(0.01f * _driveTorque);
                SetRightTrackTorque(0.01f * _driveTorque);

                // Stops tank if we stop wanting to move forwards/backwards instead of slowly coming to a stop like a car
                rigidBody.AddForce(1000f * _driveTorque * Time.fixedDeltaTime * -velocityInDirection *
                    transform.forward);
            }
            else
            {
                // Enable brakes so that we dont slide on a slope or a hill when still
                SetBrakes(_brakeStrength);
                SetLeftTrackTorque(0f);
                SetRightTrackTorque(0f);

                // Stops tank if we stop wanting to move forwards/backwards instead of slowly coming to a stop like a car
                rigidBody.AddForce(1000f * _driveTorque * Time.fixedDeltaTime * -velocityInDirection *
                    transform.forward);
            }
        }
    }

    private float CalculateDragCoefficient(float _velocity, float _coefficient)
    {
        float normalizedVelocity = _velocity / _coefficient;
        normalizedVelocity = Math.Abs(normalizedVelocity);
        if (normalizedVelocity > 1f)
        {
            normalizedVelocity = 1f;
        }

        return (1 - normalizedVelocity);
    }

    private void GetVelocities()
    {
        velocityInKMH = rigidBody.velocity.magnitude * 3.6f;
    }
    
    // private void GetHeight()
    // {
    //     // How high off the ground are we
    //     RaycastHit hit;
    //     Ray downRay = new Ray(transform.position, -transform.up);
    //     // Debug.DrawRay(downRay.origin,downRay.direction * 100, Color.red);
    //     if (Physics.Raycast(downRay, out hit))
    //     {
    //         height = hit.distance;
    //         //Debug.Log(height.ToString());
    //     }
    // }

    private void GetClimbAngle()
    {
        _curAngle = Vector3.Angle(Vector3.up, transform.up);
    }
    
    private void SetLeftTrackTorque(float speed)
    {
        for (int i = 0; i < _leftWheelColliders.Length; i++)
        {
            if (velocityInKMH < -0.25f)
            {
                // Gives initial boost so that controls seem more responsive
                _leftWheelColliders[i].motorTorque = inputs.DriveInput < 0f ? speed * 20f : speed * 5f;
            }
            else
            {
                _leftWheelColliders[i].motorTorque = speed;
            }
        }
    }

    private void SetRightTrackTorque(float speed)
    {
        for (int i = 0; i < _rightWheelColliders.Length; i++)
        {
            if (velocityInKMH < -0.25f)
            {
                // Gives initial boost so that controls seem more responsive
                _rightWheelColliders[i].motorTorque = inputs.DriveInput < 0f ? speed * 20f : speed * 5f;
            }
            else
            {
                _rightWheelColliders[i].motorTorque = speed;
            }
        }
    }

    public void SetBrakes(float strength)
    {
        for (int i = 0; i < _leftWheelColliders.Length; i++)
        {
            _leftWheelColliders[i].brakeTorque = strength;
        }

        for (int i = 0; i < _rightWheelColliders.Length; i++)
        {
            _rightWheelColliders[i].brakeTorque = strength;
        }
    }

    private void SetWheelColliderFriction()
    {
        //If we are still or almost still reduce sideways friction
        if (rigidBody.velocity.magnitude <= 1f || _curAngle > 22f)
        {
            for (int i = 0; i < _leftWheelColliders.Length; i++)
            {
                _sFrictionLeft[i].stiffness = _stillSidewaysFriction;
                _leftWheelColliders[i].sidewaysFriction = _sFrictionLeft[i];
            }

            for (int i = 0; i < _rightWheelColliders.Length; i++)
            {
                _sFrictionRight[i].stiffness = _stillSidewaysFriction;
                _rightWheelColliders[i].sidewaysFriction = _sFrictionRight[i];
            }
        }
        else
        {
            for (int i = 0; i < _leftWheelColliders.Length; i++)
            {
                _sFrictionLeft[i].stiffness = _movementSidewaysFriction;
                _leftWheelColliders[i].sidewaysFriction = _sFrictionLeft[i];
            }

            for (int i = 0; i < _rightWheelColliders.Length; i++)
            {
                _sFrictionRight[i].stiffness = _movementSidewaysFriction;
                _rightWheelColliders[i].sidewaysFriction = _sFrictionRight[i];
            }
        }
    }
}