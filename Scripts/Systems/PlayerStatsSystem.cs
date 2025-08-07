using System;
using System.Collections.Generic;
using QFramework;
using TPL.PVZR.Classes.DataClasses.Level;
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

        private PlayerSunpointStats _playerSunpointStats;

        protected override void OnInit()
        {
            _PhaseService = this.GetService<IPhaseService>();
            _SaveService = this.GetService<ISaveService>();
            _LevelModel = this.GetModel<ILevelModel>();

            _PhaseService.RegisterCallBack((GamePhase.LevelInitialization, PhaseStage.EnterNormal), e =>
            {
                _playerSunpointStats = new PlayerSunpointStats
                {
                    levelDef = _LevelModel.LevelData.LevelDef
                };
            });

            this.RegisterEvent<OnWaveStart>(e =>
            {
                _playerSunpointStats.waveSunPoints[e.Wave] = _LevelModel.SunPoint.Value;
            });

            _PhaseService.RegisterCallBack((GamePhase.LevelExiting, PhaseStage.EnterNormal),
                e =>
                {
                    // 在文件名后添加当前时间戳
                    string timeStamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                    string fileNameWithTime = $"{SaveManager.PLAYER_STATS_SUNPOINT_FILE_NAME}_{timeStamp}";
                    _SaveService.SaveManager.Save(fileNameWithTime, _playerSunpointStats);
                });
        }
    }

    [Serializable]
    public class PlayerSunpointStats : ISaveData
    {
        public LevelDef levelDef;
        public Dictionary<int, int> waveSunPoints = new();
    }
}