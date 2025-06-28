using TPL.PVZR.Classes.GameStuff;
using TPL.PVZR.Core;

namespace TPL.PVZR.ViewControllers.Entities.Plants
{
    public sealed class Flowerpot:Plant
    {
        public override PlantId Id { get; } = PlantId.Flowerpot;

        public override void Initialize(Direction2 direction)
        {
            base.Initialize(direction);
            HealthPoint = GlobalEntityData.Plant_Default_Health;
        }
    }
}