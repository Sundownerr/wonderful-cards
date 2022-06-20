using System;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UniRx;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Game.Card
{
    public class CardController
    {
        private const float valueChangeInterval = 0.2f;
        private readonly CardModel model;
        private readonly CardView view;
        private TweenerCore<Quaternion, Quaternion, NoOptions> rotateTween;
        private TweenerCore<Vector3, Vector3, VectorOptions> scaleTween;

        public CardController(CardModel model, CardView view)
        {
            this.model = model;
            this.view = view;

            model.Hp.Where(x => x <= 0)
                .Take(1)
                .Subscribe(_ => { Object.Destroy(view.gameObject); });
        }

        public IObservable<int> ChangeStarted => model.Attack.Skip(1).Merge(model.Hp.Skip(1), model.Mana.Skip(1));

        public IObservable<int> ChangeCompleted =>
            model.Attack.Skip(1).Merge(model.Hp.Skip(1), model.Mana.Skip(1)).Throttle(TimeSpan.FromSeconds(0.35f));

        public IObservable<int> Dead => model.Hp.Where(x => x <= 0);

        public void Destroy()
        {
            model.Destroy();
        }

        private void Add(ReactiveProperty<int> reactiveProperty, int changeBy, float interval)
        {
            if (changeBy <= 0) return;

            Observable.Interval(TimeSpan.FromSeconds(interval))
                .StartWith(0)
                .Take(changeBy)
                .Subscribe(_ => { reactiveProperty.Value += 1; });
        }

        private void Remove(ReactiveProperty<int> reactiveProperty, int changeBy, float interval)
        {
            if (changeBy <= 0) return;

            Observable.Interval(TimeSpan.FromSeconds(interval))
                .StartWith(0)
                .Take(changeBy)
                .Subscribe(_ => { reactiveProperty.Value -= 1; });
        }

        public void AddHp(int amount)
        {
            Add(model.Hp, amount, valueChangeInterval);
        }

        public void RemoveHp(int amount)
        {
            Remove(model.Hp, amount, valueChangeInterval);
        }

        public void AddMana(int amount)
        {
            Add(model.Mana, amount, valueChangeInterval);
        }

        public void RemoveMana(int amount)
        {
            Remove(model.Mana, amount, valueChangeInterval);
        }

        public void AddAttack(int amount)
        {
            Add(model.Attack, amount, valueChangeInterval);
        }

        public void RemoveAttack(int amount)
        {
            Remove(model.Attack, amount, valueChangeInterval);
        }
    }
}