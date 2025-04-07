using System.Collections.Generic;
using DG.Tweening;
using QFramework;
using TMPro;
using TPL.PVZR.Architecture;
using TPL.PVZR.Architecture.Systems;
using TPL.PVZR.Architecture.Systems.InGame;
using TPL.PVZR.Architecture.Systems.PhaseSystems;
using TPL.PVZR.Gameplay.Class;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace TPL.PVZR.Gameplay.ViewControllers.UI
{
	public partial class UILevelChooseLootChoice : ViewController,IController,IPointerEnterHandler,IPointerExitHandler,IPointerClickHandler
	{
		private Image _image;
		private List<Loot> _allLoots;
		private void Awake()
		{
			_image = GetComponent<Image>();
		}

		
		
		// UI显示
		public void Init(List<Loot> allLoots)
		{
			_allLoots = allLoots;
			//
			foreach (var each in _allLoots)
			{
				if (each.lootType == Loot.LootType.Card)
				{
					var go = CardOnlyView.Create(each.cardSO);
					go.transform.SetParent(transform);
				}
				
			}
			
			// UI 显示
			_image.DOColor(Color.gray, 0f);
			foreach (var each in this.GetComponentsInChildren<Image>())
			{
				each.DOColor(Color.gray, 0f);
			}
			foreach (var each in this.GetComponentsInChildren<TextMeshProUGUI>())
			{
				each.DOColor(Color.gray, 0f);
			}
		}

		public void OnPointerEnter(PointerEventData eventData)
		{
			_image.DOColor(Color.white, 0.125f).SetEase(Ease.InOutQuad);
			foreach (var each in this.GetComponentsInChildren<Image>())
			{
				each.DOColor(Color.white, 0.125f).SetEase(Ease.InOutQuad);
			}
			foreach (var each in this.GetComponentsInChildren<TextMeshProUGUI>())
			{
				each.DOColor(Color.white, 0.125f).SetEase(Ease.InOutQuad);
			}
		}

		public void OnPointerExit(PointerEventData eventData)
		{
			_image.DOColor(Color.gray, 0.125f).SetEase(Ease.InOutQuad);
			foreach (var each in this.GetComponentsInChildren<Image>())
			{
				each.DOColor(Color.gray, 0.125f).SetEase(Ease.InOutQuad);
			}
			foreach (var each in this.GetComponentsInChildren<TextMeshProUGUI>())
			{
				each.DOColor(Color.gray, 0.125f).SetEase(Ease.InOutQuad);
			}
		}

		public void OnPointerClick(PointerEventData eventData)
		{
			this.GetSystem<IInventorySystem>().AddLoot(_allLoots);
			this.GetSystem<IGamePhaseSystem>().ChangePhase(GamePhaseSystem.GamePhase.LevelExiting);
		}
		
		
		
		
		public IArchitecture GetArchitecture()
		{
			return PVZRouge.Interface;
		}
	}
}
