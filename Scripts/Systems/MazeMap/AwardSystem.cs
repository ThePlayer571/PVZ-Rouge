using System.Collections.Generic;
using System.Linq;
using QFramework;
using TPL.PVZR.Classes.DataClasses.Loot;
using TPL.PVZR.CommandEvents.Phase;
using TPL.PVZR.Helpers.ClassCreator;
using TPL.PVZR.Helpers.ClassCreator.Item;
using TPL.PVZR.Models;
using TPL.PVZR.Tools;

namespace TPL.PVZR.Systems
{
    public interface IAwardSystem : ISystem
    {
        List<LootData> GetLootGroupOfIndex(int index);
        bool HasAward { get; }
    }

    public class AwardSystem : AbstractSystem, IAwardSystem
    {
        #region 接口ILootChooseSystem实现

        private List<List<LootData>> _lootGroupList = new();

        public List<LootData> GetLootGroupOfIndex(int index)
        {
            if (index < 1 || index > _lootGroupList.Count)
            {
                throw new System.ArgumentOutOfRangeException(nameof(index), "Index is out of range.");
            }

            return _lootGroupList[index - 1];
        }

        public bool HasAward { get; private set; } = false;

        #endregion

        private void WriteLoots(List<LootGenerateInfo> infos, float value, int count)
        {
            "Call Writre".LogInfo();
            _lootGroupList.Clear();
            HasAward = true;
            for (int i = 0; i < count; i++)
            {
                var randomPool = new RandomPool<LootGenerateInfo, LootInfo>(infos, value);
                var chosenLoots = randomPool.GetAllRemainingOutputs()
                    .Select(lootInfo => LootHelper.CreateLootData(lootInfo));
                _lootGroupList.Add(chosenLoots.ToList());
            }
        }

        private void ClearLootList()
        {
            _lootGroupList.Clear();
            HasAward = false;
        }

        private ILevelModel _LevelModel;

        protected override void OnInit()
        {
            _LevelModel = this.GetModel<ILevelModel>();

            this.RegisterEvent<OnPhaseChangeEvent>(e =>
            {
                switch (e.GamePhase)
                {
                    case GamePhase.LevelExiting:
                        switch (e.PhaseStage)
                        {
                            case PhaseStage.EnterNormal:
                                WriteLoots(_LevelModel.LevelData.LootGenerateInfos, _LevelModel.LevelData.LootValue, 3);
                                break;
                        }

                        break;
                    case GamePhase.MazeMap:
                        switch (e.PhaseStage)
                        {
                            case PhaseStage.LeaveLate:
                                ClearLootList();
                                break;
                        }

                        break;
                }
            });
        }
    }
}