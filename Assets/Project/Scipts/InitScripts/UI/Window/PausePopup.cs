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

        private CameraController _cameraController;
        private TankController _tankController;
        private AttackControllerBase _attackConreoller;


        protected override void Start()
        {
            base.Start();

            _homeButton.onClick.AddListener(OnHomeButtonClick, SoundType.Click);
            _restartButton.onClick.AddListener(OnRestartButtonClick, SoundType.Click);
        }

        protected override void OnShow()
        {
            base.OnShow();

            _tankController = GetDataValue<TankController>(InGamePauseHendler.TankControllerKey);

            _tankController.ToggleAttackController(false);
            _cameraController = _tankController.CameraController;
            _cameraController.ToggleActive(false);

            Time.timeScale = 0;
        }

        protected override UniTask OnHide(bool isAnimationNeeded)
        {
            _tankController.ToggleAttackController(true);
            _cameraController.ToggleActive(true);
            Time.timeScale = 1;

            return base.OnHide(isAnimationNeeded);
        }

        private void OnHomeButtonClick()
        {
            _levelFlowController.LoadHub().Forget();
        }

        private void OnRestartButtonClick()
        {
            _uiSystem.ReturnToPreviousWindow();
            _levelFlowController.Load(false).Forget();
        }
    }
}