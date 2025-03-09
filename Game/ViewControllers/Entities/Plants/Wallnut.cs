using UnityEngine;
using QFramework;

namespace TPL.PVZR.EntityPlant
{
	public partial class Wallnut : Plant
	{
		protected override void Awake()
		{
			base.Awake();
			//
			healthPoint.SetValueWithoutEvent(500);
		}
	}
}
