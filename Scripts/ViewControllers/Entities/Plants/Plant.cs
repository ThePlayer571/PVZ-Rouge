using QFramework;
using TPL.PVZR.Classes.GameStuff;
using TPL.PVZR.Classes.LevelStuff;
using TPL.PVZR.Core;
using TPL.PVZR.Models;

namespace TPL.PVZR.ViewControllers.Entities.Plants
{
    public abstract class Plant : Entity, IAttackable
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