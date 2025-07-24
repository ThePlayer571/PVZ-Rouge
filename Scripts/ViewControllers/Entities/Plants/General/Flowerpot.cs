using QFramework;
using TPL.PVZR.Classes;
using TPL.PVZR.Classes.DataClasses_InLevel;
using TPL.PVZR.Classes.DataClasses;
using TPL.PVZR.Classes.InfoClasses;
using TPL.PVZR.CommandEvents.Level_Gameplay.PlantSpawn;
using TPL.PVZR.Models;
using TPL.PVZR.Tools;
using TPL.PVZR.ViewControllers.Entities.Plants.Base;

namespace TPL.PVZR.ViewControllers.Entities.Plants
{
    public sealed class Flowerpot : Plant
    {
        public override PlantDef Def { get; } = new PlantDef(PlantId.Flowerpot, PlantVariant.V0);

        protected override void OnInit()
        {
            HealthPoint = GlobalEntityData.Plant_Default_Health;
        }

        public override void OnRemoved()
        {
            var upCellPos = CellPos.Up();
            var LevelGridModel = this.GetModel<ILevelGridModel>();
            if (LevelGridModel.IsValidPos(upCellPos))
            {
                var cell = LevelGridModel.GetCell(upCellPos);
                if (cell.CellPlantData.HasPlant())
                {
                    foreach (var plant in cell.CellPlantData)
                    {
                        plant.Kill();
                    }
                }
            }
        }
    }
}