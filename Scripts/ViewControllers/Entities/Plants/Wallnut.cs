using TPL.PVZR.Classes.GameStuff;
using TPL.PVZR.Core;

namespace TPL.PVZR.ViewControllers.Entities.Plants
{
    public sealed class Wallnut : Plant

    {
        public override PlantId Id { get; } = PlantId.Wallnut;

        public override void Initialize(Direction2 direction)
        {
            base.Initialize(direction);
            HealthPoint = GlobalEntityData.Plant_Wallnut_Health;
        }
    }
}