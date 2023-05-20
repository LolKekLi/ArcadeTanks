using Project.Meta;
using Project.Service;
using Project.UIDebug;
using UnityEngine;
using Zenject;

namespace Project
{
    [CreateAssetMenu(fileName = "ProjectContextInstaller", menuName = "Scriptable/Zenject/Project Context Installer")]
    public class ProjectContextInstaller : ScriptableObjectInstaller<ProjectContextInstaller>
    {
        [SerializeField]
        private ScriptableObject[] _settings = null;
        
        public override void InstallBindings()
        {
            InstallSignalBus();
            BindSettings();
            BindControllers();
            BindManagers();
            BindServices();
            BindMeta();
            BindFactories();
        }

        private void InstallSignalBus()
        {
            SignalBusInstaller.Install(Container);
        }

        private void BindSettings()
        {
            foreach (var config in _settings)
            {
                Container.BindInterfacesAndSelfTo(config.GetType()).FromInstance(config);
                Container.QueueForInject(config);
            }
        }

        private void BindControllers()
        {
            Container.Bind<LevelFlowController>().FromInstance(new LevelFlowController()).AsCached();
        }

        private void BindManagers()
        {
            BindManager(PoolManager.GetManager);
            BindManager(AudioManager.GetManager);
            BindManager(ParticlesManager.GetManager);
            BindManager(UIOverlayMessage.GetManager);
            BindManager(BulletFactory.GetManager);
            
#if FORCE_DEBUG
            Container.BindInterfacesTo<DebugMenu>().AsSingle().NonLazy();
#endif
        }

        private void BindServices()
        {
            Container.Bind<IAdvertisingService>().FromMethod(AdvertisingServiceController.GetService).AsCached();
            Container.Bind<IAnalyticsService>().FromMethod(AnalyticsServiceController.GetService).AsCached();
            Container.Bind<IInAppService>().FromMethod(InAppServiceController.GetService).AsCached();
            Container.Bind<IRateUsService>().FromMethod(RateUsServiceController.GetService).AsCached();
        }

        private void BindMeta()
        {
            Container.Bind<SkinController>().AsCached().NonLazy();
            Container.Bind<Storage>().AsCached().NonLazy();
            Container.BindInterfacesAndSelfTo<User>().AsCached();
        }

        private void BindFactories()
        {
            Container.BindFactory<IStoragable, SaveStorageDataCommand, SaveStorageDataCommand.Factory>();
            Container.BindFactory<IStoragable, LoadStorageDataCommand, LoadStorageDataCommand.Factory>();
        }

        private void BindManager<T>(T managerPrefab) where T : ZenjectManager<T>
        {
            var manager = Container.InstantiatePrefabForComponent<T>(managerPrefab);
            
            Container.Bind<T>().FromInstance(manager).AsCached();
        }

        private void BindPrefab<T>(T prefab) where T : Component
        {
            var controller = Container.InstantiatePrefabForComponent<T>(prefab);
            Container.Bind<T>()
                .FromInstance(controller).AsCached();
        }
    }
}