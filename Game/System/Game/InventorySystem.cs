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

            cardsInInventory.Add(ResLoader.Allocate().LoadSync<CardDataSO>("CardData_PeaShooter"));
            cardsInInventory.Add(ResLoader.Allocate().LoadSync<CardDataSO>("CardData_Sunflower"));
            cardsInInventory.Add(ResLoader.Allocate().LoadSync<CardDataSO>("CardData_PeaShooter"));
            cardsInInventory.Add(ResLoader.Allocate().LoadSync<CardDataSO>("CardData_PeaShooter"));
            cardsInInventory.Add(ResLoader.Allocate().LoadSync<CardDataSO>("CardData_PeaShooter"));
            cardsInInventory.Add(ResLoader.Allocate().LoadSync<CardDataSO>("CardData_PeaShooter"));
            cardsInInventory.Add(ResLoader.Allocate().LoadSync<CardDataSO>("CardData_PeaShooter"));
            cardsInInventory.Add(ResLoader.Allocate().LoadSync<CardDataSO>("CardData_PeaShooter"));
            cardsInInventory.Add(ResLoader.Allocate().LoadSync<CardDataSO>("CardData_PeaShooter"));
            cardsInInventory.Add(ResLoader.Allocate().LoadSync<CardDataSO>("CardData_PeaShooter"));
            cardsInInventory.Add(ResLoader.Allocate().LoadSync<CardDataSO>("CardData_PeaShooter"));
            cardsInInventory.Add(ResLoader.Allocate().LoadSync<CardDataSO>("CardData_PeaShooter"));
            cardsInInventory.Add(ResLoader.Allocate().LoadSync<CardDataSO>("CardData_PeaShooter"));
            cardsInInventory.Add(ResLoader.Allocate().LoadSync<CardDataSO>("CardData_PeaShooter"));
            cardsInInventory.Add(ResLoader.Allocate().LoadSync<CardDataSO>("CardData_PeaShooter"));
            cardsInInventory.Add(ResLoader.Allocate().LoadSync<CardDataSO>("CardData_PeaShooter"));
            cardsInInventory.Add(ResLoader.Allocate().LoadSync<CardDataSO>("CardData_PeaShooter"));
            
        }

        public List<CardDataSO> cardsInInventory { get; private set; }
    }
}