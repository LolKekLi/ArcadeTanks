using DG.Tweening;

namespace Project.UI
{
    public class SelfSingleStartTweenController : SelfSingleTweenController
    {
        protected override void OnEnable()
        {
            base.OnEnable();

            this.InvokeWithFrameDelay(() =>
            {
                Play();
            });
        }

        protected void OnDisable()
        {
            if (_animations.Length > 0 && _animations[0].tween != null && !_animations[0].tween.IsPlaying())
            {
                _animations.Do(anim =>
                {
                    anim.tween.Rewind();
                });
            }
        }
    }
}