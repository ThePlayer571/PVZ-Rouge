using System;
using System.Collections.Generic;
using QFramework;
using TPL.PVZR.Architecture;
using TPL.PVZR.Architecture.Systems.InGame;
using TPL.PVZR.Gameplay.Class;

namespace TPL.PVZR.UI
{
	public class UILevelChooseLootPanelData : UIPanelData, ICanGetSystem
	{
		public List<Loot> Choice_1;
		public List<Loot> Choice_2;
		public List<Loot> Choice_3;

		public UILevelChooseLootPanelData(float value,List<Loot> LootDataList )
		{
			ILootCreateSystem _LootCreateSystem = this.GetSystem<ILootCreateSystem>();
			Choice_1 = _LootCreateSystem.CreateSetOfLootData(value, LootDataList);
			Choice_2 = _LootCreateSystem.CreateSetOfLootData(value, LootDataList);
			Choice_3 = _LootCreateSystem.CreateSetOfLootData(value, LootDataList);
		}

		public UILevelChooseLootPanelData()
		{
			
		}

		public IArchitecture GetArchitecture()
		{
			return PVZRouge.Interface;
		}
	}
	public partial class UILevelChooseLootPanel : UIPanel
	{
		protected override void OnInit(IUIData uiData = null)
		{
			mData = uiData as UILevelChooseLootPanelData ?? new UILevelChooseLootPanelData();
			// please add init code here
			if (mData.Choice_1.IsNull() || mData.Choice_2.IsNull() || mData.Choice_3.IsNull())
			{
				throw new NotImplementedException("未给出三个Loot");
			}
			// 设置显示
			Choice_1.Init(mData.Choice_1);
			Choice_2.Init(mData.Choice_2);
			Choice_3.Init(mData.Choice_3);

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
		}
	}
}
