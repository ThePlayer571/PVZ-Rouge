using System.Collections.Generic;
using System.Linq;
using QFramework;
using TPL.PVZR.Classes.DataClasses.Loot;
using TPL.PVZR.CommandEvents.Phase;
using TPL.PVZR.Helpers.ClassCreator;
using TPL.PVZR.Models;

namespace TPL.PVZR.Systems
{
    public interface ILootChooseSystem : ISystem
    {
        LootData GetLootOfIndex(int index);
    }

    public class LootChooseSystem : AbstractSystem, ILootChooseSystem
    {
        #region 接口ILootChooseSystem实现

        private List<LootData> _lootList = new();

        public LootData GetLootOfIndex(int index)
        {
            if (index < 1 || index > _lootList.Count)
            {
                throw new System.ArgumentOutOfRangeException(nameof(index), "Index is out of range.");
            }

            return _lootList[index - 1];
        }

        #endregion

        private void WriteLoots(LootPool lootPool, int count)
        {
            _lootList.Clear();
            var loots = lootPool.GetRandomLoot(count).Select(lootInfo => LootHelper.CreateLootData(lootInfo));
            _lootList.AddRange(loots);
        }

        private void ClearLootList()
        {
            _lootList.Clear();
        }

        private ILevelModel _LevelModel;

        protected override void OnInit()
        {
            _LevelModel = this.GetModel<ILevelModel>();

            this.RegisterEvent<OnPhaseChangeEvent>(e =>
            {
                switch (e.GamePhase)
                {
                }
            });
        }
    }
}