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
			
			// 观察世界按钮
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
			// 开始游戏按钮
			BtnStartGame.onClick.AddListener(() =>
			{
				this.GetSystem<LevelSystem>().levelState.ChangeState(LevelSystem.LevelState.Gameplay);
			});
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

		public void Init()
		{
			var _InventorySystem = this.GetSystem<InventorySystem>();
			var _ChooseCardSystem = this.GetSystem<IChooseCardSystem>();
			foreach (CardDataSO cardData in _InventorySystem.cardsInInventory)
			{
				var go = _ChooseCardSystem.CreateCard(cardData);
				go.transform.SetParent(Inventory);
			}
			// 设置选卡栏的长度
			ChosenCards.offsetMax = new Vector2(this.GetModel<ILevelModel>().maxCardCount * 100 + 25 * 2,ChosenCards.offsetMax.y)     ;

		}
		
		
		
		
		
		
		
		
		
		
		
		
		
		
		// 没用的

		public IArchitecture GetArchitecture()
		{
			return PVZRouge.Interface;
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
	}
}
