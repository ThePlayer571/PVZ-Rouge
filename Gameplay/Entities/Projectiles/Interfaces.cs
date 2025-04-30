using TPL.PVZR.Core;

namespace TPL.PVZR.Gameplay.Entities.Projectiles
{
    public interface IPea:IProjectile
    {
        public void Initialize(Direction2 direction);
    }

    public interface ICabbage : IProjectile
    {
        public void Initialize(Direction2 direction);
    }
}