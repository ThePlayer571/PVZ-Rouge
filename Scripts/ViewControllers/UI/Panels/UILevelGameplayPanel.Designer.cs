using System;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace TPL.PVZR.ViewControllers.UI
{
	// Generate Id:729337eb-4689-49a1-a27d-82f31e739d77
	public partial class UILevelGameplayPanel
	{
		public const string Name = "UILevelGameplayPanel";
		
		[SerializeField]
		public RectTransform SlotPanel;
		[SerializeField]
		public UnityEngine.UI.Button FoldPanel;
		[SerializeField]
		public UnityEngine.RectTransform SeedSlots;
		[SerializeField]
		public RectTransform StateBarPanel;
		
		private UILevelGameplayPanelData mPrivateData = null;
		
		protected override void ClearUIComponents()
		{
			SlotPanel = null;
			FoldPanel = null;
			SeedSlots = null;
			StateBarPanel = null;
			
			mData = null;
		}
		
		public UILevelGameplayPanelData Data
		{
			get
			{
				return mData;
			}
		}
		
		UILevelGameplayPanelData mData
		{
			get
			{
				return mPrivateData ?? (mPrivateData = new UILevelGameplayPanelData());
			}
			set
			{
				mUIData = value;
				mPrivateData = value;
			}
		}
	}
}
