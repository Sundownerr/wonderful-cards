using System;
using UnityEngine;

namespace Game
{
    [CreateAssetMenu(menuName = "Data/Card Drop Area Settings", fileName = "Card Drop Area Settings")]
    [Serializable]
    public class CardDropAreaSettings : ScriptableObject
    {
        [SerializeField] private float twist;
        [SerializeField] private float spacing;
        [SerializeField] private float positionChangeInterval;
        [SerializeField] private float positionChangeTime;
        [SerializeField] private float heightModifier;
        [SerializeField] private float addToAreaTime;

        public float Spacing => spacing;
        public float Twist => twist;
        public float PositionChangeInterval => positionChangeInterval;
        public float PositionChangeTime => positionChangeTime;
        public float HeightModifier => heightModifier;

        public float AddToAreaTime => addToAreaTime;
    }
}