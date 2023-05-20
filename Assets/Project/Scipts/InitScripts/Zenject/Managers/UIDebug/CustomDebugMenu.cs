using System.ComponentModel;
using Project.Meta;
using Project.UI;
using UnityEngine;
using Zenject;

namespace Project.UIDebug
{
    public class CustomDebugMenu
    {
        [InjectOptional]
        private UISystem _uiSystem = null;

        [InjectOptional]
        private User _user;

        private bool _isUIActive = true;
        
        private LevelFlowController _levelFlowController = null;

        [Inject]
        private void Construct(LevelFlowController levelFlowController)
        {
            _levelFlowController = levelFlowController;
        }
        
        [Category("Meta")]
        public void CompleteLevel()
        {
            _levelFlowController.Complete();
        }

        [Category("Meta")]
        public async void ReloadLevel()
        {
            await _levelFlowController.Load();
        }

        [Category("Meta")]
        public async void PrevLevel()
        {
            ((ILevelData)_user).LevelIndex--;

            await _levelFlowController.Load();
        }
        
        [Category("Meta")]
        public async void ClearPlayerPrefs()
        {
            PlayerPrefs.DeleteAll();
            
            await _levelFlowController.Load();
        }
        
        [Category("UA")]
        public void ToggleUI()
        {
            _isUIActive = !_isUIActive;
            
            _uiSystem.ToggleUI(_isUIActive);
        }
    }
}