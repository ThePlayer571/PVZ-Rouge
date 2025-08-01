using UnityEngine;

namespace TPL.PVZR.ViewControllers.Entities.Projectiles
{
    public interface IPeaLikeInit : IProjectile
    {
        void Initialize(Vector2 direction);
    }
}