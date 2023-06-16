using System;
using Project;
using UniRx;
using UnityEngine;

public class ScoreController : MonoBehaviour
{
    public static string ScoreKey = "ScoreKey";
    
    [SerializeField]
    private int _pointForeEnemyKill;
    [SerializeField]
    private int _pointForBuster;
    
    public static event Action<int> Changed = delegate { };

    private ReactiveProperty<int> _score = new ReactiveProperty<int>();

    private IDisposable _link;

    private void Start()
    {
        _link = _score.Subscribe(value =>
        {
            Changed(value);
        });
    }

    private void OnEnable()
    {
        EnemysController.EnemyDied += EnemysController_EnemyDied;
        BusterEffectController.EffectApplyed += BusterEffectController_EffectApplyed;
    }

    private void OnDisable()
    {
        _link.Dispose();
        
        EnemysController.EnemyDied -= EnemysController_EnemyDied;
        BusterEffectController.EffectApplyed -= BusterEffectController_EffectApplyed;
    }

    private void EnemysController_EnemyDied()
    {
        _score.Value += _pointForeEnemyKill;
    }

    private void BusterEffectController_EffectApplyed(EffectType obj)
    {
        _score.Value += _pointForBuster;
    }
}