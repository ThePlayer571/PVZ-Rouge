using System;
using System.Collections.Generic;
using System.Linq;
using QFramework;
using TPL.PVZR.Classes.DataClasses.Level;
using TPL.PVZR.Classes.InfoClasses;
using TPL.PVZR.CommandEvents.Level_Gameplay.Waves;
using TPL.PVZR.Models;
using TPL.PVZR.Services;
using TPL.PVZR.Tools.Save;
using UnityEngine;

namespace TPL.PVZR.Systems
{
    public interface IPlayerStatsSystem : ISystem
    {
    }

    public class PlayerStatsSystem : AbstractSystem, IPlayerStatsSystem
    {
        private IPhaseService _PhaseService;
        private ISaveService _SaveService;
        private ILevelModel _LevelModel;
        private IGameModel _GameModel;

        private WhenLevelEndedInfo _whenLevelEndedInfo;

        protected override void OnInit()
        {
            _PhaseService = this.GetService<IPhaseService>();
            _SaveService = this.GetService<ISaveService>();
            _LevelModel = this.GetModel<ILevelModel>();
            _GameModel = this.GetModel<IGameModel>();

            _PhaseService.RegisterCallBack((GamePhase.LevelInitialization, PhaseStage.EnterLate), e =>
            {
                _whenLevelEndedInfo = new WhenLevelEndedInfo
                {
                    levelDef = _LevelModel.LevelData.LevelDef,
                    stage = _GameModel.ActiveTombData.Stage,
                };
            });

            this.RegisterEvent<OnWaveStart>(e =>
            {
                _whenLevelEndedInfo.waveSunPoints[e.Wave] = _LevelModel.SunPoint.Value;
            });

            _PhaseService.RegisterCallBack((GamePhase.LevelPassed, PhaseStage.EnterEarly), e =>
            {
                _whenLevelEndedInfo.pass = true;
                _whenLevelEndedInfo.usedCards =
                    _LevelModel.ChosenSeeds.Select(seed => seed.CardData.CardDefinition.PlantDef).ToList();
                _whenLevelEndedInfo.inventorySaveData = _GameModel.GameData.InventoryData.ToSaveData();
                //
                _SaveService.SaveManager.Save(SaveManager.GetFileName(SavePathId.PlayerStats_LevelEndedInfo),
                    _whenLevelEndedInfo);
            });

            _PhaseService.RegisterCallBack((GamePhase.LevelDefeat, PhaseStage.EnterEarly), e =>
            {
                _whenLevelEndedInfo.pass = false;
                _whenLevelEndedInfo.usedCards =
                    _LevelModel.ChosenSeeds.Select(seed => seed.CardData.CardDefinition.PlantDef).ToList();
                _whenLevelEndedInfo.inventorySaveData = _GameModel.GameData.InventoryData.ToSaveData();
                //
                _SaveService.SaveManager.Save(SaveManager.GetFileName(SavePathId.PlayerStats_LevelEndedInfo),
                    _whenLevelEndedInfo);
            });
        }
    }

    [Serializable]
    public class WhenLevelEndedInfo : ISaveData
    {
        // 基础信息
        public LevelDef levelDef;
        public int stage;

        public bool pass;

        //
        public Dictionary<int, int> waveSunPoints = new();
        public List<PlantDef> usedCards = new();
        public InventorySaveData inventorySaveData;
    }
}