using TPL.PVZR.Gameplay.Entities.Plants.Base;

namespace TPL.PVZR.Gameplay.Entities.Plants
{
	public partial class SnowPea : PeashooterBase
	{
		protected override void Shoot()
		{
			_EntitySystem.CreateIPea(ProjectileIdentifier.IcePea,FirePoint.position, direction);
			base.Shoot();
		}
	}
}
