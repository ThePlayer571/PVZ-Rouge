using QFramework;
using TPL.PVZR.Classes.GameStuff;
using TPL.PVZR.Classes.LevelStuff;
using TPL.PVZR.Core;

namespace TPL.PVZR.ViewControllers.Entities.Plants
{
    public abstract class Plant : Entity, IAttackable
    {
        public abstract PlantId Id { get; }
        public Direction2 Direction { get; protected set; }
        public float HealthPoint {get; protected set;}

        public virtual void Initialize(Direction2 direction)
        {
            this.Direction = direction;

            gameObject.LocalScaleX(direction.ToInt());
        }

        public void TakeAttack(Attack attack)
        {
            HealthPoint -= attack.Damage;
        }
    }
}