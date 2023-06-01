using System;
using System.Collections.Generic;
using PathCreation;
using Project;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBehaviourFSM
{
    private Dictionary<EnemyBehaviourType, EnemyBehaviourBase> _behaviour =
        new Dictionary<EnemyBehaviourType, EnemyBehaviourBase>();

    public EnemyBehaviourBase CurrentBehaviour
    {
        get;
        private set;
    }

    public EnemyBehaviourFSM(Transform enemyTransform, PathCreator pathCreation,
        AttackControllerBase attackControllerBase, Transform turretTransform,
        NavMeshAgent agent, ObservableSphereTriggerTrigger seePlayerTrigger, ObservableSphereTriggerTrigger attackTrigger, LayerMask playerLayerMask, float speed)
    {
        SetupAgent(agent, attackTrigger, speed);
        
        PrepareBehaviours(enemyTransform, pathCreation, attackControllerBase, turretTransform, agent, attackTrigger,
            seePlayerTrigger, ChangeState, playerLayerMask, speed);
        
        ChangeState(EnemyBehaviourType.FollowPath, null);
    }

    private void SetupAgent(NavMeshAgent navMeshAgent, ObservableSphereTriggerTrigger attackTrigger, float speed)
    {
        navMeshAgent.stoppingDistance = attackTrigger.SphereCollider.radius;
    }
    

    private void PrepareBehaviours(Transform enemyTransform, PathCreator pathCreation,
        AttackControllerBase attackControllerBase, Transform turretTransform, NavMeshAgent agent,
        ObservableSphereTriggerTrigger attackTrigger, ObservableSphereTriggerTrigger seePlayerTrigger,
        Action<EnemyBehaviourType, Transform> changeStateCallback, LayerMask playerLayerMask, float speed)
    {
        var enemyBehaviourTypes = (EnemyBehaviourType[])Enum.GetValues(typeof(EnemyBehaviourType));

        for (int i = 0; i < enemyBehaviourTypes.Length; i++)
        {
            switch (enemyBehaviourTypes[i])
            {
                case EnemyBehaviourType.FollowPath:
                    _behaviour.Add(EnemyBehaviourType.FollowPath,
                        new FollowPathBehaviour(enemyTransform, pathCreation, seePlayerTrigger, changeStateCallback, playerLayerMask, speed));
                    break;

                case EnemyBehaviourType.MoveToPlayer:
                    _behaviour.Add(EnemyBehaviourType.MoveToPlayer,
                        new MoveToPlayerBehaviour(agent, seePlayerTrigger, attackTrigger, changeStateCallback, turretTransform));
                    break;

                case EnemyBehaviourType.ReturnToPath:
                    _behaviour.Add(EnemyBehaviourType.ReturnToPath,
                        new ReturnToPathBehaviour(agent, pathCreation, seePlayerTrigger, changeStateCallback, enemyTransform, turretTransform));
                    break;

                case EnemyBehaviourType.Attack:
                    _behaviour.Add(EnemyBehaviourType.Attack,
                        new AttackPlayerBehaviour(turretTransform, attackControllerBase, attackTrigger,
                            changeStateCallback));
                    break;
            }
        }
    }

    private void ChangeState(EnemyBehaviourType enemyBehaviourType, Transform transform)
    {
        Debug.Log(enemyBehaviourType);
        CurrentBehaviour?.Exit();
        CurrentBehaviour = _behaviour[enemyBehaviourType];
        CurrentBehaviour?.SetupTargetTransform(transform);
        CurrentBehaviour?.Enter();
    }

    public void OnDied()
    {
        foreach (var enemyBehaviourBase in _behaviour)
        {
            enemyBehaviourBase.Value.Dispose();
        }

        CurrentBehaviour = null;
    }
}