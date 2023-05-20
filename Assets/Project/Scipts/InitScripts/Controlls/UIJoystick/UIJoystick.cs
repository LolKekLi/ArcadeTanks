using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Project.UI
{
    public class UIJoystick : Joystick
    {
        [Header("Following Settings")]
        [SerializeField]
        private bool _isFollowing = false;
        [SerializeField] 
        private float _moveThreshold = 1f;

        [Header("Transition")]
        [SerializeField]
        private float _transitionTime = .2f;

        [SerializeField]
        private float _activeAlpha = 0f;

        [SerializeField]
        private float _disabledAlpha = 0f;
        
        private Vector2 _defaultPosition = Vector2.zero;

        private Coroutine _transitionCor = null;

        protected override void Start()
        {
            base.Start();
            
            _defaultPosition = _background.rectTransform.anchoredPosition;
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            base.OnPointerDown(eventData);
            
            _background.rectTransform.anchoredPosition = eventData.position / _canvas.scaleFactor;
            _handle.rectTransform.anchoredPosition = Vector2.zero;

            if (_transitionCor != null)
            {
                StopCoroutine(_transitionCor);
                _transitionCor = null;
            }

            _transitionCor = StartCoroutine(TransitionCor(_activeAlpha));
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            base.OnPointerUp(eventData);
            
            _background.rectTransform.anchoredPosition = _defaultPosition;
            
            if (_transitionCor != null)
            {
                StopCoroutine(_transitionCor);
                _transitionCor = null;
            }

            _transitionCor = StartCoroutine(TransitionCor(_disabledAlpha));
        }
        
        protected override void HandleInput(float magnitude, Vector2 normalised, Vector2 radius, Camera cam)
        {
            if (magnitude > _moveThreshold && _isFollowing)
            {
                Vector2 difference = normalised * (magnitude - _moveThreshold) * radius;
                _background.rectTransform.anchoredPosition += difference;
            }
            
            base.HandleInput(magnitude, normalised, radius, cam);
        }
        
        private IEnumerator TransitionCor(float targetAlpha)
        {
            float time = 0f;
            float progress = 0f;

            float backgroundStartAlpha = _background.color.a;
            float handleStartAlpha = _handle.color.a;
            Color color = default;
            
            while (time < _transitionTime)
            {
                yield return null;

                time += Time.deltaTime;
                progress = time / _transitionTime;

                color = _background.color;
                color.a = Mathf.Lerp(backgroundStartAlpha, targetAlpha, progress);
                _background.color = color;
                
                color = _handle.color;
                color.a = Mathf.Lerp(handleStartAlpha, targetAlpha, progress);
                _handle.color = color;
            }
        }
    }
}