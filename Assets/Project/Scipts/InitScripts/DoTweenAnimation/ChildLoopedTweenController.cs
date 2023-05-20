using System.Collections;
using UnityEngine;

namespace Project.UI
{
    public class ChildLoopedTweenController : ChildTweenController
    {
        [SerializeField]
        private float _delay = 1f;

        private Coroutine _animationCor = null;

        protected override void OnEnable()
        {
            base.OnEnable();

            if (_animationCor != null)
            {
                StopCoroutine(_animationCor);
                _animationCor = null;
            }

            _animationCor = StartCoroutine(AnimationCor());
        }

        private IEnumerator AnimationCor()
        {
            var waiter = new WaitForSeconds(_delay);

            while (true)
            {
                yield return waiter;

                Play();
            }
        }
    }
}