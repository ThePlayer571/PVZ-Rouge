using QFramework;
using TPL.PVZR.CommandEvents.Phase;
using TPL.PVZR.Models;
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

            this.RegisterEvent<OnPhaseChangeEvent>(e =>
            {
                switch (e.GamePhase)
                {
                    case GamePhase.GameInitialization:
                        switch (e.PhaseStage)
                        {
                            case PhaseStage.LeaveLate:
                                if (e.Parameters == null) "find null".LogInfo();

                                var isNewGame = (bool)e.Parameters["IsNewGame"];
                                if (isNewGame)
                                    SaveHelper.Save(SaveHelper.GAME_DATA_FILE_NAME, _GameModel.GameData.ToSaveData());
                                break;
                        }

                        break;
                    case GamePhase.LevelPassed:
                        switch (e.PhaseStage)
                        {
                            case PhaseStage.LeaveLate:
                                SaveHelper.Save(SaveHelper.GAME_DATA_FILE_NAME, _GameModel.GameData.ToSaveData());
                                break;
                        }

                        break;
                    case GamePhase.GameExiting:
                        switch (e.PhaseStage)
                        {
                            case PhaseStage.LeaveLate:
                                SaveHelper.Delete(SaveHelper.GAME_DATA_FILE_NAME);
                                break;
                        }

                        break;
                }
            });
        }
    }
}