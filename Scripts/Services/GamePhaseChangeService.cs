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
using TPL.PVZR.Tools.SoyoFramework;
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
        void ExitGameAndSave();
        void TrySpawnLevelEndObject(Vector3 position);
        void PauseGame();
        void ResumeGame(bool TEMP_stopAudio = false);
    }

    public class GamePhaseChangeService : AbstractService, IGamePhaseChangeService
    {
        private IPhaseModel _PhaseModel;
        private ILevelModel _LevelModel;
        private IGameModel _GameModel;
        private IZombieSpawnSystem _ZombieSpawnSystem;


        private IPhaseService _PhaseService;
        private IZombieService _ZombieService;


        protected override void OnInit()
        {
            _PhaseModel = this.GetModel<IPhaseModel>();
            _LevelModel = this.GetModel<ILevelModel>();
            _GameModel = this.GetModel<IGameModel>();
            _ZombieSpawnSystem = this.GetSystem<IZombieSpawnSystem>();
            _PhaseService = this.GetService<IPhaseService>();
            _ZombieService = this.GetService<IZombieService>();
        }

        public void StartGame(IGameData gameData, bool isNewGame)
        {
            if (_PhaseModel.GamePhase == GamePhase.MainMenu)
            {
                _PhaseService.ChangePhase(GamePhase.GameInitialization, ("GameData", gameData), ("IsNewGame", true));
            }
            else $"在错误的阶段StartGame: {_PhaseModel.GamePhase}".LogError();
        }

        public void StartMazeMap(IMazeMapData mazeMapData)
        {
            throw new NotImplementedException();
        }

        public void StartLevel(ITombData tombData)
        {
            if (_PhaseModel.GamePhase == GamePhase.MazeMap)
            {
                _PhaseService.ChangePhase(GamePhase.LevelPreInitialization, ("TombData", tombData));
            }
            else $"在错误的阶段StartLevel：{_PhaseModel.GamePhase}".LogError();
        }

        public void EnterMazeMapWithLevelDefeated()
        {
            if (_PhaseModel.GamePhase == GamePhase.LevelDefeatPanel)
            {
                _PhaseService.ChangePhase(GamePhase.MazeMapInitialization, ("NotRefresh", true));
            }
            else $"在错误的阶段EnterMazeMapWithLevelDefeated: {_PhaseModel.GamePhase}".LogError();
        }

        public void EnterMazeMapWithLevelPassed()
        {
            if (_PhaseModel.GamePhase == GamePhase.AllEnemyKilled)
            {
                _PhaseService.ChangePhase(GamePhase.LevelPassed);
            }
            else $"在错误的阶段EnterMazeMapWithLevelPassed: {_PhaseModel.GamePhase}".LogError();
        }

        public void EnterLevelDefeatPanel()
        {
            if (_PhaseModel.GamePhase is (GamePhase.ChooseSeeds or GamePhase.Gameplay or GamePhase.AllEnemyKilled))
            {
                _PhaseService.ChangePhase(GamePhase.LevelDefeat);
            }
            else $"在错误的阶段EnterLevelDefeatPanel: {_PhaseModel.GamePhase}".LogError();
        }

        public void ExitGameWithGiveUp()
        {
            if (_PhaseModel.GamePhase == GamePhase.LevelDefeatPanel)
            {
                _PhaseService.ChangePhase(GamePhase.GameExiting, ("DeleteSave", true));
            }
            else $"在错误的阶段ExitGameWithGiveUp: {_PhaseModel.GamePhase}".LogError();
        }


        public void ExitGameWithGamePassed()
        {
            if (_PhaseModel.GamePhase == GamePhase.MazeMap)
            {
                _PhaseService.ChangePhase(GamePhase.GameExiting, ("DeleteSave", true));
            }
            else $"在错误的阶段ExitGameWithGamePassed: {_PhaseModel.GamePhase}".LogError();
        }

        public void ExitGameAndSave()
        {
            if (_PhaseModel.IsInRoughPhase(RoughPhase.Game) && _PhaseModel.IsInRoughPhase(RoughPhase.Process))
            {
                if (_PhaseModel.GamePhase == GamePhase.MazeMap)
                {
                    _PhaseService.ChangePhase(GamePhase.GameExiting);
                }
                else if (_PhaseModel.IsInRoughPhase(RoughPhase.Level))
                {
                    _PhaseService.ChangePhase(GamePhase.LevelExiting, ("NextPhase", GamePhase.GameExiting));
                }
            }
            else $"在错误的阶段ExitGameAndSave: {_PhaseModel.GamePhase}".LogError();
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

        public void PauseGame()
        {
            if (_PhaseModel.IsInRoughPhase(RoughPhase.Game) && _PhaseModel.IsInRoughPhase(RoughPhase.Process))
            {
                _GameModel.IsGamePaused = true;
                this.SendEvent<OnGamePaused>();
            }
            else $"在错误的阶段PauseGame: {_PhaseModel.GamePhase}".LogError();
        }

        public void ResumeGame(bool TEMP_stopAudio = false)
        {
            if (_PhaseModel.IsInRoughPhase(RoughPhase.Game))
            {
                _GameModel.IsGamePaused = false;
                this.SendEvent<OnGameResumed>(new OnGameResumed { TEMP_stopAudio = TEMP_stopAudio });
            }
            else $"在错误的阶段ResumeGame: {_PhaseModel.GamePhase}".LogError();
        }
    }

    public struct OnGamePaused : IEvent
    {
    }

    public struct OnGameResumed : IEvent
    {
        public bool TEMP_stopAudio;
    }
}