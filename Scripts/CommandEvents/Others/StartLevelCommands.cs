using System.Collections.Generic;
using QFramework;
using TPL.PVZR.Classes.Game;
using TPL.PVZR.Classes.Level;
using TPL.PVZR.Models;

namespace TPL.PVZR.Commands
{
    // public class TestStartLevelCommand : AbstractCommand
    // {
    //     public TestStartLevelCommand(IGameData gameData)
    //     {
    //         _gameData = gameData;
    //     }
    //
    //     private IGameData _gameData;
    //
    //     protected override void OnExecute()
    //
    //     {
    //         var PhaseModel = this.GetModel<IPhaseModel>();
    //         if (PhaseModel.GamePhase != GamePhase.MazeMap)
    //             throw new System.Exception($"在不正确的阶段执行TestStartLevelCommand：{PhaseModel.GamePhase}");
    //
    //         PhaseModel.ChangePhase(GamePhase.LevelPreInitialization,
    //             new Dictionary<string, object> { { "LevelData", new LevelData(_gameData) } });
    //     }
    // }
}