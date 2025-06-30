using QFramework;
using TPL.PVZR.Classes;
using TPL.PVZR.Classes.DataClasses;
using TPL.PVZR.Classes.DataClasses.Attack;
using TPL.PVZR.Models;
using TPL.PVZR.Tools;
using TPL.PVZR.ViewControllers.Entities.EntityBase;

namespace TPL.PVZR.ViewControllers.Entities.Plants
{
    public abstract class Plant : Entity, IPlant
    {
        public abstract PlantId Id { get; }
        public Direction2 Direction { get; protected set; }
        public float HealthPoint { get; protected set; }

        public virtual void Initialize(Direction2 direction)
        {
            this.Direction = direction;

            gameObject.LocalScaleX(direction.ToInt());
        }

        public AttackData TakeAttack(AttackData attackData)
        {
            HealthPoint -= attackData.Damage;
            return null;
        }

        public override void Remove()
        {
            var LevelGridModel = this.GetModel<ILevelGridModel>();
            var cell = LevelGridModel.GetCell(CellPos);
            cell.CellPlantState = CellPlantState.Empty;
            cell.Plant = null;

            base.Remove();
        }
    }
}