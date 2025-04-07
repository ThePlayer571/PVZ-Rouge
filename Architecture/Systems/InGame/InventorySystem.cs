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
            cardsInInventory = new SortedSet<Card>();

            cardsInInventory.Add(Card.CreateCard(PlantIdentifier.PeaShooter));
            cardsInInventory.Add(Card.CreateCard(PlantIdentifier.Sunflower));
            cardsInInventory.Add(Card.CreateCard(PlantIdentifier.PeaShooter));
            cardsInInventory.Add(Card.CreateCard(PlantIdentifier.Sunflower));
            cardsInInventory.Add(Card.CreateCard(PlantIdentifier.PotatoMine));
            cardsInInventory.Add(Card.CreateCard(PlantIdentifier.SnowPea));
            cardsInInventory.Add(Card.CreateCard(PlantIdentifier.Flowerpot));
            cardsInInventory.Add(Card.CreateCard(PlantIdentifier.Wallnut));
            cardsInInventory.Add(Card.CreateCard(PlantIdentifier.CherryBoom));
            cardsInInventory.Add(Card.CreateCard(PlantIdentifier.Flowerpot));
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
                cardsInInventory.Add(Card.CreateCard(loot.cardSO));
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

        private void RegisterEvents()
        {
            this.RegisterEvent<OnEnterPhaseEvent>(e =>
            {
                if (e.changeToPhase is GamePhaseSystem.GamePhase.LevelExiting)
                {
                    // 清除选卡
                    var _LevelModel = this.GetModel<ILevelModel>();
                    foreach (var chosenCard in _LevelModel.chosenCards)
                    {
                        cardsInInventory.Remove(chosenCard);
                    }
                }
            });
        }

    }
}