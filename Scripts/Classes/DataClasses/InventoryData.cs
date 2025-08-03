using System.Collections.Generic;
using System.Linq;
using QFramework;
using TPL.PVZR.Classes.DataClasses.Item.Card;
using TPL.PVZR.Classes.DataClasses.Item.PlantBook;
using TPL.PVZR.Classes.DataClasses.Loot;
using TPL.PVZR.Classes.DataClasses.Recipe;
using TPL.PVZR.Classes.InfoClasses;
using TPL.PVZR.Helpers.New;
using TPL.PVZR.Helpers.New.ClassCreator;
using TPL.PVZR.Helpers.New.DataReader;
using TPL.PVZR.Tools.Save;

namespace TPL.PVZR.Classes.DataClasses
{
    public interface IInventoryData : ISavable<InventorySaveData>
    {
        int MaxSeedSlotCount { get; }
        int MaxCardCount { get; }
        int InitialSunPoint { get; set; }
        IReadonlyBindableProperty<int> SeedSlotCount { get; }
        BindableProperty<int> Coins { get; set; }
        IReadOnlyList<CardData> Cards { get; }
        IReadOnlyList<PlantBookData> PlantBooks { get; }

        void AddLootAuto(LootData lootData);
        void AddCard(CardData cardData);
        void RemoveCard(CardData cardData);
        void SortCards();
        void AddPlantBook(PlantBookData plantBookData);
        void RemovePlantBook(PlantBookData plantBookData);
        void AddSeedSlot();
        EasyEvent<CardData> OnCardAdded { get; }
        EasyEvent<CardData> OnCardRemoved { get; }
        EasyEvent<int> OnCardCountChange { get; }
        EasyEvent<PlantBookData> OnPlantBookAdded { get; }
        EasyEvent<PlantBookData> OnPlantBookRemoved { get; }

        bool HasAvailableCardSlots(int count = 1);
        bool HasPlantBook(PlantBookId plantBookId);
        bool HasAvailableSeedSlotSlots(int count = 1);
        bool CanAfford(RecipeData recipe);
        bool HasTradableCard(PlantId plantId);
    }


    public class InventoryData : IInventoryData
    {
        #region Constants

        public int MaxCardCount => 35;

        public int MaxSeedSlotCount => 10;

        private const int DefaultInitialSunPoint = 100;

        private const int DefaultSeedSlotCount = 6;

        private const int DefaultCoins = 50;

        #endregion

        #region Properties

        public int InitialSunPoint { get; set; } = DefaultInitialSunPoint;

        private BindableProperty<int> _seedSlotCount = new(DefaultSeedSlotCount);

        public BindableProperty<int> Coins { get; set; } = new(DefaultCoins);

        private readonly List<CardData> _cards = new();
        private readonly List<PlantBookData> _plantBooks = new();

        public IReadOnlyList<CardData> Cards => _cards;
        public IReadOnlyList<PlantBookData> PlantBooks => _plantBooks;

        public IReadonlyBindableProperty<int> SeedSlotCount => _seedSlotCount;

        #endregion

        #region Events

        public EasyEvent<CardData> OnCardAdded { get; } = new();
        public EasyEvent<CardData> OnCardRemoved { get; } = new();
        public EasyEvent<int> OnCardCountChange { get; } = new();
        public EasyEvent<PlantBookData> OnPlantBookAdded { get; } = new();
        public EasyEvent<PlantBookData> OnPlantBookRemoved { get; } = new();

        #endregion

        #region Public Methods

        public void AddLootAuto(LootData lootData)
        {
            switch (lootData.LootType)
            {
                case LootType.Card:
                    var cardData = ItemCreator.CreateCardData(lootData.PlantId.ToDef(), lootData.Locked);
                    this.AddCard(cardData);
                    $" 添加卡牌成功: {lootData.PlantId}, 锁定状态: {lootData.Locked}".LogInfo();
                    break;
                case LootType.PlantBook:
                    var plantBookData = ItemCreator.CreatePlantBookData(lootData.PlantBookId);
                    this.AddPlantBook(plantBookData);
                    break;
                case LootType.Coin:
                    this.Coins.Value += lootData.CoinAmount;
                    break;
                case LootType.SeedSlot:
                    this.AddSeedSlot();
                    break;
            }
        }

        public void AddCard(CardData cardData)
        {
            if (cardData == null)
            {
                "卡牌数据不能为空".LogError();
                return;
            }

            if (!CanAddCard())
            {
                $"已达到卡牌数量上限 ({MaxCardCount})，无法添加更多卡牌".LogWarning();
                return;
            }

            _cards.Add(cardData);
            OnCardAdded.Trigger(cardData);
            OnCardCountChange.Trigger(_cards.Count);
        }

