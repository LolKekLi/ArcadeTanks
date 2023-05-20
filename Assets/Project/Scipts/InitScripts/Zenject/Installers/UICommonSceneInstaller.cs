using Project.UI;
using Project.UIDebug;
using UnityEngine;
using Zenject;

namespace Project
{
    public class UICommonSceneInstaller : MonoInstaller
    {
        [SerializeField]
        private UISystem _uiSystem = null;
        
        [SerializeField]
        private JoystickController _joystickController = null;

#if FORCE_DEBUG
        [Inject]
        private CustomDebugMenu _customDebugMenu = null;
#endif
        
        private LevelFlowController _levelFlowController = null;

        [Inject]
        private void Construct(LevelFlowController levelFlowController)
        {
            _levelFlowController = levelFlowController;
        }

        public override void InstallBindings()
        {
            Container.ParentContainers[0].Bind<UISystem>().FromInstance(_uiSystem).AsCached();
            Container.ParentContainers[0].Bind<JoystickController>().FromInstance(_joystickController).AsCached();
            Container.ParentContainers[0].Inject(_levelFlowController);
            
#if FORCE_DEBUG
            Container.ParentContainers[0].Inject(_customDebugMenu);
#endif
        }
    }    
}
