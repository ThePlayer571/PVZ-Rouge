using UnityEngine;
using QFramework;

namespace TPL.PVZR.EntityPlant
{
	public partial class PeaShooter : PeashooterBase
	{
		protected override void Shoot()
		{
			_EntityCreateSystem.CreatePea(ProjectileIdentifier.Pea,FirePoint.position, direction);
			base.Shoot();
		}
	}
}
