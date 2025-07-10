using TPL.PVZR.Classes;
using TPL.PVZR.Tools;
using TPL.PVZR.ViewControllers.Entities.Plants.Base;

namespace TPL.PVZR.ViewControllers.Entities.Plants
{
    public sealed class Wallnut : Plant

    {
        public override PlantId Id { get; } = PlantId.Wallnut;

        protected override void OnInit()
        {
            HealthPoint = GlobalEntityData.Plant_Wallnut_Health;
        }
    }
}