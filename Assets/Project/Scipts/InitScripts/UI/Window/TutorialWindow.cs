using System;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using Zenject;

namespace Project.UI
{
    public class TutorialWindow : Window
    {
        [SerializeField, Space]
        private TextMeshProUGUI _tutorialText;

        [SerializeField]
        private SelfTweenController _faderTween;

        [SerializeField]
        private GameObject _commandorIcon;

        [SerializeField]
        private GameObject _textBubble;

        [Inject]
        private AudioManager _audioManager;

        [Inject]
        private TutorialSettings _tutorialSettings;

        public override bool IsPopup
        {
            get =>
                false;
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            TutorialController.StateChanged += TutorialController_StateChanged;
            TutorialController.TutorialEnded += TutorialController_TutorialEnded;
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            TutorialController.StateChanged -= TutorialController_StateChanged;
            TutorialController.TutorialEnded -= TutorialController_TutorialEnded;
        }

        protected override void OnShow()
        {
            base.OnShow();

            OnStateChanged(0, false);
        }

        private void OnStateChanged(int index, bool isNeedCameraChanged)
        {
            _tutorialText.text = _tutorialSettings.TutorialStages[index].ComandorText;

            if (isNeedCameraChanged)
            {
                _faderTween.Play();
            }
        }

        private void TutorialController_StateChanged(int index, bool isNeedCameraChanged)
        {
            OnStateChanged(index, isNeedCameraChanged);
        }

        private async void TutorialController_TutorialEnded()
        {
            _faderTween.Play();

            _textBubble.SetActive(false);
            _commandorIcon.SetActive(false);
            
            await UniTask.Delay(TimeSpan.FromSeconds(_faderTween.LongestAnimationTime));

            _uiSystem.ShowWindow<GameWindow>();
        }
    }
}