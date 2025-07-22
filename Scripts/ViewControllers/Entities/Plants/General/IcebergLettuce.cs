using System;
using QFramework;
using TPL.PVZR.Classes;
using TPL.PVZR.Classes.DataClasses_InLevel.Attack;
using TPL.PVZR.Classes.InfoClasses;
using TPL.PVZR.Helpers.New.ClassCreator;
using TPL.PVZR.Tools;
using TPL.PVZR.ViewControllers.Entities.EntityBase.Interfaces;
using TPL.PVZR.ViewControllers.Entities.Plants.Base;
using UnityEngine;

namespace TPL.PVZR.ViewControllers.Entities.Plants
{
    public sealed class IcebergLettuce : Plant
    {
        public override PlantDef Def { get; } = new PlantDef(PlantId.IcebergLettuce, PlantVariant.V0);

        [SerializeField] private CollisionDetector ZombieDetector;

        public override AttackData TakeAttack(AttackData attackData)
        {
            Freeze();
            Kill();
            return null;
        }

        protected override void OnUpdate()
        {
            if (ZombieDetector.HasTarget)
            {
                Freeze();
                Kill();
            }
        }

        protected override void OnInit()
        {
            HealthPoint = GlobalEntityData.Plant_Default_Health;
        }

        private void Freeze()
        {
            var attackData = AttackCreator.CreateAttackData(AttackId.IcebergLettuceFreeze);

            var targets = Physics2D.OverlapCircleAll(
                transform.position,
                GlobalEntityData.Plant_IcebergLettuce_FreezeRadius,
                LayerMask.GetMask("Zombie"));

            foreach (var target in targets)
            {
                var _ = new AttackData(attackData).WithPunchFrom(this.transform.position);
                target.GetComponent<IAttackable>().TakeAttack(_);
            }
        }
    }
}