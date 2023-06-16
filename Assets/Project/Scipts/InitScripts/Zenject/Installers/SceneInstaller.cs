using Project;
using Project.UI;
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

    [Inject]
    private GameWindow _gameWindow;
    
    public override void InstallBindings()
    {
        var levelData = SetupLevelData();
        
        Container.Bind<LevelData>().FromInstance(levelData).AsCached();
        Container.Bind<CameraController>().FromInstance(_cameraController).AsCached();
        Container.Bind<AttackControllerFactory>().FromInstance(new AttackControllerFactory());
        Container.Inject(_bulletFactory);
        Container.Inject(_gameWindow);
    }

    private LevelData SetupLevelData()
    {
        var levelData = new LevelData();

        levelData.TankController = _playerController;
        levelData.Tanks.Add(_playerController);

        return levelData;

    }
}