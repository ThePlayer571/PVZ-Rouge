using System;
using QFramework;
using UnityEngine;

namespace TPL.PVZR.Gameplay.ViewControllers.InLevel
{
	public partial class ZombieAttackArea : ViewController
	{

		public event Action<Collider2D> OnTriggerEnterEvent;
		public event Action<Collider2D> OnTriggerExitEvent;

		private void OnTriggerEnter2D(Collider2D other)
		{
			OnTriggerEnterEvent?.Invoke(other);
		}

		private void OnTriggerExit2D(Collider2D other)
		{
			OnTriggerExitEvent?.Invoke(other);
		}
	}
}
