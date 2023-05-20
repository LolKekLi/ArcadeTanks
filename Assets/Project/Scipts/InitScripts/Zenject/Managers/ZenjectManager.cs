using UnityEngine;

namespace Project
{
    public abstract class ZenjectManager<T> : MonoBehaviour where T : Component
    {
        public static T GetManager
        {
            get
            {
                return Resources.Load<T>($"ZenjectManagers/{typeof(T).Name}");
            }
        }

        protected virtual void Awake()
        {
            Init();
        }

        protected virtual void Init()
        {
            
        }

        protected virtual void DeInit()
        {
            
        }
        
        private void OnDestroy()
        {
            DeInit();
        }
    }
}