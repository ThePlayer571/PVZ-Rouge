using System;
using System.Runtime.CompilerServices;
using QFramework;
using TPL.PVZR.Architecture.Systems;
using TPL.PVZR.Architecture.Systems.PhaseSystems;
using TPL.PVZR.Core.Random;
using TPL.PVZR.Gameplay.Class.Games;
using TPL.PVZR.Gameplay.Class.Levels;

namespace TPL.PVZR.Architecture.Commands
{
    /// <summary>
    /// 开始一场新游戏（随机种子）
    /// </summary>
    public class StartNewGameCommand:AbstractCommand
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
            // 只能在某些时候调用：判断是否应该开始游戏
            if (_GamePhaseSystem.currentRoughGamePhase is not GamePhaseSystem.RoughGamePhase.MainMenu)
            {
                throw new Exception($"在不正确的时间开始StartGame：{_GamePhaseSystem.currentGamePhase}");
            }
            // 核心
            _GameSystem.SetCurrentGame(Game.CreateRandom(seed));
            _GamePhaseSystem.ChangePhase(GamePhaseSystem.GamePhase.GameInitialization);
        }
    }

    /// <summary>
    /// 进入一个新的关卡
    /// </summary>
    public class EnterLevelCommand : AbstractCommand
    {
        public EnterLevelCommand(ILevel level)
        {
            this._level = level;
        }

        private ILevel _level;
        
        protected override void OnExecute()
        {
            var _GamePhaseSystem = this.GetSystem<IGamePhaseSystem>();
            var _LevelSystem = this.GetSystem<ILevelSystem>();
            // 只能在某些时候调用：判断是否应该开始关卡
            if (_GamePhaseSystem.currentRoughGamePhase is not GamePhaseSystem.RoughGamePhase.MazeMap)
            {
                throw new Exception($"在不正确的时间开始StartLevel：{_GamePhaseSystem.currentGamePhase}");
            }
            // 核心
            _LevelSystem.SetCurrentLevel(_level);
            _GamePhaseSystem.ChangePhase(GamePhaseSystem.GamePhase.LevelInitialization);
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