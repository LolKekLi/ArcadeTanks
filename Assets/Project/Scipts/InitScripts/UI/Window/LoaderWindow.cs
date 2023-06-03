namespace Project.UI
{
    public class LoaderWindow : Window
    {
        public override bool IsPopup
        {
            get =>
                false;
        }

        protected override void OnShow()
        {
            gameObject.SetActive(true);
        }
    }
}