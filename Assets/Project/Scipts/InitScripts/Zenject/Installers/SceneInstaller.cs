using Project;
using UnityEngine;
using Zenject;

public class SceneInstaller : MonoInstaller
{
    [SerializeField]
    private TankController _playerController;

    [SerializeField]
    private CameraController _cameraController;

    [Inject]
    private BulletFactory _bulletFactory;
    
    public override void InstallBindings()
    {
        var levelData = SetupLevelData();
        
        Container.Bind<LevelData>().FromInstance(levelData).AsCached();
        Container.Bind<CameraController>().FromInstance(_cameraController).AsCached();
        Container.Bind<AttackControllerFactory>().FromInstance(new AttackControllerFactory());
        Container.Inject(_bulletFactory);
    }

    private LevelData SetupLevelData()
    {
        var levelData = new LevelData();
        
        levelData.Tanks.Add(_playerController);

        return levelData;

    }
}