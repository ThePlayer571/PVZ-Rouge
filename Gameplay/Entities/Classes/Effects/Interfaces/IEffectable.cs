using TPL.PVZR.Gameplay.Entities;

namespace TPL.PVZR.Gameplay.Class.Effects
{
    public interface IEffectable:IEntity
    {
        void GiveEffect(IEffect effect);
    }
}