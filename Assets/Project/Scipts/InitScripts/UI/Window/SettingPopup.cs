using UnityEngine;
using UnityEngine.UI;

namespace Project.UI
{
    public class SettingPopup : Window
    {
        [SerializeField, Space]
        private Slider _mainMusickSlider;

        [SerializeField]
        private Slider _soundEffecrSlider;

        [SerializeField]
        private Button _continueButton;

        public override bool IsPopup
        {
            get =>
                false;
        }

        protected override void Start()
        {
            base.Start();

            _continueButton.onClick.AddListener(OnContinueButton, SoundType.Click);
        }

        private void OnContinueButton()
        {
            Hide(true);
        }
    }
}