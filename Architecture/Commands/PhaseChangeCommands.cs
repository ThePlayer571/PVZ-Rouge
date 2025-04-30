using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using QFramework;
using TPL.PVZR.Architecture.Models;
using TPL.PVZR.Architecture.Systems;
using TPL.PVZR.Architecture.Systems.PhaseSystems;
using TPL.PVZR.Core.Random;
using TPL.PVZR.Core.Save;
using TPL.PVZR.Core.Save.Modules;
using TPL.PVZR.Gameplay.Class.Games;
using TPL.PVZR.Gameplay.Class.Levels;
using TPL.PVZR.Gameplay.Class.MazeMap;
using TPL.PVZR.Gameplay.Class.MazeMap.Core;
using Console = System.Console;

namespace TPL.PVZR.Architecture.Commands
{
    /// <summary>
    /// 开始一场新游戏（随机种子）
    /// </summary>
    public class StartNewGameCommand : AbstractCommand
    {
        public StartNewGameCommand(ulong? seed = null)
        {
            seed ??= RandomHelper.Default.NextUnsigned();
            this.seed = seed.Value;
        }

        private ulong seed;

        protected override void OnExecute()
        {
            var _GamePhaseSystem = this.GetSystem<IGamePhaseSystem>();
            var _GameSystem = this.GetSystem<IGameSystem>();
            var _SaveSystem = this.GetSystem<SaveSystem>();
            // 只能在某些时候调用：判断是否应该开始游戏
            if (_GamePhaseSystem.currentRoughGamePhase is not GamePhaseSystem.RoughGamePhase.MainMenu)
            {
                throw new Exception($"在不正确的时间开始StartGame：{_GamePhaseSystem.currentGamePhase}");
            }

            // 持久存储
            var gameSaveModule = _SaveSystem.GetModule<GameSaveModule>("game");
            if (gameSaveModule is null) throw new Exception("未找到GameSaveModule");
            gameSaveModule.Reset();
            // 核心
            _GamePhaseSystem.ChangePhase(GamePhaseSystem.GamePhase.GameInitialization,
                new Dictionary<string, object>
                {
                    ["gameToEnter"] = Game.CreateRandom(seed)
                });
        }
    }

    /// <summary>
    /// 进入一个新的关卡
    /// </summary>
    public class EnterLevelCommand : AbstractCommand
    {
        public EnterLevelCommand(ILevel level, Node levelFromNode)
        {
            this._level = level;
            this._levelFromNode = levelFromNode;
        }

        private ILevel _level;

        /// <summary>
        /// 玩家通过点击Spot进入关卡，这个Spot所在的Node是_levelFromNode
        /// </summary>
        private Node _levelFromNode;

        protected override void OnExecute()
        {
            var _GamePhaseSystem = this.GetSystem<IGamePhaseSystem>();
            var _LevelSystem = this.GetSystem<ILevelSystem>();
            var _GameModel = this.GetModel<IGameModel>();
            // 只能在某些时候调用：判断是否应该开始关卡
            if (_GamePhaseSystem.currentRoughGamePhase is not GamePhaseSystem.RoughGamePhase.MazeMap)
            {
                throw new Exception($"在不正确的时间开始StartLevel：{_GamePhaseSystem.currentGamePhase}");
            }

            // 核心
            _GameModel.SetLastEnteredNode(_levelFromNode);
            _GamePhaseSystem.ChangePhase(GamePhaseSystem.GamePhase.LevelPreInitialization,
                new Dictionary<string, object> { ["levelToEnter"] = _level });
        }
    }

    public class TryEndLevelByAllEnemyKilledCommand : AbstractCommand
    {
        protected override void OnExecute()
        {
            var _GamePhaseSystem = this.GetSystem<IGamePhaseSystem>();
            // 只能在某些时候调用：判断是否应该开始关卡
            if (_GamePhaseSystem.currentGamePhase is not GamePhaseSystem.GamePhase.Gameplay)
            {
                throw new Exception($"在不正确的时间开始TryEndLevelByAllEnemyKilled：{_GamePhaseSystem.currentGamePhase}");
            }

            // 核心
            _GamePhaseSystem.ChangePhase(GamePhaseSystem.GamePhase.AllEnemyKilled);
        }
    }
}