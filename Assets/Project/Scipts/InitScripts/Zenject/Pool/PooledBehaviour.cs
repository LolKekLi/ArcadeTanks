using System.Collections;
using UnityEngine;
using Zenject;

namespace Project
{
    public abstract class PooledBehaviour : MonoBehaviour
    {
        [SerializeField]
        private PooledObjectType _type = default;

        [SerializeField]
        private float _freeTimeout = 0f;

        public PooledObjectType Type
        {
            get => _type;
        }

        public bool IsFree
        {
            get;
            protected set;
        }

        public float FreeTimeout
        {
            get => _freeTimeout;
            
            protected set
            {
                _freeTimeout = value;
                
                if (_freeTimeout > 0)
                {
                    StartCoroutine(FreeCor());
                }
            }
        }

        public virtual void Prepare(PooledObjectType pooledType)
        {
            _type = pooledType;
        }

        public virtual void SpawnFromPool()
        {
            gameObject.SetActive(true);
            IsFree = false;

            if (_freeTimeout > 0)
            {
                StartCoroutine(FreeCor());
            }
        }

        protected virtual void BeforeReturnToPool()
        {

        }

        protected virtual void ReturnToPool()
        {
            gameObject.SetActive(false);
        }

        public virtual void Init()
        {

        }

        public void Free()
        {
            BeforeReturnToPool();
            ReturnToPool();
            
            IsFree = true;
        }

        private IEnumerator FreeCor()
        {
            yield return new WaitForSeconds(_freeTimeout);

            Free();
        }
    }
}