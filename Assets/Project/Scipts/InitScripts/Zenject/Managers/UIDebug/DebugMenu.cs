using System;
using System.Collections.Generic;
using Project.Service;
using SRDebugger.Services;
using SRF.Service;
using UnityEngine;
using Zenject;

namespace Project.UIDebug
{
    public class DebugMenu : IInitializable, ITickable
    {
        private CustomDebugMenu _customDebugMenu = null;
        private HotKeyHelper _helper = null;
        
        private DiContainer _diContainer = null;
        private IAdvertisingService _advertisingService;

        [Inject]
        private void Construct(DiContainer diContainer, IAdvertisingService advertisingService)
        {
            _advertisingService = advertisingService;
            _diContainer = diContainer;
        }
        
        public void Initialize()
        {
            SRDebug.Init();
            var options = SRServiceManager.GetService<IOptionsService>();
            
            _customDebugMenu = new CustomDebugMenu();
            _diContainer.Bind<CustomDebugMenu>().FromInstance(_customDebugMenu).AsCached();
            _diContainer.Inject(_customDebugMenu);
            options.AddContainer(_customDebugMenu);
            
            SRDebug.Instance.PanelVisibilityChanged += PanelVisibilityChanged;

            InitHotKeyHelper();
        }

        public void Tick()
        {
            _helper.Tick();
        }

        private void InitHotKeyHelper()
        {
            _helper = new HotKeyHelper(new Dictionary<KeyCode, Action>
            {
                { KeyCode.Z, () =>
                {
                    _customDebugMenu.CompleteLevel();
                }},

                { KeyCode.X, () =>
                {
                    _customDebugMenu.ReloadLevel();
                }},
                
                { KeyCode.C, () =>
                {
                    _customDebugMenu.PrevLevel();
                }}
            });
        }

        private void PanelVisibilityChanged(bool isVisible)
        {
            Time.timeScale = isVisible ? 0 : 1;
            
            if (isVisible)
            {
                _advertisingService.HideBanner();
            }
            else
            {
                _advertisingService.ShowBanner();
            }
        }
    }
}