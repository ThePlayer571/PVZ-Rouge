using QFramework;
using TPL.PVZR.Classes.DataClasses_InLevel;
using TPL.PVZR.Classes.DataClasses_InLevel.Attack;
using TPL.PVZR.Classes.InfoClasses;
using TPL.PVZR.Models;
using TPL.PVZR.Tools;
using TPL.PVZR.ViewControllers.Entities.Plants.Base;

namespace TPL.PVZR.ViewControllers.Entities.Plants
{
    public sealed class Flowerpot : Plant
    {
        public override PlantDef Def { get; } = new PlantDef(PlantId.Flowerpot, PlantVariant.V0);

        private ILevelGridModel _LevelGridModel;

        protected override void OnInit()
        {
            HealthPoint = GlobalEntityData.Plant_Default_Health;
        }

        public override AttackData TakeAttack(AttackData attackData)
        {
            // var upCellPos = AttachedCell.Position.Up();
            // if (_LevelGridModel.IsValidPos(upCellPos))
            // {
            //     var cell = _LevelGridModel.GetCell(upCellPos);
            //     var plant = cell.CellPlantData.GetPlant(PlacementSlot.Normal);
            //     if (plant.Def.Id != PlantId.Flowerpot)
            //     {
            //         //todo 如果这个植物不是放在花盆上的，不应该有这个逻辑
            //         plant.TakeAttack(attackData);
            //     }
            // }
            return base.TakeAttack(attackData);
        }

        public override void OnRemoved()
        {
            var upCellPos = AttachedCell.Position.Up();
            if (_LevelGridModel.IsValidPos(upCellPos))
            {
                var cell = _LevelGridModel.GetCell(upCellPos);
                if (cell.CellPlantData.HasPlant())
                {
                    foreach (var plant in cell.CellPlantData)
                    {
                        //todo 如果这个植物不是放在花盆上的，不应该有这个逻辑
                        plant.Kill();
                    }
                }
            }
        }
    }
}