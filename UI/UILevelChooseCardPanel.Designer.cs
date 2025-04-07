using UnityEngine;

namespace TPL.PVZR.UI
{
	// Generate Id:ecb2d244-b00a-4720-8e22-0acb169d1354
	public partial class UILevelChooseCardPanel
	{
		public const string Name = "UILevelChooseCardPanel";
		
		[SerializeField]
		public UnityEngine.RectTransform Inventory;
		[SerializeField]
		public UnityEngine.RectTransform ChosenCards;
		[SerializeField]
		public UnityEngine.UI.Button BtnViewWorld;
		[SerializeField]
		public UnityEngine.UI.Button BtnStartGame;
		
		private UILevelChooseCardPanelData mPrivateData = null;
		
		protected override void ClearUIComponents()
		{
			Inventory = null;
			ChosenCards = null;
			BtnViewWorld = null;
			BtnStartGame = null;
			
			mData = null;
		}
		
		public UILevelChooseCardPanelData Data
		{
			get
			{
				return mData;
			}
		}
		
		UILevelChooseCardPanelData mData
		{
			get
			{
				return mPrivateData ?? (mPrivateData = new UILevelChooseCardPanelData());
			}
			set
			{
				mUIData = value;
				mPrivateData = value;
			}
		}
	}
}
