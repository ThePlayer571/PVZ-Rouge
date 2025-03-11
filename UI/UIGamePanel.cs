using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace TPL.PVZR
{
	public class UIGamePanelData : UIPanelData
	{
	}
	public partial class UIGamePanel : UIPanel,IController
	{
		protected override void OnInit(IUIData uiData = null)
		{
			mData = uiData as UIGamePanelData ?? new UIGamePanelData();
			// please add init code here
			
			//
			ILevelModel _LevelModel = this.GetModel<ILevelModel>();
			_LevelModel.sunpoint.RegisterWithInitValue(val => { SunpointText.text = _LevelModel.sunpoint.ToString(); });


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
