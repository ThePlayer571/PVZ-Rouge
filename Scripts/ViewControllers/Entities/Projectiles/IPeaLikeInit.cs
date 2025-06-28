using TPL.PVZR.Tools;

namespace TPL.PVZR.ViewControllers.Entities.Projectiles
{
    public interface IPeaLikeInit : IProjectile
    {
        void Initialize(Direction2 direction);
    }
}