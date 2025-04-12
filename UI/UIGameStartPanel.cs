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
		private ulong? inputSeed = null;
		protected override void OnInit(IUIData uiData = null)
		{
			mData = uiData as UIGameStartPanelData ?? new UIGameStartPanelData();
			// please add init code here
			BtnStart.onClick.AddListener(() =>
            {
	            // 注：以下可能会传进去一个null，但我们的代码能够处理传入的null
	            this.SendCommand<StartNewGameCommand>(new StartNewGameCommand(inputSeed));
			});
			SeedInputField.onEndEdit.AddListener(val =>
			{
				inputSeed = ulong.Parse(val);
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
