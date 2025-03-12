using UnityEngine;
using UnityEngine.UI;
using QFramework;
using DG.Tweening;

namespace TPL.PVZR
{
	public class UILevelChooseCardPanelData : UIPanelData
	{
	}
	public partial class UILevelChooseCardPanel : UIPanel,IController
	{
		protected override void OnInit(IUIData uiData = null)
		{
			mData = uiData as UILevelChooseCardPanelData ?? new UILevelChooseCardPanelData();
			// please add init code here
			BtnViewWorld.onClick.AddListener(() =>
			{
				if (isHiding)
				{
					Show();
				}
				else
				{
					Hide();
				}
			});
			BtnStartGame.onClick.AddListener(() =>
			{
				"call func".LogInfo();
				this.GetSystem<LevelSystem>().levelState.ChangeState(LevelSystem.LevelState.Gameplay);
			});
		}
		
		protected override void OnOpen(IUIData uiData = null)
		{
		}
		
		protected override void OnShow()
		{
		}
		
		protected override void OnHide()
		{
		}
		
		protected override void OnClose()
		{
			BtnViewWorld.onClick.RemoveAllListeners();
			BtnStartGame.onClick.RemoveAllListeners();
		}

		private bool isHiding = false;
		public new void Show()
		{
			ChosenCards.DOAnchorPosY(0,0.2f);
			Inventory.DOAnchorPosX(0,0.2f);
			isHiding =false;
		}
		public new void Hide()
		{
			ChosenCards.DOAnchorPosY(200,0.2f);
			Inventory.DOAnchorPosX(-920,0.2f);
			isHiding = true;
		}

		public IArchitecture GetArchitecture()
		{
			return PVZRouge.Interface;
		}
	}
}
