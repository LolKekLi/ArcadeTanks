using System;
using System.Linq;
using Project.Scipts.TankVechileController;
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
        }

        [SerializeField]
        private TurretPreset[] _turretPresets;

        [SerializeField]
        private BodyPreset[] _bodyPresets;

        private TurretPreset currentTurretPreset;
        
        [field: SerializeField]
        public GameObject TurretGameObject
        {
            get;
            private set;
        }

        public GameObject FirePosition
        {
            get => currentTurretPreset.FirePosition;
        }

#region Debug

#if UNITY_EDITOR
        [SerializeField]
        private TurretType DebugTurretType;
        [SerializeField]
        private BodyType DebugBodyType;

        private void OnValidate()
        {
            DebugSetupBody(DebugBodyType);
            DebugSetupTurret(DebugTurretType);
        }
        
        private void DebugSetupBody(BodyType bodyType)
        {
            _bodyPresets.Do(x => x.Body.SetActive(false));
            
            _bodyPresets.FirstOrDefault(x => x.BodyType == bodyType).Body.SetActive(true);
        }
        
        private void DebugSetupTurret(TurretType turretType)
        {
            _turretPresets.Do(x => x.Turret.SetActive(false));
            currentTurretPreset = _turretPresets.FirstOrDefault(x => x.TurretType == turretType);

            if (currentTurretPreset != null)
            {
                currentTurretPreset.Turret.SetActive(true);
            }
        }

#endif

#endregion
       
        public void Setup(TurretType turretType, BodyType bodyType)
        {
            SetupTurret(turretType);
            SetupBody(bodyType);

        }

        private void SetupBody(BodyType bodyType)
        {
            _bodyPresets.Do(x => x.Body.SetActive(false));
            
            _bodyPresets.FirstOrDefault(x => x.BodyType == bodyType).Body.SetActive(true);
        }

        private void SetupTurret(TurretType turretType)
        {
            _turretPresets.Do(x => x.Turret.SetActive(false));
            currentTurretPreset = _turretPresets.FirstOrDefault(x => x.TurretType == turretType);

            if (currentTurretPreset != null)
            {
                currentTurretPreset.Turret.SetActive(true);
            }
        }
    }
}