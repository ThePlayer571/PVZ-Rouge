using QFramework;
using TPL.PVZR.Classes;
using TPL.PVZR.Classes.InfoClasses;
using TPL.PVZR.Helpers;
using TPL.PVZR.Helpers.New.GameObjectFactory;
using TPL.PVZR.Helpers.New.Methods;
using TPL.PVZR.Tools;
using TPL.PVZR.ViewControllers.Entities.Plants.Base;
using TPL.PVZR.ViewControllers.Entities.Projectiles;
using UnityEngine;
using Time = UnityEngine.Time;

namespace TPL.PVZR.ViewControllers.Entities.Plants
{
    public sealed class Torchwood : Plant
    {
        public override PlantDef Def { get; } = new PlantDef(PlantId.Torchwood, PlantVariant.V0);

        protected override void OnInit()
        {
            this.HealthPoint = GlobalEntityData.Plant_Default_Health;

            ProjectileDetector.OnTargetEnter.Register(collider2D =>
            {
                var canBeIgnited = collider2D.GetComponent<ICanBeIgnited>();
                canBeIgnited?.Ignite(IgnitionType.Fire);
                
            }).UnRegisterWhenGameObjectDestroyed(this);
        }

        [SerializeField] private TriggerDetector ProjectileDetector;

        protected override void OnUpdate()
        {
        }
    }
}