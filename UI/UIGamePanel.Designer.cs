using System;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace TPL.PVZR
{
	// Generate Id:2c1dea9f-b940-4a6a-9ac6-fe6579f4d2fb
	public partial class UIGamePanel
	{
		public const string Name = "UIGamePanel";
		
		[SerializeField]
		public RectTransform Seeds;
		[SerializeField]
		public TMPro.TextMeshProUGUI SunpointText;
		[SerializeField]
		public UnityEngine.UI.Button Shovel;
		
		private UIGamePanelData mPrivateData = null;
		
		protected override void ClearUIComponents()
		{
			Seeds = null;
			SunpointText = null;
			Shovel = null;
			
			mData = null;
		}
		
		public UIGamePanelData Data
		{
			get
			{
				return mData;
			}
		}
		
		UIGamePanelData mData
		{
			get
			{
				return mPrivateData ?? (mPrivateData = new UIGamePanelData());
			}
			set
			{
				mUIData = value;
				mPrivateData = value;
			}
		}
	}
}
