using System;
using TPL.PVZR.Gameplay.Class;

namespace TPL.PVZR.Gameplay.Entities
{
    public interface IDamageable:IEntity
    {
        public void TakeDamage(Attack attack);
        public void TakeDamage(Attack attack, out Attack leftAttack)
        {
            throw new NotImplementedException();
        }
    }
}