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

    private void Awake()
    {
        var componentInChildren = GetComponentsInChildren<EnemyController>();

        _enemyCount = componentInChildren.Length;
        for (var i = 0; i < _enemyCount; i++)
        {
            componentInChildren[i].Setup(OnEnemyDied);
            _levelData.Tanks.Add(componentInChildren[i]);
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
