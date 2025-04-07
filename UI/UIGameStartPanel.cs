using QFramework;
using TPL.PVZR.Architecture;
using TPL.PVZR.Architecture.Commands;
using TPL.PVZR.Architecture.Systems;
using TPL.PVZR.Gameplay.Class;

namespace TPL.PVZR.UI
{
	public class UIGameStartPanelData : UIPanelData
	{
	}
	public partial class UIGameStartPanel : UIPanel, IController
	{
		protected override void OnInit(IUIData uiData = null)
		{
			mData = uiData as UIGameStartPanelData ?? new UIGameStartPanelData();
			// please add init code here
			Button.onClick.AddListener(() =>
            {
	            this.SendCommand<StartNewGameCommand>(new StartNewGameCommand());
			});
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

        public IArchitecture GetArchitecture()
        {
			return PVZRouge.Interface; 
        }
    }
}
