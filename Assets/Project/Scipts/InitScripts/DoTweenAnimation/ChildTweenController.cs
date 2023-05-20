using DG.Tweening;

namespace Project.UI
{
    public class ChildTweenController : BaseTweenController
    {
        protected override void Awake()
        {
            base.Awake();

            _animations = GetComponentsInChildren<DOTweenAnimation>();
        }
    }
}