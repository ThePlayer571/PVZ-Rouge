using System;
using System.Linq;
using QFramework;
using TPL.PVZR.Classes.DataClasses.Loot;
using TPL.PVZR.Models;
using TPL.PVZR.Systems.MazeMap;
using TPL.PVZR.Tools.SoyoFramework;

namespace TPL.PVZR.CommandEvents.MazeMap_AwardPanel
{
    public class ChooseAwardCommand : AbstractCommand
    {
        public ChooseAwardCommand(int index)
        {
            this.index = index;
        }

        private int index;

        protected override void OnExecute()
        {
            var _AwardSystem = this.GetSystem<IAwardSystem>();
            var _GameModel = this.GetModel<IGameModel>();

            if (!_AwardSystem.IsAwardAvailable.Value)
                throw new Exception($"调用了 {nameof(ChooseAwardCommand)}，但AwardSystem.IsAwardAvailable: false");
            
            //
            var awards = _AwardSystem.GetLootGroupByIndex(index);
            var inventory = _GameModel.GameData.InventoryData;

            foreach (var lootData in awards)
            {
                if (lootData.LootType == LootType.Card && !inventory.HasAvailableCardSlots()) continue;
                if (lootData.LootType == LootType.PlantBook &&
                    inventory.PlantBooks.Any(b => b.Id == lootData.PlantBookId)) continue;
                if (lootData.LootType == LootType.SeedSlot && !inventory.HasAvailableSeedSlotSlots()) continue;
                inventory.AddLootAuto(lootData);
            }
            
            _AwardSystem.ChosenAwardIndex.Value = index;
            _AwardSystem.IsAwardAvailable.Value = false;
        }
    }
}