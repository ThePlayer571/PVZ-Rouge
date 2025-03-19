using System;
using DG.Tweening;
using UnityEngine;
using QFramework;

namespace TPL.PVZR
{
	public partial class InteractableIndicator : ViewController, IController, IInteractable
	{
		// 玩家在内会一直选中，但是不在内时不会，组要特殊处理，造成了代码的不好
		public void SetFather(IInteractable father)
		{
			this.father = father;
		}

		public void SetSelectable(bool selectable)
		{
			this.selectable = selectable;
			if (!selectable)
			{
				TryDeselect();
			}
		}
		
		
		
		private bool selected;
		private bool selectable = true;
		private IInteractable father;

		private void OnTriggerStay2D(Collider2D other)
		{
			TrySelect();
		}

		private void OnTriggerExit2D(Collider2D other)
		{   
			TryDeselect();
		}

		private void Awake()
		{
			this.RegisterEvent<InputInteractEvent>((@event) =>
			{
				if (selected)
				{
					Interact();
				}
			});
		}


		private void TrySelect()
		{
			if (selectable && !selected)
			{
				Select();
			}
		}

		private void TryDeselect()
		{
			if (selected)
			{
				Deselect();
			}
		}
		
		
		private void Select()
		{
			if (selectable)
			{
				ShowButton();
				selected = true;
			}
		}
		private void Deselect()
		{
				HideButton();
				selected = false;
		}
		
		private void ShowButton()
		{
			ButtonSprite.DOFade(1f, 0f);
		}

		private void HideButton()
		{
			ButtonSprite.DOFade(0f, 0f);
		}

		public IArchitecture GetArchitecture()
		{
			return PVZRouge.Interface;
		}

		public void Interact()
		{
			father.Interact();
		}
	}
}
