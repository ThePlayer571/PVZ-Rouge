using TPL.PVZR.Classes.LevelStuff;
using TPL.PVZR.Core;

namespace TPL.PVZR.ViewControllers.Entities.Plants
{
    public abstract class Plant : Entity, IAttackable
    {
        public Direction2 Direction { get; protected set; }
        public float HealthPoint {get; protected set;}

        public virtual void Initialize(Direction2 direction)
        {
            this.Direction = direction;
        }

        public void TakeAttack(Attack attack)
        {
            HealthPoint -= attack.Damage;
        }
    }
}