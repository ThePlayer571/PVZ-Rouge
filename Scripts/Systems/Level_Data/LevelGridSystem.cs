using System.Linq;
using QFramework;
using TPL.PVZR.Classes.DataClasses_InLevel;
using TPL.PVZR.CommandEvents.Level_Gameplay.PlantSpawn;
using TPL.PVZR.CommandEvents.Level_Shit;
using TPL.PVZR.Helpers.New.DataReader;
using TPL.PVZR.Models;
using TPL.PVZR.Services;
using TPL.PVZR.Systems.Level_Event;
using TPL.PVZR.Tools.SoyoFramework;

namespace TPL.PVZR.Systems.Level_Data
{
    public interface ILevelGridSystem : ISystem
    {
    }

    public class LevelGridSystem : AbstractSystem, ILevelGridSystem
    {
        private IZombieAISystem _ZombieAISystem;
        private ILevelGridModel _LevelGridModel;
        private ICellTileService _CellTileService;


        protected override void OnInit()
        {
            _ZombieAISystem = this.GetSystem<IZombieAISystem>();
            _LevelGridModel = this.GetModel<ILevelGridModel>();
            _CellTileService = this.GetService<ICellTileService>();

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

            this.RegisterEvent<OnGravestoneSpawned>(e =>
            {
                _LevelGridModel.SetTile(e.CellPos.x, e.CellPos.y, CellTileState.Gravestone);
            });


            this.RegisterEvent<OnExploded>(e =>
            {
                if (e.RemoveLadder)
                {
                    foreach (var cellPos in e.AffectedCells.Where(cellPos => _LevelGridModel.IsValidPos(cellPos)))
                    {
                        _CellTileService.TryRemoveLadder(cellPos);
                    }

                    _ZombieAISystem.ZombieAIUnit.RebakeDirty = true;
                }
            });
        }
    }
}