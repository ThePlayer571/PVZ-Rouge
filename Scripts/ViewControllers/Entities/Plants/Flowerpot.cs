using QFramework;
using TPL.PVZR.Classes;
using TPL.PVZR.Classes.DataClasses;
using TPL.PVZR.Models;
using TPL.PVZR.Tools;

namespace TPL.PVZR.ViewControllers.Entities.Plants
{
    public sealed class Flowerpot : Plant
    {
        public override PlantId Id { get; } = PlantId.Flowerpot;

        public override void Initialize(Direction2 direction)
        {
            base.Initialize(direction);
            HealthPoint = GlobalEntityData.Plant_Default_Health;
        }

        public override void Remove()
        {
            var upCellPos = CellPos.Up();
            var LevelGridModel = this.GetModel<ILevelGridModel>();
            if (LevelGridModel.IsValidPos(upCellPos))
            {
                var cell = LevelGridModel.GetCell(upCellPos);
                if (cell.CellPlantState == CellPlantState.HavePlant)
                {
                    var plant = cell.Plant;
                    plant.Remove();
                }
            }
            base.Remove();
        }
    }
}