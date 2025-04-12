using System;
using DG.Tweening;
using QFramework;
using TPL.PVZR.Architecture;
using TPL.PVZR.Architecture.Systems.PhaseSystems;
using TPL.PVZR.Core.Random;
using TPL.PVZR.Gameplay.ViewControllers.AutoInteractBase;
using UnityEngine;

namespace TPL.PVZR.Gameplay.ViewControllers.InLevel
{
	public partial class EndLevelLootChest : ViewController, IController, IInteractable
	{
		[SerializeField]
		private InteractorButton indicator;
		
		public void Interact()
		{
			var _GamePhaseSystem = this.GetSystem<IGamePhaseSystem>();
			if (_GamePhaseSystem.currentGamePhase is not GamePhaseSystem.GamePhase.AllEnemyKilled)
			{
				throw new Exception($"在不正确的时间打开箱子：{_GamePhaseSystem.currentGamePhase}");
			}
			_GamePhaseSystem.ChangePhase(GamePhaseSystem.GamePhase.ChooseLoots);
			indicator.SetSelectable(false);
		}
		
		private void Awake()
		{
			// 出场动画
			var targetPosition = this.transform.position + new Vector3(RandomHelper.Default.Range(-1f,1f), RandomHelper.Default.Range(-0.4f,-0.1f), 0);
			transform.DOJump(targetPosition, 0.5f, 1, 0.5f);
		}
		
		public IArchitecture GetArchitecture()
		{
			return PVZRouge.Interface;
		}
	}
}
