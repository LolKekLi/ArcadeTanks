using UnityEngine;
using UnityEngine.EventSystems;
using System;
using Zenject;

namespace Project.UI
{
	public class JoystickController : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
	{
        public static event Action Clicked = delegate { };
        public static event Action<Vector2> Dragged = delegate { };
        public static event Action Released = delegate { };

        [SerializeField]
        private float _minRad = 0f;
        [SerializeField]
        private float _maxRad = 1f;
        [SerializeField]
        private float _lerpPower = 1f;
        
        private Vector2 _currentPos = Vector2.zero;
        private Vector2 _prevPos = Vector2.zero;
        private Vector2 _resultDirection = Vector2.zero;

        private Vector2 _inputDirection = Vector2.zero;

        private float _speed = 0f;
        
        private bool _isHolding = false;
        private bool _isCameOut = true;

        private Camera _uiCamera = null;

        public Vector2 InputDirection => _inputDirection;
        
        public float Speed => _speed;

        [Inject]
        private void Construct(UISystem uiSystem)
        {
            _uiCamera = uiSystem.Camera;
        }

        private void Update()
        {
            Vector2 pos = GetPosition();

            var posMagnitude = pos.sqrMagnitude;
            
            if (posMagnitude > 0.01f)
            {
                _resultDirection = pos.normalized;
            }
            else
            {
                _resultDirection = Vector2.zero;
            }

            _inputDirection = Vector2.Lerp(_inputDirection, _resultDirection, _lerpPower * Time.deltaTime);
        }

        private Vector2 GetTouchPosition()
        {
            if (!_isHolding)
            {
                return Vector2.zero;
            }

            Vector2 pos = _uiCamera.ScreenToViewportPoint(_currentPos);
            return pos;
        }

        private Vector2 GetPosition()
        {
            Vector2 rawStick = Vector2.zero;

            if (_isHolding)
            {
                rawStick = GetTouchPosition() - _prevPos;

                if (rawStick.magnitude < _minRad && !_isCameOut)
                {
                    _speed = 0.0f;
                    rawStick = Vector2.zero;
                }
                else
                {
                    float f = Mathf.InverseLerp(_minRad, _maxRad, rawStick.magnitude);
                    if (f < 0.1f)
                    {
                        f = 0.1f;
                    }

                    _speed = f;

                    _isCameOut = true;
                }

                rawStick.Normalize();
            }
            else
            {
                _speed = 0;

                _isCameOut = false;

                rawStick = Vector2.zero;
            }

            return rawStick;
        }

        void IDragHandler.OnDrag(PointerEventData eventData)
        {
            _currentPos = eventData.position;

            Dragged(eventData.delta);
        }

        void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
        {
            _prevPos = _uiCamera.ScreenToViewportPoint(eventData.position);
            _currentPos = eventData.position;

            _isHolding = true;

            Clicked();
        }

        void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
        {
            Released();

            _isHolding = false;
        }
	}
}