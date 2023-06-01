using Cysharp.Threading.Tasks;
using Project.Meta;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Project.UI
{
    public class MainWindow : Window
    {
        [SerializeField]
        private Button _startButton = null;

        [SerializeField]
        private UIBodySelector _bodySelector;

        [SerializeField]
        private UITurretSelector _turretSelector;

        private LevelFlowController _levelFlowController = null;
        private IUser _user;
        private AudioManager _audioManager;


        public override bool IsPopup
        {
            get =>
                false;
        }

        [Inject]
        private void Construct(LevelFlowController levelFlowController, IUser user, AudioManager audioManager)
        {
            _audioManager = audioManager;
            _user = user;
            _levelFlowController = levelFlowController;
        }
      
        protected override void Start()
        {
            base.Start();

            _startButton.onClick.AddListener(OnStartButtonClicked, SoundType.Click)
                ;
            _bodySelector.CurrentType.Subscribe(type =>
            {
                _user.SetBodyType(type);
            });

            _turretSelector.CurrentType.Subscribe(type =>
            {
                _user.SetTurretType(type);
            });
        }

        protected override void OnShow()
        {
            base.OnShow();
            
            _audioManager.PlayLoopedSound(SoundType.HubScene, Vector3.zero, false);
        }

        protected override UniTask OnHide(bool isAnimationNeeded)
        {
            _audioManager.StopLoopedSound(SoundType.HubScene, false);
            
            return base.OnHide(isAnimationNeeded);
        }

        private void OnStartButtonClicked()
        {
            _levelFlowController.Load();
        }
    }
}