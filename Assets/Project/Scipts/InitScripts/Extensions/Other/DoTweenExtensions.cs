using DG.Tweening;

namespace Project
{
    public static class DoTweenExtensions
    {
        public static void Play(this DOTweenAnimation animation)
        {
            animation.tween.Rewind();
            animation.tween.Play();
        }
    }
}