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
                var targetCell = e.Plant.AttachedCell;
                targetCell.CellPlantData.SetPlant(e.Plant, PlantConfigReader.GetPlacementSlot(e.Plant.Def));
            });

            this.RegisterEvent<OnPlantRemoved>(e =>
            {
                var targetCell = e.Plant.AttachedCell;
                targetCell.CellPlantData.SetPlant(null, PlantConfigReader.GetPlacementSlot(e.Plant.Def));
            });
        }
    }
}