using System.Collections.Generic;
using System.Linq;
using QFramework;
using TPL.PVZR.Classes.DataClasses.Loot;
using TPL.PVZR.CommandEvents.MazeMap_AwardPanel;
using TPL.PVZR.CommandEvents.Phase;
using TPL.PVZR.Helpers.New;
using TPL.PVZR.Helpers.New.ClassCreator;
using TPL.PVZR.Models;
using TPL.PVZR.Tools;
using TPL.PVZR.Tools.Random;

namespace TPL.PVZR.Systems.MazeMap
{
    public interface IAwardSystem : ISystem
    {
        IReadOnlyList<LootData> GetLootGroupByIndex(int index);
        bool IsAwardAvailable { get; }
        bool HasAward { get; }
    }

    public class AwardSystem : AbstractSystem, IAwardSystem
    {
        #region 接口ILootChooseSystem实现

        private List<List<LootData>> _lootGroupList = new();

        public IReadOnlyList<LootData> GetLootGroupByIndex(int index)
        {
            return _lootGroupList[index];
        }

        public bool IsAwardAvailable { get; private set; } = false;

        public bool HasAward { get; private set; } = false;

        #endregion

        private void WriteLoots(IReadOnlyList<LootGenerateInfo> infos, float value, int count)
        {
            _lootGroupList.Clear();

            // 筛选：已经拥有的PlantBook不会被加入
            infos = infos.Where(info =>
                info.lootInfo.lootType != LootType.PlantBook ||
                _GameModel.GameData.InventoryData.PlantBooks.All(pb => pb.PlantId != info.lootInfo.plantId)).ToList();


            HasAward = true;
            for (int i = 0; i < count; i++)
            {
                var randomPool = new RandomPool<LootGenerateInfo, LootInfo>(infos, value, RandomHelper.Game);
                var chosenLoots = randomPool.GetAllRemainingOutputs()
                    .Select(lootInfo => LootData.Create(lootInfo));
                _lootGroupList.Add(chosenLoots.ToList());
            }
        }

        private void ClearLootList()
        {
            _lootGroupList.Clear();
            HasAward = false;
        }

        private ILevelModel _LevelModel;
        private IGameModel _GameModel;

        protected override void OnInit()
        {
            _LevelModel = this.GetModel<ILevelModel>();
            _GameModel = this.GetModel<IGameModel>();

            this.RegisterEvent<OnPhaseChangeEvent>(e =>
            {
                switch (e.GamePhase)
                {
                    case GamePhase.LevelPassed:
                        switch (e.PhaseStage)
                        {
                            case PhaseStage.EnterNormal:
                                IsAwardAvailable = true;
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

            this.RegisterEvent<ChooseAwardEvent>(e =>
            {
                IsAwardAvailable = false;
                var awards = GetLootGroupByIndex(e.index);
                var inventory = _GameModel.GameData.InventoryData;

                foreach (var lootData in awards)
                {
                    if (lootData.LootType == LootType.Card && !inventory.HasAvailableCardSlots()) continue;
                    if (lootData.LootType == LootType.PlantBook && inventory.PlantBooks.Any(b=>b.Id == lootData.PlantBookId)) continue;
                    if (lootData.LootType == LootType.SeedSlot && !inventory.HasAvailableSeedSlotSlots()) continue;
                    inventory.AddLootAuto(lootData);
                }
            });
        }
    }
}