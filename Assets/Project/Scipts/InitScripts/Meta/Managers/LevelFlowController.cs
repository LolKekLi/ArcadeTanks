using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Project.Meta;
using Project.Service;
using Project.Settings;
using Project.UI;
using UnityEngine.SceneManagement;
using Zenject;

namespace Project
{
    public class LevelFlowController
    {
        public event Action Loaded = null;
        public event Action Started = null;
        public event Action<bool> Finished = null;

        [InjectOptional]
        private UISystem _uiSystem = null;

        [InjectOptional]
        private ILevelData _levelData;
        
        [InjectOptional]
        private IAnalyticsService _analyticsService;

        private LevelSettings _levelSettings;

        [Inject]
        public void Construct(LevelSettings levelSettings)
        {
            _levelSettings = levelSettings;
        }
        
        public void Start(Action callback = null)
        {
            _analyticsService.TrackStart();
            
            Started?.Invoke();
            
            callback?.Invoke();
            
            _uiSystem.ShowWindow<MainWindow>();
        }

        public async void Complete(Dictionary<string, object> data = null, Action callback = null)
        {
            _analyticsService.TrackFinish();

            Finished?.Invoke(true);
            
            _levelData.LevelIndex++;

            callback?.Invoke();
            
            await UniTask.Delay(TimeSpan.FromSeconds(_levelSettings.ResultDelay));
            
            _uiSystem.ShowWindow<ResultWindow>(data);
        }

        public async void Fail(Action callback = null)
        {
            _analyticsService.TrackFail();
            
            Finished?.Invoke(false);

            callback?.Invoke();

            await UniTask.Delay(TimeSpan.FromSeconds(_levelSettings.FailDelay));
            
            _uiSystem.ShowWindow<FailWindow>();
        }
        
        public async UniTask Load(Action callback = null)
        {
            await SceneManager.LoadSceneAsync(_levelSettings.GetScene);

            Loaded?.Invoke();
            
            callback?.Invoke();
            
            _uiSystem.ShowWindow<GameWindow>();
        }

        public async UniTask LoadHub()
        {
            await SceneManager.LoadSceneAsync(_levelSettings.HubScene);
            
            _uiSystem.ShowWindow<MainWindow>();

            Loaded?.Invoke();
        }
    }
}