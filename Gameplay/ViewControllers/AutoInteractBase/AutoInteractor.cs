using System;
using QFramework;
using TPL.PVZR.Architecture;
using UnityEngine;

namespace TPL.PVZR.Gameplay.ViewControllers.AutoInteractBase
{
	public abstract class AutoInteractor : ViewController, IController
	{
		# region 私有
		protected IInteractable _callTarget;
		protected bool _highlightable = true; // 是否能被高亮 
		protected readonly BindableProperty<bool> _highlighted = new(false); // 是否被高亮

		protected virtual void Awake()
		{
			_callTarget = callTarget as IInteractable;
			if (_callTarget == null) throw new ArgumentException("callTarget未实现IInteractable");
		}

		#endregion
		
		[SerializeField] public MonoBehaviour callTarget;
		public bool interactable => _highlightable;
		public BindableProperty<bool> highlighted => _highlighted;
		# region 公有方法
		public void SetSelectable(bool selectable)
		{
			this._highlightable = selectable;
			if (selectable == false)
			{
				_highlighted.Value = false;
			}
		}
		/// <summary>
		/// 调用callTarget的Interact
		/// </summary>
		public void Interact()
		{
			_callTarget.Interact();
		}
		#endregion

		
		
		
		public IArchitecture GetArchitecture()
		{
			return PVZRouge.Interface;
		}

	}
}
