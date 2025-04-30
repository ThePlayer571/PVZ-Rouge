using TPL.PVZR.Gameplay.Entities.Plants.Base;

namespace TPL.PVZR.Gameplay.Entities.Plants
{
    public class CabbagePult : CabbagePultBase
    {
        protected override void Throw()
        {
            _EntitySystem.CreateICabbage(ProjectileIdentifier.Cabbage, transform.position, direction);
            base.Throw();
        }
    }
}