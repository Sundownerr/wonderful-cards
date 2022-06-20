using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game.Utility
{
    [Serializable]
    public struct MinMaxInt
    {
        [SerializeField] private int min;
        [SerializeField] private int max;

        public int Min => min;
        public int Max => max;

        public int GetRandom() => Random.Range(min, max);
    }
}