using TPL.PVZR.Gameplay.Entities.Plants.Base;

namespace TPL.PVZR.Gameplay.Entities.Plants
{
	public partial class Wallnut : Plant
	{
		protected override void Awake()
		{
			initialHealthPoint = 5000;
			base.Awake();
		}
	}
}
