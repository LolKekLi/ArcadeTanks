using System;
using Project;
using UnityEngine;
using Zenject;

public class EnemysController : MonoBehaviour
{
    [Inject]
    private LevelData _levelData;

    [Inject]
    private LevelFlowController _levelFlowController;

    private int _enemyCount;
    private EnemyController[] _componentInChildren;

    private void Awake()
    {
        _componentInChildren = GetComponentsInChildren<EnemyController>();

        _enemyCount = _componentInChildren.Length;
        for (var i = 0; i < _enemyCount; i++)
        {
            _componentInChildren[i].Setup(OnEnemyDied);
            _levelData.Tanks.Add(_componentInChildren[i]);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            _componentInChildren.Do(x=>x.TakeDamage(100));
        }
    }

    public void OnEnemyDied()
    {
        _enemyCount--;
        
        if (_enemyCount <= 0)
        {
            _levelFlowController.Complete();
        }
    }
}
