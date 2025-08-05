using System;
using QFramework;
using TPL.PVZR.Classes.DataClasses.Game;
using TPL.PVZR.Classes.MazeMap;
using TPL.PVZR.Helpers.New.ClassCreator;
using TPL.PVZR.Models;
using TPL.PVZR.Services;
using TPL.PVZR.Tools.Random;

namespace TPL.PVZR.CommandEvents._NotClassified_
{
    public class StartNewGameCommand : AbstractCommand
    {
        public StartNewGameCommand(ulong? seed = null)
        {
            _seed = seed ?? RandomHelper.Default.NextUnsigned();
        }

        private ulong _seed;

        protected override void OnExecute()
        {
            // 异常处理
            var PhaseModel = this.GetModel<IPhaseModel>();
            if (PhaseModel.GamePhase != GamePhase.MainMenu)
                throw new Exception($"在不正确的阶段执行StartNewGameCommand：{PhaseModel.GamePhase}");

            // 生成一个新的游戏数据
            IGameData GameData = GameCreator.CreateGameData(new GameDef { GameDifficulty = GameDifficulty.N0}, _seed);
            // 开始游戏
            var gamePhaseChangeService = this.GetService<IGamePhaseChangeService>();
            gamePhaseChangeService.StartGame(GameData, false);
        }
    }
}