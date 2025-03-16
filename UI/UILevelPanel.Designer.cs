using System;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace TPL.PVZR
{
	// Generate Id:7ba32422-4367-4812-8b89-8338530d79b9
	public partial class UILevelPanel
	{
		public const string Name = "UILevelPanel";
		
		[SerializeField]
		public RectTransform Seeds;
		[SerializeField]
		public TMPro.TextMeshProUGUI SunpointText;
		[SerializeField]
		public UnityEngine.UI.Button Shovel;
		[SerializeField]
		public UnityEngine.UI.Slider LevelStageBar;
		[SerializeField]
		public RectTransform Flags;
		[SerializeField]
		public RectTransform FlagMask;
		[SerializeField]
		public UnityEngine.GameObject FlagTemplate;
		
		private UILevelPanelData mPrivateData = null;
		
		protected override void ClearUIComponents()
		{
			Seeds = null;
			SunpointText = null;
			Shovel = null;
			LevelStageBar = null;
			Flags = null;
			FlagMask = null;
			FlagTemplate = null;
			
			mData = null;
		}
		
		public UILevelPanelData Data
		{
			get
			{
				return mData;
			}
		}
		
		UILevelPanelData mData
		{
			get
			{
				return mPrivateData ?? (mPrivateData = new UILevelPanelData());
			}
			set
			{
				mUIData = value;
				mPrivateData = value;
			}
		}
	}
}
