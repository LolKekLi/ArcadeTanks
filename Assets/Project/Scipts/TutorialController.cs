using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Project.UI;
using UniRx;
using UnityEngine;
using Zenject;

namespace Project
{
    public class TutorialController : MonoBehaviour
    {
        private readonly int NextCameraKey = Animator.StringToHash("NextCam");

        public static event Action<int, bool> StateChanged = delegate { };
        public static event Action TutorialStarted = delegate { };
        public static event Action TutorialEnded = delegate { };

        [SerializeField]
        private Animator _cameraAnimator;

        [Inject]
        private TutorialSettings _tutorialSettings;

        [Inject]
        private UISystem _uiSystem;

        [Inject]
        private LevelFlowController _levelFlowController;

        private ReactiveProperty<int> _currentIndex = new ReactiveProperty<int>();
        private IDisposable _subscribeLink;
        private CancellationTokenSource _lisnerToken;

        private void Awake()
        {
            if (!LocalConfig.IsFirtsGame)
            {
                _cameraAnimator.gameObject.SetActive(false);
                
            }
        }

        private void OnEnable()
        {
            _levelFlowController.Loaded += LevelFlowController_Loaded;
        }

        private void OnDisable()
        {
            _levelFlowController.Loaded -= LevelFlowController_Loaded;

            UniTaskUtil.CancelToken(ref _lisnerToken);
            _subscribeLink.Dispose();
        }

        private async void TutorialInputListner(CancellationToken refreshToken)
        {
            try
            {
                while (!refreshToken.IsCancellationRequested)
                {
                    if (Input.GetKeyDown(KeyCode.Space))
                    {
                        _currentIndex.Value++;
                    }

                    await UniTask.Yield(refreshToken);
                }
            }
            catch (OperationCanceledException e)
            {
            }
        }

        private void OnIndexChanged(int index)
        {
            
            if (index >= _tutorialSettings.TutorialStages.Length)
            {
                LocalConfig.IsFirtsGame = false;
                
                UniTaskUtil.CancelToken(ref _lisnerToken);

                _cameraAnimator.gameObject.SetActive(false);
                
                TutorialEnded();
                
                return;
            }
            
            var isChangeCam = _tutorialSettings.TutorialStages[index].IsChangeCam;

            
            StateChanged(index, isChangeCam);

            if (isChangeCam)
            {
                ChangeCam();
            }
        }

        private void ChangeCam()
        {
            _cameraAnimator.SetTrigger(NextCameraKey);
        }

        private void LevelFlowController_Loaded()
        {
            if (!LocalConfig.IsFirtsGame)
            {
                return;
            }

            TutorialStarted();

            TutorialInputListner(UniTaskUtil.RefreshToken(ref _lisnerToken));

            _subscribeLink = _currentIndex.Subscribe(OnIndexChanged);
        }
    }
}