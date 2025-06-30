namespace TPL.PVZR.CommandEvents._NotClassified_
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