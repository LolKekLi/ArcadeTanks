using UnityEngine;
using DG.Tweening;
using System;

namespace Project
{
    public abstract class BaseTweenController : MonoBehaviour
    {
        [SerializeField]
        protected float _delayBeforePlaying = 0f;

        protected DOTweenAnimation[] _animations = null;

        public DOTweenAnimation[] Animations
        {
            get => _animations;
        }

        protected virtual void Awake()
        {
            
        }

        protected virtual void OnEnable()
        {

        }

        public virtual void Play()
        {
            Action action = () =>
            {
                if (_animations.Length > 0 && !_animations[0].tween.IsPlaying())
                {
                    _animations.Do(anim =>
                    {
                        anim.tween.Rewind();
                        anim.tween.PlayForward();
                    });
                }
            };

            if (_delayBeforePlaying.AlmostEquals(0))
            {
                action.Invoke();
            }
            else
            {
                this.InvokeWithDelay(_delayBeforePlaying, () =>
                {
                    action.Invoke();
                });
            }
        }

        public virtual void PlayBackwards()
        {
            Action action = () =>
            {
                if (_animations.Length > 0 && !_animations[0].tween.IsPlaying())
                {
                    _animations.Do(anim =>
                    {
                        anim.tween.PlayBackwards();
                    });
                }
            };
            
            if (_delayBeforePlaying.AlmostEquals(0))
            {
                action.Invoke();
            }
            else
            {
                this.InvokeWithDelay(_delayBeforePlaying, () =>
                {
                    action.Invoke();
                });
            }
        }

        public virtual void Stop()
        {
            if (_animations.Length > 0)
            {
                _animations.Do(anim =>
                {
                    anim.tween.Pause();
                });
            }
        }

        public virtual void Reset()
        {
            _animations.Do(anim =>
            {
                anim.tween.Rewind();
            });
        }
    }
}