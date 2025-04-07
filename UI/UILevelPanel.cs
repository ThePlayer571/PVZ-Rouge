using System.Collections.Generic;
using DG.Tweening;
using QFramework;
using TPL.PVZR.Architecture;
using TPL.PVZR.Architecture.Events;
using TPL.PVZR.Architecture.Models;
using TPL.PVZR.Architecture.Systems;
using TPL.PVZR.Core;
using TPL.PVZR.Gameplay.Class.Items;
using TPL.PVZR.Gameplay.ViewControllers.UI;
using UnityEngine;

namespace TPL.PVZR.UI
{
	public class UILevelPanelData : UIPanelData
	{
	}
	public partial class UILevelPanel : UIPanel,IController
	{
		private ILevelModel _LevelModel;
		private InputSystem _InputSystem;
		private RectTransform rectTransform;

		protected override void OnInit(IUIData uiData = null)
		{
			mData = uiData as UILevelPanelData ?? new UILevelPanelData();
			// please add init code here
			rectTransform = GetComponent<RectTransform>();
			_LevelModel = this.GetModel<ILevelModel>();
			_InputSystem = this.GetSystem<InputSystem>();
			// 阳光显示
			_LevelModel.sunpoint.RegisterWithInitValue(val => { SunpointText.text = _LevelModel.sunpoint.ToString(); })
				.UnRegisterWhenGameObjectDestroyed(this);
			// 关卡进度条显示
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
			// 铁铲订阅事件(不想给铁铲单独写一个类，就写在这里了)
			Shovel.onClick.AddListener(_InputSystem.TriggerOnShovelButtonClick);
		}


		# region 显示/隐藏
		public void HideInstant()
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
# endregion
		public new void Init()
		{
			// seed生成
			int i = 1;
			foreach (var eachChosenCard in _LevelModel.chosenCards)
			{
				var go = Card.CreateSeed(eachChosenCard.cardSO.seedSO);
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
			Shovel.onClick.RemoveAllListeners();
		}

		public IArchitecture GetArchitecture()
		{
			return PVZRouge.Interface;
		}

	}
}
