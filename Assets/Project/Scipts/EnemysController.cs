using System;
using System.Collections.Generic;
using Project;
using UnityEngine;
using Zenject;

public class EnemysController : MonoBehaviour
{
    public static event Action EnemyDied = delegate { };
    
    private int _enemyCount;
    
    private EnemyController[] _componentInChildren;
    private Dictionary<string, object> _resultWindowData;

    [Inject]
    private LevelData _levelData;

    [Inject]
    private LevelFlowController _levelFlowController;


    private void OnEnable()
    {
        ScoreController.Changed += ScoreController_Changed;
    }

    private void OnDisable()
    {
        ScoreController.Changed += ScoreController_Changed;
    }

    private void Awake()
    {
        _componentInChildren = GetComponentsInChildren<EnemyController>();

        _enemyCount = _componentInChildren.Length;
        for (var i = 0; i < _enemyCount; i++)
        {
            _componentInChildren[i].Setup(OnEnemyDied);
            _levelData.Tanks.Add(_componentInChildren[i]);
        }

        _resultWindowData = new Dictionary<string, object>()
        {
            {ScoreController.ScoreKey, 0}
        };
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            _componentInChildren.Do(x => x.TakeDamage(100));
        }
    }

    public void OnEnemyDied()
    {
        EnemyDied();
        _enemyCount--;

        if (_enemyCount <= 0)
        {
            _levelFlowController.Complete(_resultWindowData);
        }
    }

    private void ScoreController_Changed(int value)
    {
        _resultWindowData[ScoreController.ScoreKey] = value;
    }
}