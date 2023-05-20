using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Project.UI
{
    public class MainWindow : Window
    {
        [SerializeField]
        private Button _startButton = null;

        private LevelFlowController _levelFlowController = null;
        
        public override bool IsPopup
        {
            get => false;
        }

        [Inject]
        private void Construct(LevelFlowController levelFlowController)
        {
            _levelFlowController = levelFlowController;
        }
        
        protected override void Start()
        {
            base.Start();
            
            _startButton.onClick.AddListener(OnStartButtonClicked);
        }
        
        private void OnStartButtonClicked()
        {
            _levelFlowController.Start();
        }
    }
}