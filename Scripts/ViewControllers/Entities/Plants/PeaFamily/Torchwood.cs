using System.Collections.Generic;
using QFramework;
using TPL.PVZR.Classes.InfoClasses;
using TPL.PVZR.Services;
using TPL.PVZR.Tools;
using TPL.PVZR.ViewControllers.Entities.Plants.Base;
using TPL.PVZR.ViewControllers.Entities.Projectiles;
using UnityEngine;

namespace TPL.PVZR.ViewControllers.Entities.Plants
{
    public sealed class Torchwood : Plant
    {
        public override PlantDef Def { get; } = new PlantDef(PlantId.Torchwood, PlantVariant.V0);

        private HashSet<int> _ignitedProjectiles = new();

        protected override void OnInit()
        {
            this.HealthPoint = GlobalEntityData.Plant_Default_Health;
            ProjectileDetector.TargetPredicate = collider2D =>
            {
                return collider2D.GetComponent<ICanBeIgnited>() != null;
            };

            ProjectileDetector.OnTargetEnter.Register(collider2D =>
            {
                var projectile = collider2D.GetComponent<Projectile>();
                var id = projectile.EntityId;

                if (_ignitedProjectiles.Contains(id)) return;
                _ignitedProjectiles.Add(id);

                _ProjectileService.Ignite((ICanBeIgnited)projectile, IgnitionType.Fire);

                ActionKit.Delay(1f, () => _ignitedProjectiles.Remove(id)).Start(this);
            }).UnRegisterWhenGameObjectDestroyed(this);
        }

        [SerializeField] private TriggerDetector ProjectileDetector;

        protected override void OnUpdate()
        {
        }
    }
}