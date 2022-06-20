using System;
using System.Collections;
using DG.Tweening;
using Game.Card;
using UniRx;
using UnityEngine;

namespace Game.Gameplay
{
    public class CardDropArea
    {
        private const string rotateId = nameof(rotateId);
        private const string moveId = nameof(moveId);
        private readonly WaitForSeconds positionChangeInterval;

        private readonly CardDropAreaSettings settings;

        public CardDropArea(RectTransform rect,
                            CardDropAreaSettings settings)
        {
            Rect = rect;
            this.settings = settings;

            positionChangeInterval = new WaitForSeconds(settings.PositionChangeInterval);

            rect.ObserveEveryValueChanged(x => x.childCount)
                .Where(x => x > 0)
                .Subscribe(_ => { UpdateCards(); }).AddTo(rect);
        }

        public RectTransform Rect { get; }

        private int totalCards => Rect.childCount;

        public event Action<CardController, CardView> CardRemoved;
        public event Action<CardController, CardView> CardAdded;

        private void UpdateCards()
        {
            UpdateCardsPosition().ToObservable().Take(1).Subscribe().AddTo(Rect);
            UpdateCardsRotation().ToObservable().Take(1).Subscribe().AddTo(Rect);
        }

        private IEnumerator UpdateCardsRotation()
        {
            DOTween.Kill(rotateId);

            var startTwist = -1f * (settings.Twist / 2f);
            var twistPerCard = settings.Twist / totalCards;

            for (var i = 0; i < totalCards; i++)
            {
                var twistForThisCard = startTwist + i * twistPerCard;

                Rect.GetChild(i).transform
                    .DOLocalRotate(Vector3.forward * twistForThisCard, settings.PositionChangeTime)
                    .SetEase(Ease.OutBack)
                    .SetId(rotateId);

                yield return positionChangeInterval;
            }
        }

        private IEnumerator UpdateCardsPosition()
        {
            DOTween.Kill(moveId);

            var gap = Vector3.right * settings.Spacing;
            var startPosition = -gap * (totalCards / 2f);

            startPosition += gap / 2f;

            for (var i = 0; i < totalCards; i++)
            {
                var height = totalCards / 2f - Mathf.Abs(totalCards / 2f - i);
                var heightOffset = Vector3.up * (height * settings.HeightModifier);
                var targetPosition = gap * i;

                Rect.GetChild(i).DOLocalMove(startPosition + targetPosition + heightOffset,
                        settings.PositionChangeTime)
                    .SetEase(Ease.OutBack)
                    .SetId(moveId);

                yield return positionChangeInterval;
            }
        }

        public void AddCard(CardController controller, CardView view)
        {
            CardAdded?.Invoke(controller, view);
            UpdateCards();

            var targetPosition = Rect.childCount > 0
                ? Rect.GetChild(Rect.childCount - 1).position
                : Rect.transform.position;

            targetPosition += Vector3.right * settings.Spacing;

            view.transform.DOMove(targetPosition, settings.AddToAreaTime)
                .SetEase(Ease.InFlash)
                .OnComplete(() => { view.transform.SetParent(Rect, true); });

            view.transform.SetParent(Rect, true);

            var cardSiblingIndex = view.transform.GetSiblingIndex();

            view.transform.ObserveEveryValueChanged(x => x.parent)
                .Where(x => x != Rect)
                .Take(1)
                .Subscribe(_ =>
                {
                    CardRemoved?.Invoke(controller, view);
                    UpdateCards();
                }).AddTo(view);

            controller.ChangeStarted
                .TakeWhile(_ => view.transform.parent == Rect)
                .Do(_ => { view.transform.SetAsLastSibling(); })
                .Sample(controller.ChangeCompleted)
                .Subscribe(_ => { view.transform.SetSiblingIndex(cardSiblingIndex); }).AddTo(view);
        }
    }
}