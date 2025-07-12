using System;
using QFramework;
using TPL.PVZR.Classes.DataClasses.Recipe;
using TPL.PVZR.Helpers.ClassCreator.Item;
using TPL.PVZR.Models;
using TPL.PVZR.Systems;

namespace TPL.PVZR.CommandEvents.__NewlyAdded__
{
    public struct BarterEvent
    {
        public int index;
    }


    public class BarterCommand : AbstractCommand
    {
        public BarterCommand(int index)
        {
            _index = index;
        }


        private int _index;

        protected override void OnExecute()
        {
            var _GameModel = this.GetModel<IGameModel>();
            var _StoreSystem = this.GetSystem<IStoreSystem>();

            // 检测物品是否足够
            var recipe = _StoreSystem.GetRecipeByIndex(_index);
            var inventory = _GameModel.GameData.InventoryData;
            if (recipe.used) throw new Exception($"配方已经使用过，index: {_index}");
            if (!inventory.CanAfford(recipe)) throw new Exception($"材料不足，无法兑换，index: {_index}");

            //
            this.SendEvent<BarterEvent>(new BarterEvent { index = _index });
        }
    }
}