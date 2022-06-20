using Game.Card;
using Game.Gameplay;
using Game.Utility;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    public class Game : MonoBehaviour
    {
        [SerializeField] private CardModelFactoryData cardModelFactoryData;
        [SerializeField] private CardView cardPrefab;
        [Space] [SerializeField] private Button changeValuesButton;
        [SerializeField] private Transform pickedCardsParent;
        [SerializeField] private RectTransform playerHandDropAreaRect;
        [SerializeField] private RectTransform tableDropAreaRect;
        [Space] [SerializeField] private string imageLink;
        [SerializeField] private CardDropAreaSettings playerHandDropAreaSettings;
        [SerializeField] private CardDropAreaSettings tableDropAreaSettings;
        [SerializeField] private CardDragSettings dragSettings;
        [SerializeField] private MinMaxInt cardCount;
        [SerializeField] private MinMaxInt valueChangeRange;
        private CardDragController cardDragController;
        private CardDropArea playerHandDropArea;
        private CardValueChangeController valueChanger;

        private void Start()
        {
            var cardControllerFactory = new CardControllerFactory();
            var cardViewFactory = new CardViewFactory(cardPrefab, playerHandDropAreaRect, imageLink);
            var cardModelFactory = new CardModelFactory(cardModelFactoryData);
            playerHandDropArea = new CardDropArea(playerHandDropAreaRect, playerHandDropAreaSettings);
            playerHandDropArea.CardAdded += PlayerHandDropAreaOnCardAdded;
            playerHandDropArea.CardRemoved += PlayerHandDropAreaOnCardRemoved;

            var tableDropArea = new CardDropArea(tableDropAreaRect, tableDropAreaSettings);

            valueChanger = new CardValueChangeController(changeValuesButton, valueChangeRange);
            cardDragController = new CardDragController(pickedCardsParent, playerHandDropArea, tableDropArea,
                dragSettings);

            var randomCardCount = cardCount.GetRandom();

            for (var i = 0; i < randomCardCount; i++)
            {
                var view = cardViewFactory.Create();
                var model = cardModelFactory.Create();
                var controller = cardControllerFactory.Create(model, view);

                cardDragController.AddCard(controller, view);
                playerHandDropArea.AddCard(controller, view);
            }
        }

        private void OnDestroy()
        {
            cardDragController.Destroy();
            playerHandDropArea.CardAdded -= PlayerHandDropAreaOnCardAdded;
            playerHandDropArea.CardRemoved -= PlayerHandDropAreaOnCardRemoved;
        }

        private void PlayerHandDropAreaOnCardRemoved(CardController controller, CardView view)
        {
            valueChanger.RemoveCard(view);
        }

        private void PlayerHandDropAreaOnCardAdded(CardController controller, CardView view)
        {
            valueChanger.AddCard(controller, view);
        }
    }
}