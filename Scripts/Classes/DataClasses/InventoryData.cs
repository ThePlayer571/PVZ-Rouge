using System.Collections.Generic;
using QFramework;
using TPL.PVZR.Classes.DataClasses.Item.Card;

namespace TPL.PVZR.Classes.DataClasses
{
    public class InventoryData
    {
        public int InitialSunPoint { get; set; } = 50;
        public int SeedSlotCount { get; set; } = 6;
        public BindableProperty<int> Coins { get; set; } = new(100);

        // Cards
        public BindableList<CardData> Cards { get; set; } = new BindableList<CardData>();
    }
}