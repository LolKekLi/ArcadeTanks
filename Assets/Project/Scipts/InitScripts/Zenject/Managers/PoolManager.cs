using System;
using System.Collections.Generic;
using System.Linq;
using Project.Settings;
using UnityEngine;
using Zenject;

namespace Project
{
    public class PoolManager : ZenjectManager<PoolManager>
    {
        private LevelFlowController _levelFlowController = null;
        private DiContainer _diContainer = null;
        
        private Dictionary<PooledBehaviour, Queue<PooledBehaviour>> _pooledObjects =
            new Dictionary<PooledBehaviour, Queue<PooledBehaviour>>();

        public PoolSettings PoolSettings
        {
            get;
            private set;
        }

        [Inject]
        public void Construct(DiContainer container, PoolSettings poolSettings, LevelFlowController levelFlowController)
        {
            _diContainer = container;
            PoolSettings = poolSettings;
            _levelFlowController = levelFlowController;
        }
        
        protected override void Init()
        {
            base.Init();

            PreparePool();
            
            _levelFlowController.PreLoaded += LevelFlowController_Loaded;
        }

        protected override void DeInit()
        {
            base.DeInit();
            
            _levelFlowController.PreLoaded -= LevelFlowController_Loaded;
        }

        private void PreparePool()
        {
            Prepare(PoolSettings.PooledAudio, PooledObjectType.Save, 20);
        }
        
        private PooledBehaviour PrepareObject(PooledBehaviour pooledBehaviour, PooledObjectType pooledType)
        {
            PooledBehaviour obj =
                _diContainer.InstantiatePrefabForComponent<PooledBehaviour>(pooledBehaviour, gameObject.transform);
            
            obj.Prepare(pooledType);
            obj.Init();
            obj.Free();

            return obj;
        }

        private void Prepare(PooledBehaviour pooledBehaviour, PooledObjectType pooledType, int count)
        {
            if (!_pooledObjects.ContainsKey(pooledBehaviour))
            {
                Queue<PooledBehaviour> objectPool = new Queue<PooledBehaviour>();

                for (int i = 0; i < count; i++)
                {
                    var obj = PrepareObject(pooledBehaviour, pooledType);

                    objectPool.Enqueue(obj);
                }

                _pooledObjects.Add(pooledBehaviour, objectPool);
            }
            else
            {
#if UNITY_EDITOR
                Debug.LogException(new Exception($"{pooledBehaviour} actialy contains in pooled objects"));
#endif
            }
        }

        private void CheckContains(PooledBehaviour obj)
        {
            if (!_pooledObjects.ContainsKey(obj))
            {
                Prepare(obj, PooledObjectType.FreeOnBattleEnd, 1);

#if UNITY_EDITOR
                Debug.LogError("PoolObjects with Tag " + obj + " doesn't exist ..");
#endif
            }
        }

        private void PrepareObject(PooledBehaviour obj, ref PooledBehaviour pooledObject)
        {
            if (pooledObject == null)
            {
                pooledObject = PrepareObject(obj, PooledObjectType.FreeOnBattleEnd);

                _pooledObjects[obj].Enqueue(pooledObject);

#if UNITY_EDITOR
                Debug.LogError($"prepare object: {obj}");
#endif
            }
        }

        private void SetupObject(PooledBehaviour pooledObject, Vector3 position, Quaternion rotation,
            Transform parent = null)
        {
            if (parent)
            {
                pooledObject.transform.SetParent(parent);
            }

            pooledObject.transform.SetPositionAndRotation(position, rotation);

            pooledObject.SpawnFromPool();
        }

        public T Get<T>(PooledBehaviour obj, Vector3 position, Quaternion rotation, Transform parent = null)
            where T : PooledBehaviour
        {
            CheckContains(obj);

            var pooledObject = _pooledObjects[obj].FirstOrDefault(item => item.IsFree);

            PrepareObject(obj, ref pooledObject);

            SetupObject(pooledObject, position, rotation, parent);

            return (T)pooledObject;
        }

        public T GetSpecific<T>(PooledBehaviour obj, PooledBehaviour objectInPool, Vector3 position,
            Quaternion rotation, Transform parent = null) where T : PooledBehaviour
        {
            CheckContains(obj);

            var pooledObject = _pooledObjects[obj].FirstOrDefault(item => ReferenceEquals(item, objectInPool));

            PrepareObject(obj, ref pooledObject);

            SetupObject(pooledObject, position, rotation, parent);

            return (T)pooledObject;
        }

        public void FreeObject(PooledBehaviour obj)
        {
            obj.Free();
        }

        private void DestroyAll(PooledBehaviour prefab)
        {
            if (_pooledObjects.TryGetValue(prefab, out Queue<PooledBehaviour> queue))
            {
                foreach (var entry in queue)
                {
                    Destroy(entry.gameObject);
                }

                _pooledObjects.Remove(prefab);
            }
        }
        
        private void FreeAll()
        {
            var pairsToFree = _pooledObjects;
    
            foreach (var pair in pairsToFree)
            {
                Free(pair.Key);
            }
        }

        private void Free(PooledBehaviour prefab)
        {
            if (_pooledObjects.TryGetValue(prefab, out Queue<PooledBehaviour> queue))
            {
                foreach (var entry in queue)
                {
                    if (!entry.IsFree)
                    {
                        entry.Free();
                        
                    }
                }
            }
        }

        private void LevelFlowController_Loaded()
        {
            FreeAll();
        }
    }
}