using UnityEngine;

namespace Game.Card
{
    [CreateAssetMenu(menuName = "Data/Text Change Settings", fileName = "TextChangeSettings")]
    public class TextChangeSettings : ScriptableObject
    {
        [SerializeField] private Color targetColor;

        [SerializeField] private float colorChangeTime;
        [SerializeField] private float targetSize;
        [SerializeField] private float sizeChangeTime;
        [SerializeField] private float colorRevertTime;
        [SerializeField] private float sizeRevertTime;

        public Color TargetColor => targetColor;
        public float ColorChangeTime => colorChangeTime;
        public float TargetSize => targetSize;
        public float SizeChangeTime => sizeChangeTime;
        public float ColorRevertTime => colorRevertTime;
        public float SizeRevertTime => sizeRevertTime;
    }
}