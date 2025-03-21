using System;
using UnityEngine;
using QFramework;
using DG.Tweening;
using Random = UnityEngine.Random;

namespace TPL.PVZR
{
	public interface IInteractable
	{
		void Interact();
	}
	public partial class EndLevelLootChest : ViewController, IController, IInteractable
	{
		private InteractableIndicator indicator;
		
		public void Interact()
		{
			var _LevelSystem = this.GetSystem<ILevelSystem>();
			// TODO： 测试代码，记得删
			_LevelSystem.levelState.ChangeState(LevelSystem.LevelState.EndGameplay);
			if (_LevelSystem.levelState.CurrentStateId != LevelSystem.LevelState.EndGameplay)
			{
				throw new Exception("在不正确的时间调用了");
			}
			_LevelSystem.levelState.ChangeState(LevelSystem.LevelState.ChooseLoots);
			indicator.SetSelectable(false);
		}

		private void Awake()
		{
			indicator = GetComponentInChildren<InteractableIndicator>();
			indicator.SetFather(this);
			// 出场动画
			var targetPosition = this.transform.position + new Vector3(Random.Range(-1f,1f), Random.Range(-0.1f,-0.4f), 0);
			transform.DOJump(targetPosition, 0.5f, 1, 0.5f);
		}

		public IArchitecture GetArchitecture()
		{
			return PVZRouge.Interface;
		}
	}
}
