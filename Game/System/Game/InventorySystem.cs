using QFramework;
using UnityEngine;
using System;
using System.Collections.Generic;

namespace TPL.PVZR
{
    public interface IInventorySystem:ISystem,IInLevelSystem
    {
        public List<CardDataSO> cardsInInventory { get; }
        public void AddLoot(LootData loot);
        public void AddLoot(List<LootData> loots);
    }
    public class InventorySystem: AbstractSystem,IInventorySystem
    {
        protected override void OnInit()
        {
            cardsInInventory = new List<CardDataSO>();

            cardsInInventory.Add(Card.GetCardData(PlantIdentifier.PeaShooter));
            cardsInInventory.Add(Card.GetCardData(PlantIdentifier.Sunflower));
            cardsInInventory.Add(Card.GetCardData(PlantIdentifier.PeaShooter));
            cardsInInventory.Add(Card.GetCardData(PlantIdentifier.Sunflower));
            cardsInInventory.Add(Card.GetCardData(PlantIdentifier.PotatoMine));
            cardsInInventory.Add(Card.GetCardData(PlantIdentifier.SnowPea));
            cardsInInventory.Add(Card.GetCardData(PlantIdentifier.Flowerpot));
            cardsInInventory.Add(Card.GetCardData(PlantIdentifier.Wallnut));
            cardsInInventory.Add(Card.GetCardData(PlantIdentifier.CherryBoom));
            cardsInInventory.Add(Card.GetCardData(PlantIdentifier.Flowerpot));
        }
        
        

        public List<CardDataSO> cardsInInventory { get; private set; }
        
        public void AddLoot(LootData loot)
        {
            if (loot.lootType == LootData.LootType.Card)
            {
                cardsInInventory.Add( loot.cardData);
            }
        }

        public void AddLoot(List<LootData> loots)
        {
            foreach (LootData loot in loots)
            {
                AddLoot(loot);
            }
        }

        public void OnEndGameplay()
        {
            var _LevelModel = this.GetModel<ILevelModel>();
            foreach (var chosenCard in _LevelModel.chosenCards)
            {
                foreach (var cardInInventory in cardsInInventory)
                {
                    if (cardInInventory.seedData.plantIdentifier == chosenCard.cardData.seedData.plantIdentifier)
                    {
                        cardsInInventory.Remove(cardInInventory);
                        break;
                    }
                }
            }
        }
    }

}