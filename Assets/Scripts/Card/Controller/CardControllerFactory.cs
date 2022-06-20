namespace Game.Card
{
    public class CardControllerFactory
    {
        public CardController Create(CardModel model, CardView view)
        {
            view.SetModel(model);
            var controller = new CardController(model, view);

            return controller;
        }
    }
}