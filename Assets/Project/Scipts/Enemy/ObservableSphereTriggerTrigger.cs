using System;
using UniRx.Triggers;
using UnityEngine;

namespace Project
{
    [RequireComponent(typeof(SphereCollider), typeof(ObservableTriggerTrigger))]
    public class ObservableSphereTriggerTrigger : MonoBehaviour
    {
        public SphereCollider SphereCollider
        {
            get;
            private set;
        }

        public ObservableTriggerTrigger ObservableTriggerTrigger
        {
            get;
            private set;
        }

        private void Awake()
        {
            SphereCollider = GetComponent<SphereCollider>();
            ObservableTriggerTrigger = GetComponent<ObservableTriggerTrigger>();
        }
    }
}