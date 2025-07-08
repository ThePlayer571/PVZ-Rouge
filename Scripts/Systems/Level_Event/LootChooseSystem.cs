using System.Collections.Generic;
using QFramework;
using TPL.PVZR.Classes.DataClasses.Loot;
using TPL.PVZR.CommandEvents.Phase;
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

        private void WriteLoots()
        {
        }

        private void ClearLootList()
        {
            _lootList.Clear();
        }


        protected override void OnInit()
        {
            this.RegisterEvent<OnPhaseChangeEvent>(e =>
            {
                switch (e.GamePhase)
                {
                    case GamePhase.ChooseLoots:
                        switch (e.PhaseStage)
                        {
                            case PhaseStage.EnterEarly:
                                "生成数据".LogInfo();
                                break;
                        }

                        break;
                }
            });
        }
    }
}