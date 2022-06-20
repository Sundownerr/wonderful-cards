using Game.Utility;
using UnityEngine;

namespace Game.Card
{
    [CreateAssetMenu(menuName = "Data/Card Spawn", fileName = "CardModelFactoryData", order = 0)]
    public class CardModelFactoryData : ScriptableObject
    {
        [SerializeField] private string cardName;
        [SerializeField] private string description;
        [SerializeField] private MinMaxInt attackValues;
        [SerializeField] private MinMaxInt hpValues;
        [SerializeField] private MinMaxInt manaValues;

        public string CardName => cardName;
        public string Description => description;
        public MinMaxInt AttackValues => attackValues;
        public MinMaxInt HpValues => hpValues;
        public MinMaxInt ManaValues => manaValues;
    }
}