using QFramework;

namespace TPL.PVZR.UI
{
	public class UILevelDefeatPanelData : UIPanelData
	{
	}
	public partial class UILevelDefeatPanel : UIPanel
	{
		protected override void OnInit(IUIData uiData = null)
		{
			mData = uiData as UILevelDefeatPanelData ?? new UILevelDefeatPanelData();
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
