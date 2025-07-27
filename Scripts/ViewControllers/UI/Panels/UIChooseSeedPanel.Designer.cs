using UnityEngine;

namespace TPL.PVZR.ViewControllers.UI
{
	// Generate Id:c16dae4d-76b4-4512-95e6-66a7fb5651b0
	public partial class UIChooseSeedPanel
	{
		public const string Name = "UIChooseSeedPanel";
		
		[SerializeField]
		public UnityEngine.RectTransform InventorySeeds;
		[SerializeField]
		public UnityEngine.RectTransform ChosenSeeds;
		[SerializeField]
		public UnityEngine.UI.Button ViewTheMapBtn;
		[SerializeField]
		public UnityEngine.UI.Button StartGameBtn;
		
		private UIChooseSeedPanelData mPrivateData = null;
		
		protected override void ClearUIComponents()
		{
			InventorySeeds = null;
			ChosenSeeds = null;
			ViewTheMapBtn = null;
			StartGameBtn = null;
			
			mData = null;
		}
		
		public UIChooseSeedPanelData Data
		{
			get
			{
				return mData;
			}
		}
		
		UIChooseSeedPanelData mData
		{
			get
			{
				return mPrivateData ?? (mPrivateData = new UIChooseSeedPanelData());
			}
			set
			{
				mUIData = value;
				mPrivateData = value;
			}
		}
	}
}
