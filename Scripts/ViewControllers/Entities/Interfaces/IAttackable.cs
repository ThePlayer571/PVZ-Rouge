using TPL.PVZR.Classes.LevelStuff;

namespace TPL.PVZR.ViewControllers.Entities
{
    public interface IAttackable : IEntity
    {
        AttackData TakeAttack(AttackData attackData);
    }
}