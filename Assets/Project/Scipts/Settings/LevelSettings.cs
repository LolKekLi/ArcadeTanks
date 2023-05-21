using System;
using System.Linq;
using Project.Meta;
using UnityEngine;
using Zenject;

namespace Project.Settings
{
    [CreateAssetMenu(fileName = "LevelSettings", menuName = "Scriptable/LevelSettings", order = 0)]
    public class LevelSettings : ScriptableObject
    {
        [Serializable]
        public class FinishCoinPreset
        {
            [field: SerializeField]
            public IntRange LevelIndex
            {
                get;
                private set;
            }

            [field: SerializeField]
            public int Coin
            {
                get;
                private set;
            }
        }
        
#if UNITY_EDITOR
        [SerializeField, Header("Test Group")]
        private bool _isTestSceneEnabled = false;

        [SerializeField, EnabledIf(nameof(_isTestSceneEnabled), true, EnabledIfAttribute.HideMode.Invisible)]
        private string _testSceneName = string.Empty;
#endif
        
        [SerializeField, Header("Main Group")]
        private string _tutorialSceneName = string.Empty;

        [field: SerializeField, Space]
        public string HubScene
        {
            get;
            private set;
        }
        

        [SerializeField]
        private string[] _levels = null;
        
        [SerializeField]
        private string[] _loopedLevels = null;

        [Header("Finish")]
        [SerializeField]
        private FinishCoinPreset[] _coinPresets = null;
        
        [field: SerializeField]
        public float ResultDelay
        {
            get;
            private set;
        }
        
        [field: SerializeField]
        public float FailDelay
        {
            get;
            private set;
        }

        [InjectOptional]
        private ILevelData _levelData;
        
        public string GetScene
        {
            get
            {
#if UNITY_EDITOR
                if (_isTestSceneEnabled)
                {
                    return _testSceneName;
                }
#endif
                int levelIndex = _levelData.LevelIndex;

                if (levelIndex == 0)
                {
                    return _tutorialSceneName;
                }
                else
                {
                    // NOTE: учитываем туториал
                    levelIndex -= 1;
                }

                if (levelIndex < _levels.Length)
                {
                    return _levels[levelIndex % _levels.Length];
                }
                else
                {
                    levelIndex -= _levels.Length;

                    return _loopedLevels[levelIndex % _loopedLevels.Length];
                }
            }
        }
        
        public int CompleteCoinCount
        {
            get
            {
                var coinPreset = _coinPresets.FirstOrDefault(pr => pr.LevelIndex.InRange(_levelData.LevelIndex - 1));
                if (coinPreset == null)
                {
                    coinPreset = _coinPresets[_coinPresets.Length - 1];
                }

                return coinPreset.Coin;
            }
        }

    }
}