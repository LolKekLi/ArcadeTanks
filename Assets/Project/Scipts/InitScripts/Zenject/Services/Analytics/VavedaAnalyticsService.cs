using Project.Meta;
using UnityEngine;
using Zenject;

#if Vaveda
using Uni.Wrapper;
#endif

namespace Project.Service
{
    public class VavedaAnalyticsService : IAnalyticsService
    {
        [InjectOptional]
        private ILevelData _levelData;
        
        public void TrackStart()
        {
#if Vaveda
            UniWrapper.AnalyticsManager?.TrackLevelStarted(_levelData.LevelIndex + 1);
            
            DebugSafe.Log($"level_started. Level Index - {_levelData.LevelIndex + 1}");
#endif
        }

        public void TrackFinish()
        {
#if Vaveda
            UniWrapper.AnalyticsManager?.TrackLevelCompleted(_levelData.LevelIndex + 1);
            
            DebugSafe.Log($"level_finish. Level Index - {_levelData.LevelIndex + 1}.");
#endif
        }

        public void TrackFail()
        {
#if Vaveda
            UniWrapper.AnalyticsManager?.TrackLevelFailed(_levelData.LevelIndex + 1);
            
            DebugSafe.Log($"level_fail. Level Index - {_levelData.LevelIndex + 1}.");
#endif
        }
    }
}