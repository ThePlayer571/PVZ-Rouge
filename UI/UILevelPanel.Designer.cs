using UnityEngine;

namespace TPL.PVZR.UI
{
	// Generate Id:a1dde997-63d8-4034-8f34-132933c86579
	public partial class UILevelPanel
	{
		public const string Name = "UILevelPanel";
		
		[SerializeField]
		public UnityEngine.RectTransform SeedPanel;
		[SerializeField]
		public RectTransform Seeds;
		[SerializeField]
		public TMPro.TextMeshProUGUI SunpointText;
		[SerializeField]
		public UnityEngine.RectTransform ShovelSlot;
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
			SeedPanel = null;
			Seeds = null;
			SunpointText = null;
			ShovelSlot = null;
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
