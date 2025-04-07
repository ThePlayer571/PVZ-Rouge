using DG.Tweening;
using QFramework;
using TPL.PVZR.Architecture.Models;
using UnityEngine;

namespace TPL.PVZR.Gameplay.ViewControllers.InLevel
{
	public partial class Sun : ViewController
	{
		bool picked = false;
		private void Start()
		{
			ActionKit.Delay(2f, Pick).Start(this);
		}

		private void Pick()
		{
			if (!picked)
			{
				picked = true;
				gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
				transform.DOMoveY(transform.position.y + 1f, 1f);
				GetComponent<SpriteRenderer>().DOFade(0f, 1f);
				this.GetModel<ILevelModel>().sunpoint.Value += 25;
				ActionKit.Delay(1.1f,gameObject.DestroySelf).Start(this);
			}
		}

		private void OnMouseEnter()
		{
			Pick();
		}
	}
}
