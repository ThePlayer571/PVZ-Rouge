using TPL.PVZR.Classes;
using TPL.PVZR.Classes.InfoClasses;
using TPL.PVZR.Tools;
using TPL.PVZR.ViewControllers.Entities.Plants.Base;

namespace TPL.PVZR.ViewControllers.Entities.Plants
{
    public sealed class Tallnut : Plant
    {
        public override PlantDef Def { get; } = new PlantDef(PlantId.Tallnut, PlantVariant.V0);

        protected override void OnInit()
        {
            HealthPoint = GlobalEntityData.Plant_Tallnut_Health;
        }
    }
}