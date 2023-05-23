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
        

        public override bool IsPopup
        {
            get =>
                false;
        }

        [Inject]
        private void Construct(LevelFlowController levelFlowController, IUser user)
        {
            _user = user;
            _levelFlowController = levelFlowController;
        }
      
        protected override void Start()
        {
            base.Start();

            _startButton.onClick.AddListener(OnStartButtonClicked);
            _bodySelector.CurrentType.Subscribe(type =>
            {
                _user.SetBodyType(type);
            });

            _turretSelector.CurrentType.Subscribe(type =>
            {
                _user.SetTurretType(type);
            });
        }
        
        private void OnStartButtonClicked()
        {
            _levelFlowController.Load();
        }
    }
}