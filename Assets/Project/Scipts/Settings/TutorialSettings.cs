using System;
using UnityEngine;

namespace Project
{
    [CreateAssetMenu(menuName = "Scriptable/TutorialSettings", fileName = "TankSettings", order = 0)]
    public class TutorialSettings : ScriptableObject
    {
        [Serializable]
        public class TutorialStage
        {
            [field: SerializeField, TextArea]
            public string ComandorText
            {
                get;
                private set;
            }

            [field: SerializeField]
            public bool IsChangeCam
            {
                get;
                private set;
            }
        }

        [field: SerializeField]
        public TutorialStage[] TutorialStages
        {
            get;
            private set;
        }
    }
}