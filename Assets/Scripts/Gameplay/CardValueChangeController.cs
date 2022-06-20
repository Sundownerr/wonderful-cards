using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Game.Card;
using Game.Utility;
using UniRx;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Game.Gameplay
{
    public class CardValueChangeController
    {
        private readonly Dictionary<CardView, CardController> cards = new();

        public CardValueChangeController(Button changeButton, MinMaxInt changeRange)
        {
            changeButton.onClick.AddListener(() =>
            {
                // repeat
                HandleClick().ToObservable().Take(1).Subscribe(_ =>
                {
                    HandleClick().ToObservable().Take(1).Subscribe();
                });
            });

            IEnumerator HandleClick()
            {
                var queue = new Queue<CardController>();
                var orderedCards = cards.Keys.OrderBy(x => x.transform.position.x).ToArray();

                foreach (var cardView in orderedCards)
                    queue.Enqueue(cards[cardView]);

                StartChanging(queue);

                while (queue.Count > 0)
                    yield return null;
            }

            void StartChanging(Queue<CardController> queue)
            {
                if (queue.Count == 0)
                    return;

                var next = queue.Dequeue();

                next.ChangeCompleted.Take(1)
                    .Where(_ => queue.Count > 0)
                    .Subscribe(_ => { StartChanging(queue); });

                var changeAmount = changeRange.GetRandom();

                while (changeAmount == 0)
                    changeAmount = changeRange.GetRandom();

                var cardValue = (CardParameter) Random.Range(0, 3);
                var changeType = changeAmount > 0 ? Operation.Add : Operation.Remove;

                changeAmount = Math.Abs(changeAmount);

                ChangeCardValue(next, cardValue, changeType, changeAmount);
            }

            void ChangeCardValue(CardController cardController,
                                 CardParameter cardValue,
                                 Operation changeType,
                                 int changeAmount)
            {
                switch (cardValue)
                {
                    case CardParameter.Hp:
                        if (changeType == Operation.Add)
                            cardController.AddHp(changeAmount);
                        else
                            cardController.RemoveHp(changeAmount);
                        break;
                    case CardParameter.Mana:
                        if (changeType == Operation.Add)
                            cardController.AddMana(changeAmount);
                        else
                            cardController.RemoveMana(changeAmount);
                        break;
                    case CardParameter.Attack:
                        if (changeType == Operation.Add)
                            cardController.AddAttack(changeAmount);
                        else
                            cardController.RemoveAttack(changeAmount);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        public void RemoveCard(CardView view)
        {
            cards.Remove(view);
        }

        public void AddCard(CardController cardController, CardView view)
        {
            cards.Add(view, cardController);

            cardController.Dead.Take(1).Subscribe(_ => { cards.Remove(view); });
        }

        private enum CardParameter
        {
            Hp = 0,
            Mana = 1,
            Attack = 2,
        }

        private enum Operation
        {
            Add,
            Remove,
        }
    }
}