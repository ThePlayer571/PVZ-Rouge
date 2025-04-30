using TPL.PVZR.Gameplay.Entities.Plants.Base;

namespace TPL.PVZR.Gameplay.Entities.Plants
{
	public partial class PeaShooter : PeashooterBase
	{
		protected override void Shoot()
		{
			_EntitySystem.CreateIPea(ProjectileIdentifier.Pea,FirePoint.position, direction);
			base.Shoot();
		}
	}
}
