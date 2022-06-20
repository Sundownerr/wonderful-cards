using UniRx;

namespace Game.Card
{
    public class CardModel
    {
        public CardModel(string cardName, string description, int attack, int hp, int mana)
        {
            CardName = cardName;
            Description = description;
            Attack = new ReactiveProperty<int>(attack);
            Hp = new ReactiveProperty<int>(hp);
            Mana = new ReactiveProperty<int>(mana);
        }

        public string CardName { get; set; }
        public string Description { get; set; }
        public ReactiveProperty<int> Attack { get; }
        public ReactiveProperty<int> Hp { get; }
        public ReactiveProperty<int> Mana { get; }

        public void Destroy()
        {
            Attack.Dispose();
            Hp.Dispose();
            Mana.Dispose();
        }
    }
}