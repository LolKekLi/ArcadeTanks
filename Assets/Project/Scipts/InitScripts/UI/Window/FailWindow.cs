using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Project.UI
{
    public class FailWindow : Window
    {
        [SerializeField, Space]
        private Button _homeButton = null;

        [SerializeField]
        private Button _retstartButton;

        private LevelFlowController _levelFlowController = null;

        public override bool IsPopup
        {
            get =>
                false;
        }
        
        [Inject]
        private void Construct(LevelFlowController levelFlowController)
        {
            _levelFlowController = levelFlowController;
        }
        
        protected override void Start()
        {
            base.Start();
            
            _homeButton.onClick.AddListener(OnHomeButtonClicked, SoundType.Click);
            _retstartButton.onClick.AddListener(OnRestartButtonClick, SoundType.Click);
        }

        private void OnRestartButtonClick()
        {
            _levelFlowController.Load();
        }

        private void OnHomeButtonClicked()
        {
            _levelFlowController.LoadHub();
        }
    }
}