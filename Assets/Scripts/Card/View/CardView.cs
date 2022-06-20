using System;
using DG.Tweening;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game.Card
{
    public class CardView : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField] private GameObject highlight;
        [SerializeField] private Image image;
        [SerializeField] private TextChangeSettings positiveTextChangeSettings;
        [SerializeField] private TextChangeSettings negativeTextChangeSettings;
        [SerializeField] private TMP_Text attackText;
        [SerializeField] private TMP_Text descriptionText;
        [SerializeField] private TMP_Text hpText;
        [SerializeField] private TMP_Text manaText;
        [SerializeField] private TMP_Text nameText;

        private readonly CompositeDisposable bindings = new();

        private void OnDestroy()
        {
            bindings.Dispose();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            highlight.SetActive(true);
            Pressed?.Invoke(eventData);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            highlight.SetActive(false);
            Released?.Invoke(eventData);
        }

        public event Action<PointerEventData> Pressed;
        public event Action<PointerEventData> Released;

        public void SetModel(CardModel model)
        {
            bindings.Clear();

            var hpChanged = model.Hp;
            var manaChanged = model.Mana;
            var attackChanged = model.Attack;

            AddValueBinding(nameText, model.ObserveEveryValueChanged(x => x.CardName).Select(x => x.ToString()));
            AddValueBinding(descriptionText,
                model.ObserveEveryValueChanged(x => x.Description).Select(x => x.ToString()));
            AddValueBinding(attackText, attackChanged.Select(x => x.ToString()));
            AddValueBinding(hpText, hpChanged.Select(x => x.ToString()));
            AddValueBinding(manaText, manaChanged.Select(x => x.ToString()));

            AddChangeBinding(attackText, attackChanged);
            AddChangeBinding(hpText, hpChanged);
            AddChangeBinding(manaText, manaChanged);
        }

        private void AddValueBinding(TMP_Text valueText, IObservable<string> stream)
        {
            bindings.Add(stream.Subscribe(x => { valueText.text = x; }).AddTo(this));
        }

        private void AddChangeBinding(TMP_Text valueText, IObservable<int> stream)
        {
            var defaultTextColor = valueText.color;

            bindings.Add(
                stream.Pairwise()
                    .Where(x => x.Current > x.Previous)
                    .Subscribe(x => { PlaySequence(valueText, positiveTextChangeSettings, defaultTextColor); })
                    .AddTo(this));

            bindings.Add(
                stream.Pairwise()
                    .Where(x => x.Current < x.Previous)
                    .Subscribe(x => { PlaySequence(valueText, negativeTextChangeSettings, defaultTextColor); })
                    .AddTo(this));

            void PlaySequence(TMP_Text text, TextChangeSettings settings, Color defaultColor)
            {
                DOTween.Kill(text, true);

                DOTween.Sequence(text)
                    .Append(text.DOColor(settings.TargetColor, settings.ColorChangeTime))
                    .Join(text.transform.DOScale(settings.TargetSize, settings.SizeChangeTime))
                    .Append(text.DOColor(defaultColor, settings.ColorRevertTime))
                    .Join(text.transform.DOScale(1f, settings.SizeRevertTime))
                    .Play();
            }
        }

        public void SetTexture(Texture2D texture)
        {
            image.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
        }
    }
}