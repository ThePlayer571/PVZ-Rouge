using TPL.PVZR.Classes.GameStuff;
using TPL.PVZR.Tools;
using TPL.PVZR.ViewControllers.Entities.Interfaces;

namespace TPL.PVZR.ViewControllers.Entities.Plants
{
    public interface IPlant : IEntity, IAttackable
    {
        PlantId Id { get; }
        Direction2 Direction { get; }
        float HealthPoint { get; }

        void Initialize(Direction2 direction);
    }
}