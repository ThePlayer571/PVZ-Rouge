using QFramework;
using TPL.PVZR.Classes.DataClasses_InLevel.Attack;
using TPL.PVZR.Classes.InfoClasses;
using TPL.PVZR.Services;
using TPL.PVZR.Tools;
using TPL.PVZR.ViewControllers.Entities.EntityBase;
using UnityEngine;

namespace TPL.PVZR.ViewControllers.Entities.Projectiles
{
    public abstract class Projectile : Entity, IProjectile
    {
        public abstract ProjectileId Id { get; }
        protected Timer _autoKillSelfTimer;

        protected IProjectileService _ProjectileService { get; private set; }

        protected override void Awake()
        {
            base.Awake();
            _ProjectileService = this.GetService<IProjectileService>();

            _autoKillSelfTimer = new Timer(GlobalEntityData.Projectile_Default_AutoKillSelfTime);
        }

        protected override void Update()
        {
            base.Update();
            _autoKillSelfTimer.Update(Time.deltaTime);
            if (_autoKillSelfTimer.Ready)
            {
                Kill();
            }
        }

        public override void DieWith(AttackData attackData)
        {
            var projectileService = this.GetService<IProjectileService>();
            projectileService.RemoveProjectile(this);
        }
    }
}