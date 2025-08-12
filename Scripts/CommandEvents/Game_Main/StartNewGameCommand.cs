using System;
using QFramework;
using TPL.PVZR.Classes.DataClasses.Game;
using TPL.PVZR.Classes.MazeMap;
using TPL.PVZR.Helpers.New.ClassCreator;
using TPL.PVZR.Models;
using TPL.PVZR.Services;
using TPL.PVZR.Tools.Random;
using TPL.PVZR.Tools.Save;

namespace TPL.PVZR.CommandEvents._NotClassified_
{
    public class StartNewGameCommand : AbstractCommand
    {
        public StartNewGameCommand(GameDef gameDef, ulong? seed = null)
        {
            _gameDef = gameDef;
            _seed = seed ?? RandomHelper.Default.NextUnsigned();
        }

        private ulong _seed;
        private GameDef _gameDef;

        protected override void OnExecute()
        {
            // 异常处理
            var PhaseModel = this.GetModel<IPhaseModel>();
            if (PhaseModel.GamePhase != GamePhase.MainMenu)
                throw new Exception($"在不正确的阶段执行StartNewGameCommand：{PhaseModel.GamePhase}");

            // 生成一个新的游戏数据
            IGameData GameData = GameCreator.CreateGameData(_gameDef, _seed);
            // 开始游戏
            var gamePhaseChangeService = this.GetService<IGamePhaseChangeService>();
            gamePhaseChangeService.StartGame(GameData, false);
        }
    }

    public class ContinueGameCommand : AbstractCommand
    {
        protected override void OnExecute()
        {
            var PhaseModel = this.GetModel<IPhaseModel>();
            var saveService = this.GetService<ISaveService>();
            // 异常处理
            if (PhaseModel.GamePhase != GamePhase.MainMenu)
                throw new Exception($"在不正确的阶段执行ContinueGameCommand：{PhaseModel.GamePhase}");
            if (!saveService.SaveManager.Exists(SaveManager.GAME_DATA_FILE_NAME))
            {
                throw new Exception("调用了 ContinueGameCommand，但没有找到游戏数据文件");
            }

            // 继续游戏
            var savedGameData =
                new GameData(saveService.SaveManager.Load<GameSaveData>(SaveManager.GAME_DATA_FILE_NAME));
            var gamePhaseChangeService = this.GetService<IGamePhaseChangeService>();
            gamePhaseChangeService.StartGame(savedGameData, false);
        }
    }
}