using QFramework;
using TPL.PVZR.Classes.DataClasses.Game;
using TPL.PVZR.Classes.DataClasses.Item.Card;
using TPL.PVZR.CommandEvents.Phase;
using TPL.PVZR.Helpers.New;
using TPL.PVZR.Models;

namespace TPL.PVZR.Systems
{
    public interface IGameSystem : IMainSystem
    {
    }

    public class GameSystem : AbstractSystem, IGameSystem
    {
        private IPhaseModel _PhaseModel;
        private IGameModel _GameModel;

        protected override void OnInit()
        {
            _PhaseModel = this.GetModel<IPhaseModel>();
            _GameModel = this.GetModel<IGameModel>();

            this.RegisterEvent<OnPhaseChangeEvent>(e =>
            {
                switch (e.GamePhase)
                {
                    case GamePhase.GameInitialization:
                        switch (e.PhaseStage)
                        {
                            case PhaseStage.EnterEarly:
                                _GameModel.GameData = e.Parameters["GameData"] as GameData;
                                PlantDefHelper.SetInventory(_GameModel.GameData.InventoryData);
                                break;
                            case PhaseStage.EnterLate:
                                _PhaseModel.DelayChangePhase(GamePhase.MazeMapInitialization);
                                break;
                        }

                        break;
                }
            });
        }
    }
}