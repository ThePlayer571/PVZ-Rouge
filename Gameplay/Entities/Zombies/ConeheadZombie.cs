using TPL.PVZR.Gameplay.Class;
using TPL.PVZR.Gameplay.Class.Tags;
using TPL.PVZR.Gameplay.Entities.Zombies.Base;
using TPL.PVZR.Gameplay.ViewControllers.ZombieArmor.Base;

namespace TPL.PVZR.Gameplay.Entities.Zombies
{
	public sealed partial class ConeheadZombie : ArmoredZombie
	{
		protected override void OnAwake()
		{
			tagGroup.Add(Tag.ConeheadZombie);
		}
	}
}