        public void RemoveCard(CardData cardData)
        {
            if (cardData == null)
            {
                "卡牌数据不能为空".LogError();
                return;
            }

            if (_cards.Remove(cardData))
            {
                OnCardRemoved.Trigger(cardData);
                OnCardCountChange.Trigger(_cards.Count);
            }
            else
            {
                "未找到要移除的卡牌".LogWarning();
            }
        }

        public void AddPlantBook(PlantBookData plantBookData)
        {
            // 异常处理
            if (plantBookData == null)
            {
                "植物秘籍数据不能为空".LogError();
                return;
            }

            if (PlantBooks.Any(book => book.PlantId == plantBookData.PlantId))
            {
                $"已存在相同的秘籍，无法添加，id: {plantBookData.PlantId}".LogError();
                return;
            }

            // 修改数据
            _plantBooks.Add(plantBookData);

            var newDefinition =
                PlantConfigReader.GetCardDefinition(new PlantDef(plantBookData.PlantId, plantBookData.Variant));
            foreach (var cardData in _cards.Where(cardData =>
                         cardData.CardDefinition.PlantDef.Id == plantBookData.PlantId))
            {
                cardData.CardDefinition = newDefinition;
            }

            // 事件
            OnPlantBookAdded.Trigger(plantBookData);
        }

        public void RemovePlantBook(PlantBookData plantBookData)
        {
            if (plantBookData == null)
            {
                "植物秘籍数据不能为空".LogError();
                return;
            }

            if (_plantBooks.Remove(plantBookData))
            {
                OnPlantBookRemoved.Trigger(plantBookData);
            }
            else
            {
                "未找到要移除的植物秘籍".LogWarning();
            }
        }


        public void AddSeedSlot()
        {
            if (_seedSlotCount.Value >= MaxSeedSlotCount)
            {
                $"尝试添加SeedSlot，但是已经达到上限".LogError();
                return;
            }

            _seedSlotCount.Value++;
        }

        public void SortCards()
        {
            if (_cards.Count <= 1) return;

            var sortedCards = _cards
                .OrderByDescending(card => card.Locked) // Locked的在前面
                .ThenBy(card => card.CardDefinition.PlantDef.Id) // 按PlantId从小到大
                .ToList();

            // 清空原列表并重新添加排序后的卡牌
            _cards.Clear();
            foreach (var card in sortedCards)
            {
                _cards.Add(card);
            }
        }

        public bool HasAvailableCardSlots(int count = 1)
        {
            if (count <= 0) return true;

            return _cards.Count + count <= MaxCardCount;
        }

        public bool HasPlantBook(PlantBookId plantBookId)
        {
            return _plantBooks.Any(book => book.Id == plantBookId);
        }

        public bool HasAvailableSeedSlotSlots(int count = 1)
        {
            if (count <= 0) return true;

            return _seedSlotCount.Value + count <= MaxSeedSlotCount;
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

            // 背包空位
            // todo 可能不是换卡牌
            if (!HasAvailableCardSlots(1 - recipe.consumeCards.Count)) return false;

            //
            return true;
        }

        public bool HasTradableCard(PlantId plantId)
        {
            return _cards.Any(cardData => !cardData.Locked && cardData.CardDefinition.PlantDef.Id == plantId);
        }

        #endregion

        #region Private Methods

        private bool CanAddCard()
        {
            return _cards.Count < MaxCardCount;
        }

        #endregion


        public InventoryData()
        {
        }

        public InventoryData(InventorySaveData saveData)
        {
            InitialSunPoint = saveData.initialSunpoint;
            _seedSlotCount.Value = saveData.seedSlotCount;
            Coins.Value = saveData.coins;
            _cards.AddRange(saveData.cards.Select(cardSaveData => new CardData(cardSaveData)));
            _plantBooks.AddRange(saveData.plantBooks.Select(bookSaveData => new PlantBookData(bookSaveData)));
        }

        public InventorySaveData ToSaveData()
        {
            return new InventorySaveData
            {
                initialSunpoint = InitialSunPoint,
                seedSlotCount = _seedSlotCount.Value,
                coins = Coins.Value,
                cards = _cards.Select(card => card.ToSaveData()).ToList(),
                plantBooks = _plantBooks.Select(book => book.ToSaveData()).ToList()
            };
        }
    }
}