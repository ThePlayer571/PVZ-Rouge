using TPL.PVZR.Classes.DataClasses.Attack;

namespace TPL.PVZR.ViewControllers.Entities.EntityBase.Interfaces
{
    public interface IAttackable
    {
        /// <summary>
        /// 可能修改传入的attackData，请在使用前新建
        /// </summary>
        /// <param name="attackData"></param>
        /// <returns></returns>
        AttackData TakeAttack(AttackData attackData);
    }
}