using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using Game.Card;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Game.Gameplay
{
    public class CardDragController
    {
        private readonly SerialDisposable disposable = new();
        private readonly Transform draggedCardParent;
        private readonly CardDropArea playerHandDropArea;
        private readonly CardDragSettings settings;
        private readonly CardDropArea tableDropArea;
        private TweenerCore<Quaternion, Quaternion, NoOptions> rotateTween;
        private TweenerCore<Vector3, Vector3, VectorOptions> scaleTween;

        public CardDragController(Transform draggedCardParent,
                                  CardDropArea playerHandDropArea,
                                  CardDropArea tableDropArea,
                                  CardDragSettings settings)
        {
            this.draggedCardParent = draggedCardParent;
            this.playerHandDropArea = playerHandDropArea;
            this.tableDropArea = tableDropArea;
            this.settings = settings;
        }

        public void Destroy()
        {
            disposable.Dispose();
        }

        public void AddCard(CardController controller, CardView view)
        {
            view.Pressed += CardOnPressed;
            view.Released += CardOnReleased;

            controller.Dead.Take(1).Subscribe(_ =>
            {
                view.Pressed -= CardOnPressed;
                view.Released -= CardOnReleased;
            }).AddTo(view);

            void CardOnPressed(PointerEventData obj)
            {
                view.transform.SetParent(draggedCardParent);

                var offset = view.transform.position - Input.mousePosition;

                scaleTween?.Kill();
                scaleTween = view.transform.DOScale(settings.DragStartScale, settings.DragStartScaleTime);

                rotateTween?.Kill();
                rotateTween = view.transform.DORotateQuaternion(Quaternion.identity, settings.DragStartRotationTime)
                    .SetEase(Ease.InOutBack);

                disposable.Disposable = Observable.EveryUpdate().TakeUntil(
                        Observable.FromEvent<PointerEventData>(h => view.Released += h, h => view.Released -= h))
                    .Subscribe(_ => { view.transform.position = Input.mousePosition + offset; });
            }

            void CardOnReleased(PointerEventData obj)
            {
                scaleTween?.Kill();
                scaleTween = view.transform.DOScale(settings.DragEndScale, settings.DragEndScaleTime);

                if (RectTransformUtility.RectangleContainsScreenPoint(tableDropArea.Rect, Input.mousePosition))
                {
                    tableDropArea.AddCard(controller, view);
                    return;
                }

                playerHandDropArea.AddCard(controller, view);
            }
        }
    }
}