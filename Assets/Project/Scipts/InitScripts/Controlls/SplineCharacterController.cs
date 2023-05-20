using UnityEngine;
// NOTE: required DreamTech package
// using Dreamteck.Splines;
using Project.UI;

namespace Project
{
    public class SplineCharacterController : MonoBehaviour
    {
        // NOTE: required DreamTech package
        // [SerializeField]
        // private SplineFollower _splineFollower = null;

        [SerializeField]
        private FloatRange _borderOffset = null;
        
        [SerializeField]
        private float _swipeSensitivity = 1f;

        // [SerializeField]
        // private float _xAxisSensitivity = 1f;

        // NOTE: required DreamTech package
        // [SerializeField]
        // private SplineComputer _splineComputer = null;
        
        private Vector2 _expectedCharacterPosition = Vector2.zero;
        
        private Transform _transform = null;
        
        private void Awake()
        {
            _transform = transform;

            // NOTE: required DreamTech package
            // _splineFollower.computer = _splineComputer;
        }

        private void OnEnable()
        {
            JoystickController.Dragged += JoystickController_Dragged;
        }

        private void OnDisable()
        {
            JoystickController.Dragged -= JoystickController_Dragged;
        }

        // NOTE: required DreamTech package
        // private void FixedUpdate()
        // {
        //     _splineFollower.motion.offset = Vector2.Lerp(_splineFollower.motion.offset, _expectedCharacterPosition,
        //         Time.deltaTime * _xAxisSensitivity);
        // }

        private void JoystickController_Dragged(Vector2 delta)
        {
            _expectedCharacterPosition = _expectedCharacterPosition.ChangeX(Mathf.Clamp(
                _expectedCharacterPosition.x + delta.x * _swipeSensitivity / 18 * Mathf.Deg2Rad, _borderOffset.Min,
                _borderOffset.Max));
        }
    }
}