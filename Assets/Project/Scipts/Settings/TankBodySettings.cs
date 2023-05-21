using System;
using System.Linq;
using UnityEngine;

namespace Project
{
    [CreateAssetMenu(menuName = "Scriptable/TankBodySettings", fileName = "TankBodySettings", order = 0)]
    public class TankBodySettings : ScriptableObject
    {
        [Serializable]
        public class TankMovementPreset
        {
            [field: SerializeField]
            public BodyType Type
            {
                get;
                private set;
            }

            [field: SerializeField, Space]
            public float HP
            {
                get;
                private set;
            }


            [field: SerializeField, Header("Tank Power Settings")]
            public float TurnTorque
            {
                get;
                private set;
            } = 1f;

            [field: SerializeField]
            public float DriveTorque
            {
                get;
                private set;
            } = 2f;

            [field: SerializeField]
            public float BrakeStrength
            {
                get;
                private set;
            } = 3f;

            [field: SerializeField]
            public float TurningForceCoefficient
            {
                get;
                private set;
            } = 0.7f;

            [field: SerializeField]
            public float ForwardForceCoefficient
            {
                get;
                private set;
            } = 12f;

            [field: SerializeField, Header("Tank Turning Friction")]
            public float MovementSidewaysFriction
            {
                get;
                private set;
            } = 2.2f;

            [field: SerializeField]
            public float StillSidewaysFriction
            {
                get;
                private set;
            } = 0.8f;

            [field: SerializeField, Header("Tank Physical Settings")]
            public float CenterOfMassYOffset
            {
                get;
                private set;
            } = -1.0f;

            [field: SerializeField,Header("Turret and Gun Settings")]
            public float turretTraverseSpeed
            {
                get;
                private set;
            } = 45f;

            [field: SerializeField]
            public float gunTraverseSpeed
            {
                get;
                private set;
            } = 0.001f;

            [field: SerializeField]
            public int maxGunAngle_elevation
            {
                get;
                private set;
            } = 1;

            [field: SerializeField]
            public int minGunAngle_depression
            {
                get;
                private set;
            } = 1;
        }

        [SerializeField]
        private TankMovementPreset[] _tankMovementPresets;

        public TankMovementPreset GetPresetByType(BodyType type)
        {
            var tankMovementPreset = _tankMovementPresets.FirstOrDefault(x => x.Type == type);

            if (tankMovementPreset == null)
            {
                Debug.LogError($"Нет пресета под тип {type}");
            }

            return tankMovementPreset;
        }
    }
}