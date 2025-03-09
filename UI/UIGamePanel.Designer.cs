using System;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace TPL.PVZR
{
	// Generate Id:19588810-68c8-4da2-9fc0-bc8c520ef49e
	public partial class UIGamePanel
	{
		public const string Name = "UIGamePanel";
		
		[SerializeField]
		public UnityEngine.UI.Image Normal;
		[SerializeField]
		public UnityEngine.UI.Image Gray;
		[SerializeField]
		public UnityEngine.UI.Image Mask;
		[SerializeField]
		public UnityEngine.UI.Button Btn;
		[SerializeField]
		public TMPro.TextMeshProUGUI SunpointText;
		[SerializeField]
		public UnityEngine.UI.Button Shovel;
		
		private UIGamePanelData mPrivateData = null;
		
		protected override void ClearUIComponents()
		{
			Normal = null;
			Gray = null;
			Mask = null;
			Btn = null;
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
