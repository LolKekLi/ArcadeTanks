using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;
using Cysharp.Threading.Tasks;
using Project.Settings;

namespace Project
{
    public class StartupManager : MonoBehaviour
    {
        private const string MainUI = "UICommon";
        
        [SerializeField]
        private CanvasGroup[] _canvasGroups = null;

        private LoadingSettings _loadingSettings;
        private LevelFlowController _levelFlowController = null;

        [Inject]
        public void Construct(LoadingSettings loadingSettings, LevelFlowController levelFlowController)
        {
            _loadingSettings = loadingSettings;
            _levelFlowController = levelFlowController;
        }
        
        private async void Start()
        {
            DontDestroyOnLoad(gameObject);

            SetupProjectSettings();

            await LoadScene();
            await FadeAsync();

            Destroy(gameObject);
        }

        private void SetupProjectSettings()
        {
            Application.targetFrameRate = 60;
            QualitySettings.vSyncCount = 0;
            Screen.orientation = ScreenOrientation.Portrait;
        }
        
        private async UniTask LoadScene()
        {
            var uiWaiter = SceneManager.LoadSceneAsync(MainUI);
            float time = 0f;

           
            while (uiWaiter.progress < 1)
            {
                
                await UniTask.Yield();

                time += Time.deltaTime;
            }
            
            var levelWaiter = _levelFlowController.LoadHub(false);

            while (levelWaiter.Status == UniTaskStatus.Pending || time < _loadingSettings.LoadingTime)
            {
                
                await UniTask.Yield();

                time += Time.deltaTime;
            }

        }
        
        private async UniTask FadeAsync()
        {
            foreach (var canvasGroup in _canvasGroups)
            {
                await UniTaskExtensions.Lerp(progress =>
                    {
                        canvasGroup.alpha = 1 - progress;
                    }, 
                    _loadingSettings.FadeTime, _loadingSettings.FadeCurve, CancellationToken.None); 
            }
        }
    }
}