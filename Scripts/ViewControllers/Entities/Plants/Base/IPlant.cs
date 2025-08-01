using TPL.PVZR.Classes.InfoClasses;
using TPL.PVZR.Tools;
using TPL.PVZR.ViewControllers.Entities.EntityBase.Interfaces;

namespace TPL.PVZR.ViewControllers.Entities.Plants.Base
{
    public interface IPlant : IEntity, IAttackable
    {
        PlantDef Def { get; }
        Direction2 Direction { get; }
        float HealthPoint { get; }

        void Initialize(Direction2 direction);
    }

    public interface ICanBeStackedOn : IPlant
    {
        bool CanStack(PlantDef plantDef);
        void StackAdd();
    }
}