using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Project.UI
{
    public class ResultWindow : Window
    {
        private static readonly string ReceivedCoinsKey = "ReceivedCoinsKey";

        [SerializeField, Space]
        private Button _continueButton = null;

        [SerializeField]
        private TMPro.TextMeshProUGUI _scoreText;

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
            
            _continueButton.onClick.AddListener(OnContinueButtonClicked, SoundType.Click);
        }
        
        protected override void OnShow()
        {
            base.OnShow();

            var score = GetDataValue<int>(ScoreController.ScoreKey);

            _scoreText.text = $"x {score}";
        }
        
        private void OnContinueButtonClicked()
        {
            _levelFlowController.LoadHub().Forget();
        }
    }
}