using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Project.UI
{
    public enum AxisOptions
    {
        Both,
        Horizontal,
        Vertical
    }

    public class Joystick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
    {
        [SerializeField]
        private float _handleRange = 1;

        [SerializeField]
        protected float _deadZone = 0;

        [SerializeField]
        private AxisOptions _axisOptions = AxisOptions.Both;

        [SerializeField]
        private bool _isSnapX = false;

        [SerializeField]
        private bool _isSnapY = false;

        [SerializeField]
        protected Image _background = null;

        [SerializeField]
        protected Image _handle = null;

        private RectTransform baseRect = null;

        protected Canvas _canvas;
        private Camera _camera;

        private Vector2 _input = Vector2.zero;

        public float Horizontal
        {
            get
            {
                return (_isSnapX) ? SnapFloat(_input.x, AxisOptions.Horizontal) : _input.x;
            }
        }

        public float Vertical
        {
            get
            {
                return (_isSnapY) ? SnapFloat(_input.y, AxisOptions.Vertical) : _input.y;
            }
        }

        public Vector2 Direction
        {
            get
            {
                return new Vector2(Horizontal, Vertical);
            }
        }

        public float HandleRange
        {
            get
            {
                return _handleRange;
            }
            set
            {
                _handleRange = Mathf.Abs(value);
            }
        }

        public float DeadZone
        {
            get
            {
                return _deadZone;
            }
            set
            {
                _deadZone = Mathf.Abs(value);
            }
        }

        public AxisOptions AxisOptions
        {
            get
            {
                return AxisOptions;
            }
            set
            {
                _axisOptions = value;
            }
        }

        public bool IsSnapX
        {
            get
            {
                return _isSnapX;
            }
            set
            {
                _isSnapX = value;
            }
        }

        public bool IsSnapY
        {
            get
            {
                return _isSnapY;
            }
            set
            {
                _isSnapY = value;
            }
        }
        
        protected virtual void Start()
        {
            HandleRange = _handleRange;
            DeadZone = _deadZone;
            baseRect = GetComponent<RectTransform>();
            _canvas = GetComponentInParent<Canvas>();
            if (_canvas == null)
                Debug.LogError("The Joystick is not placed inside a canvas");

            Vector2 center = new Vector2(0.5f, 0.5f);
            _background.rectTransform.pivot = center;
            _handle.rectTransform.anchorMin = center;
            _handle.rectTransform.anchorMax = center;
            _handle.rectTransform.pivot = center;
            _handle.rectTransform.anchoredPosition = Vector2.zero;
        }

        public virtual void OnPointerDown(PointerEventData eventData)
        {
            OnDrag(eventData);
        }

        public virtual void OnDrag(PointerEventData eventData)
        {
            _camera = null;
            if (_canvas.renderMode == RenderMode.ScreenSpaceCamera)
                _camera = _canvas.worldCamera;

            Vector2 position = RectTransformUtility.WorldToScreenPoint(_camera, _background.rectTransform.position);
            Vector2 radius = _background.rectTransform.sizeDelta / 2;
            _input = (eventData.position - position) / (radius * _canvas.scaleFactor);
            FormatInput();
            HandleInput(_input.magnitude, _input.normalized, radius, _camera);
            _handle.rectTransform.anchoredPosition = _input * radius * _handleRange;
        }

        protected virtual void HandleInput(float magnitude, Vector2 normalised, Vector2 radius, Camera cam)
        {
            if (magnitude > _deadZone)
            {
                if (magnitude > 1)
                {
                    _input = normalised;
                }
            }
            else
            {
                _input = Vector2.zero;
            }
        }

        private void FormatInput()
        {
            if (_axisOptions == AxisOptions.Horizontal)
                _input = new Vector2(_input.x, 0f);
            else if (_axisOptions == AxisOptions.Vertical)
                _input = new Vector2(0f, _input.y);
        }

        private float SnapFloat(float value, AxisOptions snapAxis)
        {
            if (value == 0)
                return value;

            if (_axisOptions == AxisOptions.Both)
            {
                float angle = Vector2.Angle(_input, Vector2.up);
                if (snapAxis == AxisOptions.Horizontal)
                {
                    if (angle < 22.5f || angle > 157.5f)
                        return 0;
                    else
                        return (value > 0) ? 1 : -1;
                }
                else if (snapAxis == AxisOptions.Vertical)
                {
                    if (angle > 67.5f && angle < 112.5f)
                        return 0;
                    else
                        return (value > 0) ? 1 : -1;
                }

                return value;
            }
            else
            {
                if (value > 0)
                    return 1;
                if (value < 0)
                    return -1;
            }

            return 0;
        }

        public virtual void OnPointerUp(PointerEventData eventData)
        {
            _input = Vector2.zero;
            _handle.rectTransform.anchoredPosition = Vector2.zero;
        }

        protected Vector2 ScreenPointToAnchoredPosition(Vector2 screenPosition)
        {
            Vector2 localPoint = Vector2.zero;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(baseRect, screenPosition, _camera, out localPoint))
            {
                Vector2 pivotOffset = baseRect.pivot * baseRect.sizeDelta;
                return localPoint - (_background.rectTransform.anchorMax * baseRect.sizeDelta) + pivotOffset;
            }

            return Vector2.zero;
        }
    }
}