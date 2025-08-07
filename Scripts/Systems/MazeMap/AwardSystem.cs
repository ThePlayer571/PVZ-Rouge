using System.Collections.Generic;
using System.Linq;
using QFramework;
using TPL.PVZR.Classes.DataClasses.Award;
using TPL.PVZR.Classes.DataClasses.Loot;
using TPL.PVZR.CommandEvents.MazeMap_AwardPanel;
using TPL.PVZR.Helpers.New;
using TPL.PVZR.Helpers.New.ClassCreator;
using TPL.PVZR.Models;
using TPL.PVZR.Services;
using TPL.PVZR.Tools;
using TPL.PVZR.Tools.Random;
using TPL.PVZR.Tools.SoyoFramework;

namespace TPL.PVZR.Systems.MazeMap
{
    public interface IAwardSystem : IDataSystem
    {
        IReadOnlyList<LootData> GetLootGroupByIndex(int index); 
        BindableProperty<bool> IsAwardAvailable { get; set; }
        BindableProperty<int> ChosenAwardIndex { get; set; }
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

        public BindableProperty<bool> IsAwardAvailable { get; set; } = new();
        public BindableProperty<int> ChosenAwardIndex { get; set; } = new();

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

            var phaseService = this.GetService<IPhaseService>();

            phaseService.RegisterCallBack((GamePhase.LevelPassed, PhaseStage.EnterNormal), e =>
            {
                //
                _GameModel.GameData.AwardData.AwardsToGenerate = _LevelModel.LevelData.AwardGenerateInfo;
            });

            phaseService.RegisterCallBack((GamePhase.MazeMapInitialization, PhaseStage.EnterNormal), e =>
            {
                var notRefresh = (bool)e.Paras.GetValueOrDefault<string, object>("NotRefresh", false);
                if (notRefresh) return;
                // 清空现有数据，生成新的奖励
                IsAwardAvailable.Value = true;
                ChosenAwardIndex.Value = -1;
                WriteLoots(_GameModel.GameData.AwardData.AwardsToGenerate);
            });

            phaseService.RegisterCallBack((GamePhase.GameExiting, PhaseStage.EnterNormal), e =>
            {
                //
                ClearLootList();
            });
        }
    }
}