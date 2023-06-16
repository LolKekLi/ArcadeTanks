using UnityEngine;
using UnityEngine.UI;
using Zenject;

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

        [SerializeField]
        private Button _exitButton;

        [Inject]
        private AudioManager _audioManager;

        public override bool IsPopup
        {
            get =>
                true;
        }

        protected override void Start()
        {
            base.Start();

            _continueButton.onClick.AddListener(OnContinueButton, SoundType.Click);

            _soundEffecrSlider.value = LocalConfig.VFXMusicVolumeCoef;
            _mainMusickSlider.value = LocalConfig.LoopedMusicVolumeCoef;
            
            _soundEffecrSlider.onValueChanged.AddListener(SoundEffectSliderValueChanged);
            _mainMusickSlider.onValueChanged.AddListener(MainMusicSliderValueChanged);

            if (_exitButton)
            {
                _exitButton.onClick.AddListener(()=> Application.Quit());
            }

        }

        protected override void OnShow()
        {
            base.OnShow();
            
            _soundEffecrSlider.value = LocalConfig.VFXMusicVolumeCoef;
            _mainMusickSlider.value = LocalConfig.LoopedMusicVolumeCoef;
        }

        private void SoundEffectSliderValueChanged(float sliderValue)
        {
            _audioManager.VFXMusicVolumeCoef.Value = sliderValue;
        }
        
        private void MainMusicSliderValueChanged(float sliderValue)
        {
            _audioManager.LoopedMusicVolumeCoef.Value = sliderValue;
        }

        private void OnContinueButton()
        {
            _uiSystem.ReturnToPreviousWindow();
        }
    }
}