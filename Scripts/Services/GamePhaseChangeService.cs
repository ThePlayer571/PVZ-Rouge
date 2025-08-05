using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using QFramework;
using TPL.PVZR.Classes.DataClasses.Game;
using TPL.PVZR.Classes.DataClasses.Tomb;
using TPL.PVZR.Classes.MazeMap;
using TPL.PVZR.Helpers.New.GameObjectFactory;
using TPL.PVZR.Models;
using TPL.PVZR.Systems.Level_Event;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace TPL.PVZR.Services
{
    public interface IGamePhaseChangeService : IService
    {
        // Start开头：重设数据结构并开始
        void StartGame(IGameData gameData, bool isNewGame);

        void StartMazeMap(IMazeMapData mazeMapData);
        void StartLevel(ITombData tombData);

        // Enter开头：单纯的进入某个阶段
        void EnterMazeMapWithLevelDefeated();

        void EnterMazeMapWithLevelPassed();

        void EnterLevelDefeatPanel();

        // 其他
        void ExitGameWithGiveUp();
        void ExitGameWithGamePassed();
        void TrySpawnLevelEndObject(Vector3 position);
    }

    public class GamePhaseChangeService : AbstractService, IGamePhaseChangeService
    {
        private IPhaseModel _PhaseModel;
        private ILevelModel _LevelModel;
        private IZombieSpawnSystem _ZombieSpawnSystem;


        private IPhaseService _PhaseService;
        private IZombieService _ZombieService;


        protected override void OnInit()
        {
            _PhaseModel = this.GetModel<IPhaseModel>();
            _LevelModel = this.GetModel<ILevelModel>();
            _ZombieSpawnSystem = this.GetSystem<IZombieSpawnSystem>();
            _PhaseService = this.GetService<IPhaseService>();
            _ZombieService = this.GetService<IZombieService>();
        }

        public void StartGame(IGameData gameData, bool isNewGame)
        {
            if (_PhaseModel.GamePhase != GamePhase.MainMenu)
                throw new Exception($"在错误的阶段StartGame: {_PhaseModel.GamePhase}");

            _PhaseService.ChangePhase(GamePhase.GameInitialization, ("GameData", gameData), ("IsNewGame", true));
        }

        public void StartMazeMap(IMazeMapData mazeMapData)
        {
            throw new NotImplementedException();
        }

        public void StartLevel(ITombData tombData)
        {
            if (_PhaseModel.GamePhase != GamePhase.MazeMap)
                throw new System.Exception($"在错误的阶段StartLevel：{_PhaseModel.GamePhase}");
            //
            _PhaseService.ChangePhase(GamePhase.LevelPreInitialization, ("TombData", tombData));
        }

        public void EnterMazeMapWithLevelDefeated()
        {
            if (_PhaseModel.GamePhase != GamePhase.LevelDefeatPanel)
                throw new Exception($"在错误的阶段EnterMazeMapWithLevelDefeated: {_PhaseModel.GamePhase}");

            _PhaseService.ChangePhase(GamePhase.MazeMapInitialization, ("NotRefresh", true));
        }

        public void EnterMazeMapWithLevelPassed()
        {
            if (_PhaseModel.GamePhase != GamePhase.AllEnemyKilled)
                throw new Exception($"在错误的阶段EnterMazeMapWithLevelPassed: {_PhaseModel.GamePhase}");

            _PhaseService.ChangePhase(GamePhase.LevelPassed);
        }

        public void EnterLevelDefeatPanel()
        {
            if (_PhaseModel.GamePhase is not (GamePhase.ChooseSeeds or GamePhase.Gameplay or GamePhase.AllEnemyKilled))
                throw new Exception($"在错误的阶段EnterLevelDefeatPanel: {_PhaseModel.GamePhase}");

            _PhaseService.ChangePhase(GamePhase.LevelDefeat);
        }

        public void ExitGameWithGiveUp()
        {
            if (_PhaseModel.GamePhase != GamePhase.LevelDefeatPanel)
                throw new Exception($"在错误的阶段ExitGameWithGiveUp: {_PhaseModel.GamePhase}");

            _PhaseService.ChangePhase(GamePhase.GameExiting, ("DeleteSave", true));
        }

        public void ExitGameWithGamePassed()
        {
            throw new NotImplementedException();
        }

        public void TrySpawnLevelEndObject(Vector3 position)
        {
            bool pass = _LevelModel.CurrentWave.Value == _LevelModel.LevelData.TotalWaveCount &&
                        _ZombieSpawnSystem.ActiveTasksCount == 0 &&
                        _ZombieService.ActiveZombies.Count == 0;
            if (pass)
            {
                Addressables.LoadAssetAsync<GameObject>("LevelEndObject").Completed += handle =>
                {
                    handle.Result.Instantiate(position, Quaternion.identity);
                    handle.Release();
                };
                var phaseService = this.GetService<IPhaseService>();
                phaseService.ChangePhase(GamePhase.AllEnemyKilled);
            }
        }
    }
}