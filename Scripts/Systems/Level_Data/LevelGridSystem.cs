using QFramework;
using TPL.PVZR.Classes.DataClasses_InLevel;
using TPL.PVZR.CommandEvents.Level_Gameplay.PlantSpawn;
using TPL.PVZR.Helpers.New.DataReader;
using TPL.PVZR.Models;
using TPL.PVZR.Tools.SoyoFramework;

namespace TPL.PVZR.Systems.Level_Data
{
    public interface ILevelGridSystem : ISystem
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
                targetCell.SetPlant(e.Plant, PlantConfigReader.GetPlacementSlotInCell(e.Plant.Def));
            });
            
            this.RegisterEvent<OnPlantRemoved>(e =>
            {
                var targetCell = _LevelGridModel.GetCell(e.Plant.CellPos);
                targetCell.SetPlant(null, PlantConfigReader.GetPlacementSlotInCell(e.Plant.Def));
            });
        }
    }
}