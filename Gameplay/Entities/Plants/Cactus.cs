using TPL.PVZR.Gameplay.Entities.Plants.Base;

namespace TPL.PVZR.Gameplay.Entities.Plants
{
    public sealed partial class Cactus:PeashooterBase
    {
        protected override void Shoot()
        {
            _EntitySystem.CreateIPea(ProjectileIdentifier.Spike, FirePoint.position, direction);
            base.Shoot();
        }
    }
}