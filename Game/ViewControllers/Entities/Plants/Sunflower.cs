using UnityEngine;
using QFramework;

namespace TPL.PVZR.EntityPlant
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
			base.DefaultAI();
			//
			timer += Time.deltaTime;
			if (timer > coldTime)
			{
				timer -= coldTime;
				_EntitySystem.CreateSunBySunflower(transform.position);
			}
		}

		protected override void Awake()
		{
			base.Awake();
			//
			coldTime = Global.sunflowerColdTime;
			timer = 0;
		}
	}
}
