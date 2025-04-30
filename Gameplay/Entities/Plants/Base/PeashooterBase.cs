using QFramework;
using TPL.PVZR.Core;
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
		
		// 植物属性
		protected float shootColdTime = Global.peashooterColdTime;
		protected PeashooterState peashooterState = PeashooterState.InCold;
		protected float shootTimer;

		// 植物行为
		protected override void DefaultAI()
		{
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
				RaycastHit2D hit = Physics2D.Raycast(transform.position, direction.ToVector2(), Global.peashooterRange,
					LayerMask.GetMask("Zombie", "Barrier","ZombieShield"));
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
	}
}
