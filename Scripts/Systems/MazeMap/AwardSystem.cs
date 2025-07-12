using System.Collections.Generic;
using System.Linq;
using QFramework;
using TPL.PVZR.Classes.DataClasses.Loot;
using TPL.PVZR.CommandEvents.__NewlyAdded__;
using TPL.PVZR.CommandEvents.Phase;
using TPL.PVZR.Helpers.ClassCreator;
using TPL.PVZR.Helpers.ClassCreator.Item;
using TPL.PVZR.Models;
using TPL.PVZR.Tools;
using TPL.PVZR.Tools.Random;

namespace TPL.PVZR.Systems
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

        private void WriteLoots(List<LootGenerateInfo> infos, float value, int count)
        {
            _lootGroupList.Clear();
            HasAward = true;
            for (int i = 0; i < count; i++)
            {
                var randomPool = new RandomPool<LootGenerateInfo, LootInfo>(infos, value, RandomHelper.Game);
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
        private IGameModel _GameModel;

        protected override void OnInit()
        {
            _LevelModel = this.GetModel<ILevelModel>();
            _GameModel = this.GetModel<IGameModel>();

            this.RegisterEvent<OnPhaseChangeEvent>(e =>
            {
                switch (e.GamePhase)
                {
                    case GamePhase.LevelExiting:
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
                    if (!inventory.HasAvailableCardSlots()) break;

                    switch (lootData.LootType)
                    {
                        case LootType.Card:
                            inventory.AddCard(lootData.CardData);
                            break;
                    }
                }
            });
        }
    }
}