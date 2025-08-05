using System.Collections.Generic;
using QFramework;
using TPL.PVZR.Models;
using TPL.PVZR.Services;
using TPL.PVZR.Tools.Save;

namespace TPL.PVZR.Systems
{
    public interface ISaveSystem : ISystem
    {
    }

    public class SaveSystem : AbstractSystem, ISaveSystem
    {
        private IGameModel _GameModel;

        protected override void OnInit()
        {
            _GameModel = this.GetModel<IGameModel>();

            var phaseService = this.GetService<IPhaseService>();
            var saveService = this.GetService<ISaveService>();

            phaseService.RegisterCallBack((GamePhase.GameInitialization, PhaseStage.LeaveLate), e =>
            {
                var isNewGame = (bool)e.Paras["IsNewGame"];
                if (isNewGame)
                    saveService.SaveManager.Save(SaveManager.GAME_DATA_FILE_NAME, _GameModel.GameData.ToSaveData());
            });

            phaseService.RegisterCallBack((GamePhase.LevelPassed, PhaseStage.LeaveLate), e =>
            {
                //
                saveService.SaveManager.Save(SaveManager.GAME_DATA_FILE_NAME, _GameModel.GameData.ToSaveData());
            });

            phaseService.RegisterCallBack((GamePhase.GameExiting, PhaseStage.LeaveLate), e =>
            {
                var deleteSave = (bool)e.Paras.GetValueOrDefault("DeleteSave", false);
                if (deleteSave)
                {
                    saveService.SaveManager.Delete(SaveManager.GAME_DATA_FILE_NAME);
                }
            });
        }
    }
}