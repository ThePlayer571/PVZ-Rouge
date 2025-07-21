using QFramework;
using TPL.PVZR.Classes.DataClasses_InLevel;
using TPL.PVZR.CommandEvents.Level_Gameplay.PlantSpawn;
using TPL.PVZR.Models;
using TPL.PVZR.Tools.SoyoFramework;

namespace TPL.PVZR.Systems.Level_Data
{
    public interface ILevelGridSystem : IServiceManageSystem
    {
    }

    public class LevelGridSystem : AbstractSystem, ILevelGridSystem
    {
        private ILevelGridModel _LevelGridModel;

        protected override void OnInit()
        {
            _LevelGridModel = this.GetModel<ILevelGridModel>();

            this.RegisterEvent<OnPlantSpawned>(e =>
            {
                var targetCell = _LevelGridModel.GetCell(e.CellPos);
                targetCell.CellPlantState = CellPlantState.HavePlant;
                targetCell.Plant = e.Plant;
            });
        }
    }
}