using UnityEngine;

namespace Game
{
    [CreateAssetMenu(menuName = "Data/Card Drag Settings", fileName = "CardDragSettings")]
    public class CardDragSettings : ScriptableObject
    {
        [SerializeField] private float dragStartScale;
        [SerializeField] private float dragStartScaleTime;
        [SerializeField] private float dragStartRotationTime;
        [SerializeField] private float dragEndScale;
        [SerializeField] private float dragEndScaleTime;

        public float DragStartScale => dragStartScale;

        public float DragStartScaleTime => dragStartScaleTime;

        public float DragStartRotationTime => dragStartRotationTime;

        public float DragEndScale => dragEndScale;

        public float DragEndScaleTime => dragEndScaleTime;
    }
}