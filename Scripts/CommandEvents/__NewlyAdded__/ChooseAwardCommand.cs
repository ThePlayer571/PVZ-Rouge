using QFramework;
using TPL.PVZR.Classes.DataClasses.Loot;
using TPL.PVZR.Models;
using TPL.PVZR.Systems;

namespace TPL.PVZR.CommandEvents.__NewlyAdded__
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
            var AwardSystem = this.GetSystem<IAwardSystem>();
            var GameModel = this.GetModel<IGameModel>();
            var inventory = GameModel.GameData.InventoryData;

            var award = AwardSystem.GetLootGroupOfIndex(index);
            foreach (var lootData in award)
            {
                switch (lootData.LootType)
                {
                    case LootType.Card:
                        inventory.Cards.Add(lootData.CardData);
                        break;
                }
            }
        }
    }
}