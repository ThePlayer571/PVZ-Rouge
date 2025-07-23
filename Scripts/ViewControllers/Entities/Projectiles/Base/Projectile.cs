using TPL.PVZR.ViewControllers.Entities.EntityBase;
using UnityEngine;

namespace TPL.PVZR.ViewControllers.Entities.Projectiles
{
    public abstract class Projectile : Entity, IProjectile
    {
        protected override void Awake()
        {
            base.Awake();
        }

    }

}