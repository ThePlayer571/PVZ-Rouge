using System.Collections.Generic;
using QFramework;
using TPL.PVZR.Architecture.Events.GamePhase;
using TPL.PVZR.Architecture.Models;
using TPL.PVZR.Architecture.Systems.PhaseSystems;
using TPL.PVZR.Gameplay.Class;
using TPL.PVZR.Gameplay.Class.Items;
using TPL.PVZR.Gameplay.Entities;

namespace TPL.PVZR.Architecture.Systems.InGame
{
    public interface IInventorySystem : ISystem
    {
        public SortedSet<Card> cardsInInventory { get; }
        public void AddLoot(Loot loot);

        public void AddLoot(List<Loot> loots);

        //
        public void AddCard(Card card);
        public void RemoveCard(Card card);
    }

    public class InventorySystem : AbstractSystem, IInventorySystem
    {
        protected override void OnInit()
        {
            _LevelModel = this.GetModel<ILevelModel>();
            cardsInInventory = new SortedSet<Card>();

            cardsInInventory.Add(CardHelper.CreateCard(PlantIdentifier.PeaShooter));
            cardsInInventory.Add(CardHelper.CreateCard(PlantIdentifier.Sunflower));
            cardsInInventory.Add(CardHelper.CreateCard(PlantIdentifier.PeaShooter));
            cardsInInventory.Add(CardHelper.CreateCard(PlantIdentifier.Sunflower));
            cardsInInventory.Add(CardHelper.CreateCard(PlantIdentifier.PotatoMine));
            cardsInInventory.Add(CardHelper.CreateCard(PlantIdentifier.SnowPea));
            cardsInInventory.Add(CardHelper.CreateCard(PlantIdentifier.Flowerpot));
            cardsInInventory.Add(CardHelper.CreateCard(PlantIdentifier.Wallnut));
            cardsInInventory.Add(CardHelper.CreateCard(PlantIdentifier.CherryBoom));
            cardsInInventory.Add(CardHelper.CreateCard(PlantIdentifier.CherryBoom));
            cardsInInventory.Add(CardHelper.CreateCard(PlantIdentifier.CherryBoom));
            cardsInInventory.Add(CardHelper.CreateCard(PlantIdentifier.CherryBoom));
            cardsInInventory.Add(CardHelper.CreateCard(PlantIdentifier.CherryBoom));
            cardsInInventory.Add(CardHelper.CreateCard(PlantIdentifier.CherryBoom));
            cardsInInventory.Add(CardHelper.CreateCard(PlantIdentifier.CherryBoom));
            cardsInInventory.Add(CardHelper.CreateCard(PlantIdentifier.CherryBoom));
            cardsInInventory.Add(CardHelper.CreateCard(PlantIdentifier.CherryBoom));
            cardsInInventory.Add(CardHelper.CreateCard(PlantIdentifier.CherryBoom));
            cardsInInventory.Add(CardHelper.CreateCard(PlantIdentifier.Flowerpot));

            RegisterEvents();
        }

        # region IInventorySystem
        
        public SortedSet<Card> cardsInInventory { get; private set; }

        public void AddCard(Card card)
        {
            cardsInInventory.Add(card);
        }

        public void RemoveCard(Card card)
        {
            cardsInInventory.Remove(card);
        }

        public void AddLoot(Loot loot)
        {
            if (loot.lootType == Loot.LootType.Card)
            {
                cardsInInventory.Add(CardHelper.CreateCard(loot.cardSO));
            }
        }

        public void AddLoot(List<Loot> loots)
        {
            foreach (var loot in loots)
            {
                AddLoot(loot);
            }
        }

        #endregion

        private ILevelModel _LevelModel;
        private void RegisterEvents()
        {
            this.RegisterEvent<OnEnterPhaseEarlyEvent>(e =>
            {
                if (e.changeToPhase is GamePhaseSystem.GamePhase.LevelExiting)
                {
                    // 清除选卡
                    foreach (var chosenCard in _LevelModel.chosenCards)
                    {
                        cardsInInventory.Remove(chosenCard);
                    }
                }
            });
        }

    }
}