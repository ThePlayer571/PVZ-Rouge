using System.Collections.Generic;
using System.Linq;
using QFramework;
using TPL.PVZR.Classes.DataClasses.Award;
using TPL.PVZR.Classes.DataClasses.Loot;
using TPL.PVZR.CommandEvents.MazeMap_AwardPanel;
using TPL.PVZR.CommandEvents.Phase;
using TPL.PVZR.Helpers.New;
using TPL.PVZR.Helpers.New.ClassCreator;
using TPL.PVZR.Models;
using TPL.PVZR.Tools;
using TPL.PVZR.Tools.Random;
using TPL.PVZR.Tools.SoyoFramework;

namespace TPL.PVZR.Systems.MazeMap
{
    public interface IAwardSystem : IServiceManageSystem, IDataSystem
    {
        IReadOnlyList<LootData> GetLootGroupByIndex(int index);
        bool IsAwardAvailable { get; }
        int AwardCount { get; }
    }

    public class AwardSystem : AbstractSystem, IAwardSystem
    {
        private List<List<LootData>> CurrentAwards = new();

        #region 接口ILootChooseSystem实现

        public IReadOnlyList<LootData> GetLootGroupByIndex(int index)
        {
            return CurrentAwards[index];
        }

        public bool IsAwardAvailable { get; private set; }

        public int AwardCount => CurrentAwards.Count;

        #endregion

        private void WriteLoots(AwardGenerateInfo awardInfo)
        {
            CurrentAwards.Clear();
            CurrentAwards.AddRange(AwardCreator.CreateAwardData(awardInfo));
        }

        private void ClearLootList()
        {
            CurrentAwards.Clear();
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
                                _GameModel.GameData.AwardData.AwardsToGenerate =
                                    _LevelModel.LevelData.AwardGenerateInfo;
                                break;
                        }

                        break;
                    case GamePhase.MazeMapInitialization:
                        switch (e.PhaseStage)
                        {
                            case PhaseStage.EnterNormal:
                                IsAwardAvailable = true;
                                $"null is : GameData: {_GameModel.GameData == null}, AwardData : {_GameModel.GameData.AwardData == null}, AwardsToGenerate : {_GameModel.GameData?.AwardData?.AwardsToGenerate == null}"
                                    .LogInfo();
                                WriteLoots(_GameModel.GameData.AwardData.AwardsToGenerate);
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
                    if (lootData.LootType == LootType.PlantBook &&
                        inventory.PlantBooks.Any(b => b.Id == lootData.PlantBookId)) continue;
                    if (lootData.LootType == LootType.SeedSlot && !inventory.HasAvailableSeedSlotSlots()) continue;
                    inventory.AddLootAuto(lootData);
                }
            });
        }
    }
}