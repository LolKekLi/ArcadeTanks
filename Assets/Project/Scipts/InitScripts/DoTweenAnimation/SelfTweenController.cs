using DG.Tweening;

namespace Project.UI
{
    public class SelfTweenController : BaseTweenController
    {
        public float LongestAnimationTime
        {
            get;
            private set;
        }

        protected override void Awake()
        {
            base.Awake();

            _animations = GetComponents<DOTweenAnimation>();
            
            var longestAnimation = _animations.Max(x => x.delay + x.duration);

            if (longestAnimation != null)
            {
                LongestAnimationTime = longestAnimation.delay + longestAnimation.duration;
            }
        }
    }
}