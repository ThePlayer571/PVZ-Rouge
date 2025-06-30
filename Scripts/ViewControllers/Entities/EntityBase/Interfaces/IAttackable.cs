using TPL.PVZR.Classes.DataClasses.Attack;

namespace TPL.PVZR.ViewControllers.Entities.EntityBase.Interfaces
{
    public interface IAttackable : IEntity
    {
        AttackData TakeAttack(AttackData attackData);
    }
}