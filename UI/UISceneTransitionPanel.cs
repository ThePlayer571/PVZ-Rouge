using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace TPL.PVZR
{
	public class UISceneTransitionPanelData : UIPanelData
	{
	}
	public partial class UISceneTransitionPanel : UIPanel
	{
		protected override void OnInit(IUIData uiData = null)
		{
			mData = uiData as UISceneTransitionPanelData ?? new UISceneTransitionPanelData();
			// please add init code here
		}
		
		protected override void OnOpen(IUIData uiData = null)
		{
		}
		
		protected override void OnShow()
		{
		}
		
		protected override void OnHide()
		{
		}
		
		protected override void OnClose()
		{
		}
	}
}
