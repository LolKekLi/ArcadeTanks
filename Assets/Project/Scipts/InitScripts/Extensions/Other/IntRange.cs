using UnityEngine;
using System;

namespace Project
{
    [Serializable]
    public class IntRange
    {
        [SerializeField]
        private int _min = 0;

        [SerializeField]
        private int _max = 0;

        public int Min
        {
            get => _min;
        }

        public int Max
        {
            get => _max;
        }

        public IntRange(int min, int max)
        {
            _min = min;
            _max = max;
        }

        public bool InRange(int value)
        {
            return value >= _min && value < _max;
        }
        
        public int InRangeValue()
        {
            return UnityEngine.Random.Range(_min, _max);
        }
    }
}