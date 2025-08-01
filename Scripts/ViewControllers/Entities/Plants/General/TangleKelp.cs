using QFramework;
using TPL.PVZR.Classes.DataClasses_InLevel.Attack;
using TPL.PVZR.Classes.InfoClasses;
using TPL.PVZR.Helpers.New.ClassCreator;
using TPL.PVZR.Tools;
using TPL.PVZR.ViewControllers.Entities.EntityBase.Interfaces;
using TPL.PVZR.ViewControllers.Entities.Plants.Base;
using UnityEngine;

namespace TPL.PVZR.ViewControllers.Entities.Plants
{
    public sealed class TangleKelp : Plant
    {
        public override PlantDef Def { get; } = new PlantDef(PlantId.TangleKelp, PlantVariant.V0);

        [SerializeField] private TriggerDetector ZombieDetector;

        public override AttackData TakeAttack(AttackData attackData)
        {
            return null;
        }

        protected override void OnInit()
        {
            ZombieDetector.OnTargetEnter.Register(other =>
            {
                var attack = AttackCreator.CreateAttackData(AttackId.TangleKelp);
                other.GetComponent<IAttackable>().TakeAttack(attack);

                Kill();
            }).UnRegisterWhenGameObjectDestroyed(this);
        }
    }
}