using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using QFramework;
using DG.Tweening;
using Unity.VisualScripting;

namespace TPL.PVZR
{
	public class UILevelPanelData : UIPanelData
	{
	}
	public partial class UILevelPanel : UIPanel,IController
	{
		private ILevelModel _LevelModel;

		protected override void OnInit(IUIData uiData = null)
		{
			mData = uiData as UILevelPanelData ?? new UILevelPanelData();
			// please add init code here
			rectTransform = GetComponent<RectTransform>();
			_LevelModel = this.GetModel<ILevelModel>();
			_LevelModel.sunpoint.RegisterWithInitValue(val => { SunpointText.text = _LevelModel.sunpoint.ToString(); })
				.UnRegisterWhenGameObjectDestroyed(this);
//
			this.RegisterEvent<WaveStartEvent>(@event =>
			{
				LevelStageBar.DOValue((float)@event.wave / _LevelModel.WaveConfig.totalWaveCount, 1f)
					.SetEase(Ease.OutQuad);
				if (_LevelModel.WaveConfig.flaggedWaves.Contains(@event.wave))
				{
					if (FlagDict.TryGetValue(@event.wave, out var flag))
					{
						flag.DOAnchorPosY(19, 1.4f).SetEase(Ease.OutQuad);
					}
				}
			});

		}

		private RectTransform rectTransform;

		public void HideQuick()
		{
			SeedPanel.DOAnchorPosY(100, 0f);
			ShovelSlot.DOAnchorPosY(100, 0f);
			LevelStageBar.GetComponent<RectTransform>().DOAnchorPosY(-100, 0f);
		}
		public new void Hide()
		{
			SeedPanel.DOAnchorPosY(100, 0.2f);
			ShovelSlot.DOAnchorPosY(100, 0.2f);
			LevelStageBar.GetComponent<RectTransform>().DOAnchorPosY(-100, 0f);
		}

		public new void Show()
		{
			SeedPanel.DOAnchorPosY(-100, 0.2f);
			ShovelSlot.DOAnchorPosY(-100, 0.2f);
			LevelStageBar.GetComponent<RectTransform>().DOAnchorPosY(40, 0f);
		}

		public void SetUp()
		{
			// seed生成
			var _ChooseCardSystem = this.GetSystem<IChooseCardSystem>();
			int i = 1;
			foreach (var eachChosenCard in _ChooseCardSystem.chosenCards)
			{
				var go = _ChooseCardSystem.CreateSeed(eachChosenCard.cardData);
				go.GetComponent<Seed>().seedIndex = i++;
				go.transform.SetParent(Seeds);
			}
			// 旗子
			foreach (var flaggedWave in _LevelModel.WaveConfig.flaggedWaves)
			{
				var ratio = (float)flaggedWave / _LevelModel.WaveConfig.totalWaveCount;
				var barLength = Flags.rect.width;
				var newFlag =  FlagTemplate.Instantiate().Parent(FlagMask).GetComponent<RectTransform>();
				newFlag.anchoredPosition = new Vector2(-barLength * ratio, 0);
				newFlag.Show();
				FlagDict[flaggedWave] = newFlag;
			}
		}

		private Dictionary<int, RectTransform> FlagDict = new();
		
		
		
		
		
		
		
		
		
		
		
		
		
		//
		
		
		
		
		
		
		
		
		
		
		
		
		
		
		
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

		public IArchitecture GetArchitecture()
		{
			return PVZRouge.Interface;
		}

	}
}
