using System;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace TPL.PVZR
{
	// Generate Id:c86ed9b7-58ad-4641-9fc7-36af931505ab
	public partial class UILevelDefeatPanel
	{
		public const string Name = "UILevelDefeatPanel";
		
		
		private UILevelDefeatPanelData mPrivateData = null;
		
		protected override void ClearUIComponents()
		{
			
			mData = null;
		}
		
		public UILevelDefeatPanelData Data
		{
			get
			{
				return mData;
			}
		}
		
		UILevelDefeatPanelData mData
		{
			get
			{
				return mPrivateData ?? (mPrivateData = new UILevelDefeatPanelData());
			}
			set
			{
				mUIData = value;
				mPrivateData = value;
			}
		}
	}
}
