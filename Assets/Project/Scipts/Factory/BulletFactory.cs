using System.Collections.Generic;
using System.Linq;
using Project;
using UnityEngine;
using Zenject;

public class BulletFactory : ZenjectManager<BulletFactory>
{
    private readonly Dictionary<TurretType, int> MaxBulletCount = new Dictionary<TurretType, int>()
    {
        { TurretType.Classic, 50 },
        { TurretType.Fire, 5},
        { TurretType.TwoGuns, 50},
    };

    [Inject]
    private LevelFlowController _levelFlowController;

    [InjectOptional]
    private LevelData _levelData = null;

    [Inject]
    private TankFireSettings _tankFireSettings;

    private Dictionary<TurretType, Bullet[]> _pooledBuulets;

    [Inject]
    private DiContainer _diContainer;
    
    private void OnEnable()
    {
        
        _levelFlowController.Loaded += LevelFlowController_Loaded;
    }

    private void OnDisable()
    {
        _levelFlowController.Loaded += LevelFlowController_Loaded;
    }

    public Bullet GetBullet(TurretType turretType)
    {
        var bullet = _pooledBuulets[turretType].FirstOrDefault(x => x.IsFree);

        if (bullet == null)
        {
            Debug.LogError($"No free bullet type {turretType}");
        }
        else
        {
            bullet.SpawnFromPool();
        }

        return bullet;
    }

    private void LevelFlowController_Loaded()
    {
        if (_pooledBuulets == null)
        {
            _pooledBuulets = new Dictionary<TurretType, Bullet[]>();
        }
        foreach (var _pooledBullet in _pooledBuulets)
        {
            _pooledBullet.Value.Do(x => x.Free());
        }

        _pooledBuulets.Clear();

        var _levelDataTanks = _levelData.Tanks;

        foreach (var bulletPreset in MaxBulletCount)
        {
            if (_levelDataTanks.FirstOrDefault(x => x.Type == bulletPreset.Key) != null)
            {
                _pooledBuulets.Add(bulletPreset.Key, PrepareBullet(bulletPreset.Key, bulletPreset.Value));
            }
        }
    }

    private Bullet[] PrepareBullet(TurretType bulletType, int bulletCount)
    {
        var _bullet = _tankFireSettings.GetBullet(bulletType);
        Bullet[] result = new Bullet[bulletCount];

        for (int i = 0; i < bulletCount; i++)
        {
            result[i] = Instantiate(_bullet, transform);
            result[i].Prepare(result[i].Type);
            result[i].Free();
        }

        return result;
    }
}