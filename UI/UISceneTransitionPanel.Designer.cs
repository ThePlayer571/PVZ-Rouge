using UnityEngine;

namespace TPL.PVZR.UI
{
	// Generate Id:9d0aaf32-3564-492f-a2c0-650fe1557669
	public partial class UISceneTransitionPanel
	{
		public const string Name = "UISceneTransitionPanel";
		
		[SerializeField]
		public UnityEngine.UI.Image Bg;
		
		private UISceneTransitionPanelData mPrivateData = null;
		
		protected override void ClearUIComponents()
		{
			Bg = null;
			
			mData = null;
		}
		
		public UISceneTransitionPanelData Data
		{
			get
			{
				return mData;
			}
		}
		
		UISceneTransitionPanelData mData
		{
			get
			{
				return mPrivateData ?? (mPrivateData = new UISceneTransitionPanelData());
			}
			set
			{
				mUIData = value;
				mPrivateData = value;
			}
		}
	}
}
