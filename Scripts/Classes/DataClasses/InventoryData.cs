using System.Collections.Generic;
using System.Linq;
using QFramework;
using TPL.PVZR.Classes.DataClasses.Item.Card;
using TPL.PVZR.Classes.DataClasses.Recipe;

namespace TPL.PVZR.Classes.DataClasses
{
    public interface IInventoryData
    {
        int InitialSunPoint { get; set; }
        int SeedSlotCount { get; set; }
        BindableProperty<int> Coins { get; set; }
        IReadOnlyList<CardData> Cards { get; }

        void AddCard(CardData cardData);
        void RemoveCard(CardData cardData);
        EasyEvent<CardData> OnCardAdded { get; }
        EasyEvent<CardData> OnCardRemoved { get; }

        bool HasAvailableCardSlots(int count = 1);
        bool CanAfford(RecipeData recipe);
    }


    public class InventoryData : IInventoryData
    {
        #region Constants

        private const int MaxCardCount = 35;

        private const int DefaultInitialSunPoint = 50;

        private const int DefaultSeedSlotCount = 6;

        private const int DefaultCoins = 100;

        #endregion

        #region Properties

        public int InitialSunPoint { get; set; } = DefaultInitialSunPoint;

        public int SeedSlotCount { get; set; } = DefaultSeedSlotCount;

        public BindableProperty<int> Coins { get; set; } = new(DefaultCoins);

        private readonly BindableList<CardData> _cards = new();

        public IReadOnlyList<CardData> Cards => _cards;

        #endregion

        #region Events

        public EasyEvent<CardData> OnCardAdded { get; } = new();

        public EasyEvent<CardData> OnCardRemoved { get; } = new();

        #endregion

        #region Public Methods

        public void AddCard(CardData cardData)
        {
            if (!ValidateCardData(cardData))
                return;

            if (!CanAddCard())
            {
                UnityEngine.Debug.LogWarning($"已达到卡牌数量上限 ({MaxCardCount})，无法添加更多卡牌");
                return;
            }

            _cards.Add(cardData);
            OnCardAdded.Trigger(cardData);
        }

        public void RemoveCard(CardData cardData)
        {
            if (!ValidateCardData(cardData))
                return;

            if (_cards.Remove(cardData))
            {
                OnCardRemoved.Trigger(cardData);
            }
            else
            {
                UnityEngine.Debug.LogWarning("未找到要移除的卡牌");
            }
        }

        public bool HasAvailableCardSlots(int count = 1)
        {
            if (count <= 0)
            {
                UnityEngine.Debug.LogWarning("检查的卡牌数量必须大于0");
                return false;
            }

            return _cards.Count + count <= MaxCardCount;
        }

        public bool CanAfford(RecipeData recipe)
        {
            // 硬币
            if (recipe.consumeCoins > Coins.Value) return false;
            // 卡牌
            var consumePlants = recipe.consumeCards.Distinct();
            foreach (var cardId in consumePlants)
            {
                var consumeCount = recipe.consumeCards.Count(id => id == cardId);
                var ownedCount = _cards.Count(card => card.CardDefinition.PlantDef.Id == cardId && !card.Locked);
                if (consumeCount > ownedCount) return false;
            }
            //
            return true;
        }

        #endregion

        #region Private Methods

        private bool ValidateCardData(CardData cardData)
        {
            if (cardData == null)
            {
                UnityEngine.Debug.LogError("卡牌数据不能为空");
                return false;
            }

            return true;
        }

        private bool CanAddCard()
        {
            return _cards.Count < MaxCardCount;
        }

        #endregion
    }
}