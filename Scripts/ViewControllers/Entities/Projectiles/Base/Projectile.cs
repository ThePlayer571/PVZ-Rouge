using QFramework;
using TPL.PVZR.Classes.DataClasses_InLevel.Attack;
using TPL.PVZR.Classes.InfoClasses;
using TPL.PVZR.Services;
using TPL.PVZR.ViewControllers.Entities.EntityBase;

namespace TPL.PVZR.ViewControllers.Entities.Projectiles
{
    public abstract class Projectile : Entity, IProjectile
    {
        public abstract ProjectileId Id { get; }
        
        protected override void Awake()
        {
            base.Awake();
        }

        public override void DieWith(AttackData attackData)
        {
            var projectileService = this.GetService<IProjectileService>();
            projectileService.RemoveProjectile(this);
        }
    }

}