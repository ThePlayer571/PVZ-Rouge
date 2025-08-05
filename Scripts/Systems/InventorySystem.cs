using System.Linq;
using QFramework;
using TPL.PVZR.Models;
using TPL.PVZR.Services;

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

            var phaseService = this.GetService<IPhaseService>();

            phaseService.RegisterCallBack((GamePhase.LevelPassed, PhaseStage.EnterNormal), e =>
            {
                foreach (var chosenCard in _LevelModel.ChosenSeeds.Select(seedData => seedData.CardData))
                {
                    if (!chosenCard.Locked) _GameModel.GameData.InventoryData.RemoveCard(chosenCard);
                }
            });

            phaseService.RegisterCallBack((GamePhase.MazeMap, PhaseStage.LeaveLate), e =>
            {
                //
                _GameModel.GameData.InventoryData.SortCards();
            });
        }
    }
}