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
    public sealed class PotatoMine : Plant
    {
        public override PlantDef Def { get; } = new PlantDef(PlantId.PotatoMine, PlantVariant.V0);

        [SerializeField] private CollisionDetector ZombieDetector;
        [SerializeField] private Animator _Animator;

        private bool _grown = false;

        public override AttackData TakeAttack(AttackData attackData)
        {
            return _grown ? null : base.TakeAttack(attackData);
        }

        protected override void Update()
        {
            base.Update();
            if (_grown && ZombieDetector.HasTarget)
            {
                Boom();
                Kill();
            }
        }

        protected override void OnInit()
        {
            HealthPoint = GlobalEntityData.Plant_Default_Health;

            ActionKit.Sequence()
                .Delay(1f /*GlobalEntityData.Plant_PotatoMine_GrowTime*/)
                .Callback(() =>
                {
                    _Animator.SetTrigger("Grow");
                    _grown = true;
                })
                .Start(this);
        }

        private void Boom()
        {
            var attackData = AttackCreator.CreateAttackData(AttackId.PotatoMineExplosion);

            var targets = Physics2D.OverlapCircleAll(
                transform.position,
                GlobalEntityData.Plant_PotatoMine_ExplosionRadius,
                LayerMask.GetMask("Zombie"));

            foreach (var target in targets)
            {
                var _ = new AttackData(attackData).WithPunchFrom(this.transform.position);
                target.GetComponent<IAttackable>().TakeAttack(_);
            }
        }
    }
}