using QFramework;
using UnityEngine;
using System;
using System.Collections.Generic;

namespace TPL.PVZR
{
    public interface IInventorySystem
    {
        public List<CardDataSO> cardsInInventory { get; }
    }
    public class InventorySystem: AbstractSystem
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
            cardsInInventory.Add(Card.GetCardData(PlantIdentifier.Wallnut));
            
            
        }

        public List<CardDataSO> cardsInInventory { get; private set; }
    }

}