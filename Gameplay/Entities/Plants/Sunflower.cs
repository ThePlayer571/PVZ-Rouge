using TPL.PVZR.Gameplay.Data;
using TPL.PVZR.Gameplay.Entities.Plants.Base;
using UnityEngine;

namespace TPL.PVZR.Gameplay.Entities.Plants
{
	public partial class Sunflower : Plant
	{
		
		/// <summary>
		/// Behavior
		/// </summary>
		
		protected float coldTime;
		protected float timer;
		
		protected override void DefaultAI()
		{
			timer += Time.deltaTime;
			if (timer > coldTime)
			{
				timer -= coldTime;
				_EntitySystem.CreateSunBySunflower(transform.position);
			}
		}

		protected override void OnAwake()
		{
			coldTime = Global.sunflowerColdTime;
			timer = 0;
		}
	}
}
