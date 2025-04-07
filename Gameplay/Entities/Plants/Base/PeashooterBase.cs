using TPL.PVZR.Gameplay.Data;
using UnityEngine;

namespace TPL.PVZR.Gameplay.Entities.Plants.Base
{
	public abstract class PeashooterBase : Plant
	{
		protected enum PeashooterState
		{
			InCold, Ready
		}
		/// <summary>
		/// Behavior
		/// </summary>

		// 植物属性
		protected float shootColdTime;
		protected PeashooterState peashooterState;
		protected float shootTimer;

		// 植物行为
		protected override void DefaultAI()
		{
			base.DefaultAI();
			//
			if (peashooterState == PeashooterState.InCold)
			{
				shootTimer+=Time.deltaTime;
				if (shootTimer >= shootColdTime)
				{
					peashooterState = PeashooterState.Ready;
				}
			}

			if (peashooterState == PeashooterState.Ready)
			{
				RaycastHit2D hit = Physics2D.Raycast(transform.position, this.directionVector, Global.peashooterRange,
					LayerMask.GetMask("Zombie", "Barrier"));
				if (hit.collider && hit.collider.CompareTag("Zombie"))
				{
					Shoot();
				}
			}
		}
		protected virtual void Shoot()
		{
			// 创建实体
			peashooterState = PeashooterState.InCold;
			shootTimer = 0;
		}
		// 初始化

		protected override void Awake()
		{
			base.Awake();
			//
			shootColdTime = Global.peashooterColdTime;
			peashooterState = PeashooterState.Ready;
			shootTimer = 0;
		}
	}
}
