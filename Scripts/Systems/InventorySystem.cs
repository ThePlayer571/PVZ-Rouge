using System.Linq;
using QFramework;
using TPL.PVZR.CommandEvents.Phase;
using TPL.PVZR.Models;

namespace TPL.PVZR.Systems
{
    public interface IInventorySystem : ISystem
    {
    }

    public class InventorySystem : AbstractSystem, IInventorySystem
    {
        private IGameModel _GameModel;
        private ILevelModel _LevelModel;

        protected override void OnInit()
        {
            _GameModel = this.GetModel<IGameModel>();
            _LevelModel = this.GetModel<ILevelModel>();

            this.RegisterEvent<OnPhaseChangeEvent>(e =>
            {
                switch (e.GamePhase)
                {
                    case GamePhase.LevelPassed or GamePhase.LevelDefeat:
                        switch (e.PhaseStage)
                        {
                            case PhaseStage.EnterNormal:
                                foreach (var chosenCard in
                                         _LevelModel.ChosenSeeds.Select(seedData => seedData.CardData))
                                {
                                    if (!chosenCard.Locked)
                                        _GameModel.GameData.InventoryData.RemoveCard(chosenCard);
                                }

                                break;
                        }

                        break;
                    case GamePhase.MazeMap:
                        switch (e.PhaseStage)
                        {
                            case PhaseStage.LeaveLate:
                                _GameModel.GameData.InventoryData.SortCards();
                                break;
                        }

                        break;
                }
            });
        }
    }
}