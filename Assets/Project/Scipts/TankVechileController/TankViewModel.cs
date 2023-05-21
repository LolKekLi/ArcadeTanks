using System;
using System.Linq;
using UnityEngine;

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
        }

        [SerializeField]
        private TurretPreset[] _turretPresets;

        [SerializeField]
        private BodyPreset[] _bodyPresets;

        private BodyPreset _bodyPreset;
        private TurretPreset _currentTurretPreset;
        
        [field: SerializeField]
        public GameObject TurretGameObject
        {
            get;
            private set;
        }

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

        public void Setup(TurretType turretType, BodyType bodyType)
        {
            SetupBody(bodyType);
            SetupTurret(turretType);
        }

        protected void SetupBody(BodyType bodyType)
        {
            _bodyPresets.Do(x => x.Body.SetActive(false));

            _bodyPreset = _bodyPresets.FirstOrDefault(x => x.BodyType == bodyType);
            if (_bodyPreset != null)
            {
                _bodyPreset.Body.SetActive(true);
            }
        }

        protected void SetupTurret(TurretType turretType)
        {
            _turretPresets.Do(x => x.Turret.SetActive(false));
            _currentTurretPreset = _turretPresets.FirstOrDefault(x => x.TurretType == turretType);

            if (_currentTurretPreset != null)
            {
                var turret = _currentTurretPreset.Turret;
                if (_bodyPreset.TurretTransform != null)
                {
                    turret.transform.position = _bodyPreset.TurretTransform.position;
                    Debug.Log(turret.transform.position);
                }
               
                turret.SetActive(true);
            }
        }
        
#region Debug

#if UNITY_EDITOR
        
        public void DebugSetupBody( BodyType bodyType)
        {
            SetupBody(bodyType);
        }

        public void DebugSetupTurret(TurretType turretType)
        {
            SetupTurret(turretType);
        }
        
        protected virtual void OnDrawGizmos()
        {
            if (_currentTurretPreset == null)
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