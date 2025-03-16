using UnityEngine;
using QFramework;

namespace TPL.PVZR.EntityPlant
{
	public partial class PeaShooter : PeashooterBase
	{
		protected override void Shoot()
		{
			_EntitySystem.CreatePea(ProjectileIdentifier.Pea,FirePoint.position, direction);
			base.Shoot();
		}
	}
}
