using System.Collections.Generic;
using QFramework;
using TPL.PVZR.Classes.DataClasses.Item.Card;
using TPL.PVZR.Helpers.New.GameObjectFactory;
using TPL.PVZR.Models;
using UnityEngine;

namespace TPL.PVZR.ViewControllers.Others.UI.MazeMap
{
    public class CardPanelController : MonoBehaviour, IController
    {
        [SerializeField] private RectTransform CardSlots;
        
        private IGameModel _GameModel;
        
        private readonly Dictionary<CardData, GameObject> _cardViews = new Dictionary<CardData, GameObject>();
        

        private void Awake()
        {
            _GameModel = this.GetModel<IGameModel>();
        }

        private void Start()
        {
            // 卡牌显示 - 初始化
            foreach (var cardData in _GameModel.GameData.InventoryData.Cards)
            {
                CreateCardView(cardData);
            }

            // 卡牌显示 - 卡牌变化事件
            _GameModel.GameData.InventoryData.OnCardAdded.Register(CreateCardView)
                .UnRegisterWhenGameObjectDestroyed(this);
            _GameModel.GameData.InventoryData.OnCardRemoved
                .Register(RemoveCardView)
                .UnRegisterWhenGameObjectDestroyed(this);
        }

        private void CreateCardView(CardData cardData)
        {
            var cardView = ItemViewFactory.CreateItemView(cardData);
            cardView.transform.SetParent(CardSlots, false);
            _cardViews.Add(cardData, cardView);
        }

        private void RemoveCardView(CardData cardData)
        {
            var cardView = _cardViews[cardData];
            cardView.DestroySelf();
            _cardViews.Remove(cardData);
        }

        public IArchitecture GetArchitecture()
        {
            return PVZRouge.Interface;
        }
    }
}