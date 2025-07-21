using System;
using System.Collections.Generic;
using QFramework;
using TPL.PVZR.Classes.DataClasses.Game;
using TPL.PVZR.Models;

namespace TPL.PVZR.CommandEvents._NotClassified_
{
    public class StartGameCommand : AbstractCommand
    {
        public StartGameCommand(IGameData gameData, bool isNewGame)
        {
            _gameData = gameData;
            _isNewGame = isNewGame;
        }

        private IGameData _gameData;
        private bool _isNewGame;

        protected override void OnExecute()
        {
            var PhaseModel = this.GetModel<IPhaseModel>();

            // 异常处理
            if (PhaseModel.GamePhase != GamePhase.MainMenu)
                throw new Exception($"在不正确的阶段执行StartGameCommand：{PhaseModel.GamePhase}");

            PhaseModel.ChangePhase(GamePhase.GameInitialization,
                new Dictionary<string, object> { { "GameData", _gameData }, { "IsNewGame", _isNewGame } });
        }
    }
}