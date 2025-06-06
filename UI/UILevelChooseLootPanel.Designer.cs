using TPL.PVZR.Gameplay.ViewControllers.UI;
using UnityEngine;

namespace TPL.PVZR.UI
{
	// Generate Id:234e9cb9-627e-4788-816f-51c6a57f844d
	public partial class UILevelChooseLootPanel
	{
		public const string Name = "UILevelChooseLootPanel";
		
		[SerializeField]
		public UILevelChooseLootChoice Choice_1;
		[SerializeField]
		public UILevelChooseLootChoice Choice_2;
		[SerializeField]
		public UILevelChooseLootChoice Choice_3;
		
		private UILevelChooseLootPanelData mPrivateData = null;
		
		protected override void ClearUIComponents()
		{
			Choice_1 = null;
			Choice_2 = null;
			Choice_3 = null;
			
			mData = null;
		}
		
		public UILevelChooseLootPanelData Data
		{
			get
			{
				return mData;
			}
		}
		
		UILevelChooseLootPanelData mData
		{
			get
			{
				return mPrivateData ?? (mPrivateData = new UILevelChooseLootPanelData());
			}
			set
			{
				mUIData = value;
				mPrivateData = value;
			}
		}
	}
}
