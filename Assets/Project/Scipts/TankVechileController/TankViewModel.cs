using System;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Project
{
    public class TankViewModel : MonoBehaviour
    {
        [Serializable]
        public class TurretPreset
        {
            [field: SerializeField]
            public TurretType TurretType
            {
                get;
                private set;
            }

            [field: SerializeField]
            public GameObject Turret
            {
                get;
                private set;
            }

            [field: SerializeField]
            public GameObject FirePosition
            {
                get;
                private set;
            }

            [field: SerializeField]
            public ParticleSystem OnFireParticle
            {
                get;
                private set;
            }

            [field: SerializeField]
            public float FirePositionRange
            {
                get;
                private set;
            }
        }

        [Serializable]
        public class BodyPreset
        {
            [field: SerializeField]
            public BodyType BodyType
            {
                get;
                private set;
            }

            [field: SerializeField]
            public GameObject Body
            {
                get;
                private set;
            }

            [field: SerializeField]
            public Transform TurretTransform
            {
                get;
                private set;
            }

            [field: SerializeField]
            public ParticleSystem[] OnDiedParticles
            {
                get;
                private set;
            }
        }

        [SerializeField]
        private TurretPreset[] _turretPresets;

        [SerializeField]
        private BodyPreset[] _bodyPresets;
        
        [field: SerializeField]
        public GameObject TurretGameObject
        {
            get;
            private set;
        }

        [SerializeField]
        private Rigidbody _turretRb;

        [SerializeField]
        private Collider _turretColleder;

        [SerializeField]
        private float _turretPushForce;

        [SerializeField]
        private float _pushRange;

        [SerializeField]
        private float _torqueRange;
        
        
        private BodyPreset _bodyPreset;
        private TurretPreset _currentTurretPreset;

        public GameObject FirePosition
        {
            get =>
                _currentTurretPreset.FirePosition;
        }

        public float FireRange
        {
            get =>
                _currentTurretPreset.FirePositionRange;
        }

        protected virtual void Awake()
        {
            foreach (var preset in _bodyPresets)
            {
                preset.OnDiedParticles.Do(x=>x.gameObject.SetActive(false));
            }
        }

        public void Setup(TurretType turretType, BodyType bodyType)
        {
            SetupBody(bodyType);
            SetupTurret(turretType);

            if (_turretRb)
            {
                _turretRb.isKinematic = true;
                _turretRb.useGravity = false;
                _turretColleder.enabled = false;
                _turretRb.velocity = Vector3.zero;
            }
        }

        private void SetupBody(BodyType bodyType)
        {
            _bodyPresets.Do(x => x.Body.SetActive(false));

            _bodyPreset = _bodyPresets.FirstOrDefault(x => x.BodyType == bodyType);
            if (_bodyPreset != null)
            {
                _bodyPreset.Body.SetActive(true);
            }
        }

        private void SetupTurret(TurretType turretType)
        {
            _turretPresets.Do(x => x.Turret.SetActive(false));
            _currentTurretPreset = _turretPresets.FirstOrDefault(x => x.TurretType == turretType);

            if (_currentTurretPreset != null)
            {
                var turret = _currentTurretPreset.Turret;
                if (_bodyPreset.TurretTransform != null)
                {
                    turret.transform.position = _bodyPreset.TurretTransform.position;
                }

                turret.SetActive(true);
            }
        }

        public void OnDied()
        {
            _turretRb.useGravity = true;
            _turretRb.isKinematic = false;
            _turretColleder.enabled = true;
            _turretRb.velocity = Vector3.zero;

            var pushDirection = TurretGameObject.transform.up;
            pushDirection += new Vector3(Random.Range(-_pushRange, _pushRange), 0,
                Random.Range(-_pushRange, _pushRange));

            pushDirection = pushDirection.normalized;
            _turretRb.AddForce(pushDirection * _turretPushForce, ForceMode.Impulse);

            float torque = Random.Range(_torqueRange, _torqueRange);
            Vector3 torqueDirection = Random.insideUnitSphere.normalized;
            _turretRb.AddTorque(torqueDirection * torque, ForceMode.Impulse);
            
            _bodyPreset.OnDiedParticles.Do(x=>
            {
                x.gameObject.SetActive(true);
                x.Play();
            });
        }

#region Debug

#if UNITY_EDITOR

        public void DebugSetupBody(BodyType bodyType)
        {
            SetupBody(bodyType);
        }

        public void DebugSetupTurret(TurretType turretType)
        {
            SetupTurret(turretType);
        }

        protected virtual void OnDrawGizmos()
        {
            if (_currentTurretPreset == null || !_currentTurretPreset.FirePosition)
            {
                return;
            }


            var _firePositionTransform = _currentTurretPreset.FirePosition.transform;
            Gizmos.DrawWireSphere(
                _firePositionTransform.position +
                _firePositionTransform.TransformDirection(new Vector3(_currentTurretPreset.FirePositionRange, 0)),
                0.1f);
            Gizmos.DrawWireSphere(
                _firePositionTransform.position -
                _firePositionTransform.TransformDirection(new Vector3(_currentTurretPreset.FirePositionRange, 0)),
                0.1f);
        }

#endif

#endregion
    }
}