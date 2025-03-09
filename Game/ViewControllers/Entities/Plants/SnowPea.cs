using UnityEngine;
using QFramework;

namespace TPL.PVZR.EntityPlant
{
	public partial class SnowPea : PeashooterBase
	{
		protected override void Shoot()
		{
			_EntityCreateSystem.CreatePea(ProjectileIdentifier.IcePea,FirePoint.position, direction);
			base.Shoot();
		}
	}
}
