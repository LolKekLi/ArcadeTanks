namespace Project.UI
{
    public class ChildSingleTweenController : ChildTweenController
    {
        protected override void OnEnable()
        {
            base.OnEnable();

            Play();
        }
    }
}