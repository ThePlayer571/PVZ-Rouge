using TPL.PVZR.Classes.DataClasses_InLevel.Attack;
using TPL.PVZR.Classes.InfoClasses;
using TPL.PVZR.ViewControllers.Entities.Plants.Base;
using UnityEngine;

namespace TPL.PVZR.ViewControllers.Entities.Plants
{
    public sealed class Pumpkin : Plant
    {
        public override PlantDef Def { get; } = new PlantDef(PlantId.Pumpkin, PlantVariant.V0);

        protected override void OnInit()
        {
            HealthPoint = GlobalEntityData.Plant_Wallnut_Health;
        }

        public override AttackData TakeAttack(AttackData attackData)
        {
            HealthPoint = Mathf.Clamp(HealthPoint - attackData.Damage, 0, Mathf.Infinity);
            if (HealthPoint <= 0) DieWith(attackData);
            return null;
        }
    }
}