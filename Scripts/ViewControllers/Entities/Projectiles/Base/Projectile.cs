using TPL.PVZR.Classes.InfoClasses;
using TPL.PVZR.ViewControllers.Entities.EntityBase;
using UnityEngine;

namespace TPL.PVZR.ViewControllers.Entities.Projectiles
{
    public abstract class Projectile : Entity, IProjectile
    {
        public abstract ProjectileId Id { get; }
        
        protected override void Awake()
        {
            base.Awake();
        }

    }

}