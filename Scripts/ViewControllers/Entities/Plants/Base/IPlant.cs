using TPL.PVZR.Classes;
using TPL.PVZR.Tools;
using TPL.PVZR.ViewControllers.Entities.EntityBase.Interfaces;

namespace TPL.PVZR.ViewControllers.Entities.Plants.Base
{
    public interface IPlant : IEntity, IAttackable
    {
        PlantId Id { get; }
        Direction2 Direction { get; }
        float HealthPoint { get; }

        void Initialize(Direction2 direction);
    }
}