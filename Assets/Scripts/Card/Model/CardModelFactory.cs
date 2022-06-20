namespace Game.Card
{
    public class CardModelFactory
    {
        private readonly CardModelFactoryData modelFactoryData;

        public CardModelFactory(CardModelFactoryData modelFactoryData)
        {
            this.modelFactoryData = modelFactoryData;
        }

        public CardModel Create()
        {
            var attack = modelFactoryData.AttackValues.GetRandom();
            var hp = modelFactoryData.HpValues.GetRandom();
            var mana = modelFactoryData.ManaValues.GetRandom();
            var model = new CardModel(modelFactoryData.CardName, modelFactoryData.Description, attack, hp, mana);

            return model;
        }
    }
}