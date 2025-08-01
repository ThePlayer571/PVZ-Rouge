using DG.Tweening;
using TPL.PVZR.Classes.DataClasses_InLevel.Attack;
using TPL.PVZR.Classes.InfoClasses;
using TPL.PVZR.Helpers.New.ClassCreator;
using TPL.PVZR.ViewControllers.Entities.EntityBase.Interfaces;
using TPL.PVZR.ViewControllers.Entities.Plants.Base;
using UnityEngine;

namespace TPL.PVZR.ViewControllers.Entities.Plants
{
    public sealed class CherryBomb : Plant
    {
        public override PlantDef Def { get; } = new PlantDef(PlantId.CherryBomb, PlantVariant.V0);

        protected override void OnInit()
        {
            transform.DOScale(transform.localScale * 1.5f, 0.3f).onComplete = () =>
            {
                var targets = Physics2D.OverlapCircleAll(
                    transform.position,
                    GlobalEntityData.Plant_CherryBomb_ExplosionRadius,
                    LayerMask.GetMask("Zombie"));

                var attackTemplate = AttackCreator.CreateAttackData(AttackId.CherryBombExplosion);
                foreach (var target in targets)
                {
                    var attackData = new AttackData(attackTemplate);
                    target.GetComponent<IAttackable>().TakeAttack(attackData);
                }

                Kill();
            };
        }

        public override AttackData TakeAttack(AttackData attackData)
        {
            return null;
        }
    }
}