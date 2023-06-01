using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Project.UI
{
    public class PausePopup : SettingPopup
    {
        [SerializeField]
        private Button _homeButton;

        [SerializeField]
        private Button _restartButton;

        [Inject]
        private LevelFlowController _levelFlowController;


        protected override void Start()
        {
            base.Start();

            _homeButton.onClick.AddListener(OnHomeButtonClick, SoundType.Click);
            _restartButton.onClick.AddListener(OnRestartButtonClick, SoundType.Click);
        }

        protected override void OnShow()
        {
            base.OnShow();

            Time.timeScale = 0;
        }

        protected override UniTask OnHide(bool isAnimationNeeded)
        {
            Time.timeScale = 1;

            return base.OnHide(isAnimationNeeded);
        }

        private void OnHomeButtonClick()
        {
            _levelFlowController.LoadHub();
        }

        private void OnRestartButtonClick()
        {
            _levelFlowController.Load();
        }
    }
}