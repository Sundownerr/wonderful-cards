using DG.Tweening;
using UniRx;
using UnityEngine;
using UnityEngine.Networking;

namespace Game.Card
{
    public class CardViewFactory
    {
        private readonly Transform cardsParent;
        private readonly string imageLink;

        private readonly CardView prefab;

        public CardViewFactory(CardView prefab, Transform cardsParent, string imageLink)
        {
            this.cardsParent = cardsParent;
            this.imageLink = imageLink;
            this.prefab = prefab;
        }

        public CardView Create()
        {
            var view = Object.Instantiate(prefab, cardsParent);
            view.gameObject.SetActive(false);
            view.transform.localScale = Vector3.zero;

            LoadTexture(view);

            void LoadTexture(CardView cardView)
            {
                var request = UnityWebRequestTexture.GetTexture(imageLink);
                request.timeout = 3;

                request.SendWebRequest()
                    .AsAsyncOperationObservable()
                    .Take(1)
                    .Subscribe(x =>
                    {
                        if (x.webRequest.result != UnityWebRequest.Result.Success)
                        {
                            Debug.LogError($"Can't load texture: {x.webRequest.error}");
                            Debug.LogWarning("Retrying..");
                            LoadTexture(cardView);
                            return;
                        }

                        var texture = DownloadHandlerTexture.GetContent(x.webRequest);
                        cardView.SetTexture(texture);

                        view.gameObject.SetActive(true);
                        view.transform.SetParent(cardsParent);
                        view.transform.DOScale(Vector3.one * 0.8f, 0.4f)
                            .SetEase(Ease.OutBack)
                            .SetUpdate(UpdateType.Normal);
                    }, exception => Debug.LogError(exception)).AddTo(cardView);
            }

            return view;
        }
    }
}