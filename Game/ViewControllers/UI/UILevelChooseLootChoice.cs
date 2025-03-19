using System;
using System.Collections.Generic;
using UnityEngine;
using QFramework;
using UnityEngine.EventSystems;
using DG.Tweening;
using TMPro;
using UnityEditor.Search;
using UnityEngine.UI;

namespace TPL.PVZR
{
	public partial class UILevelChooseLootChoice : ViewController,IController,IPointerEnterHandler,IPointerExitHandler,IPointerClickHandler
	{
		private Image _image;
		private List<LootData> _allLoots;
		private void Awake()
		{
			_image = GetComponent<Image>();
		}

		
		
		// UI显示
		public void Init(List<LootData> allLoots)
		{
			_allLoots = allLoots;
			//
			foreach (var each in _allLoots)
			{
				if (each.lootType == LootData.LootType.Card)
				{
					var go = CardOnlyView.Create(each.cardData);
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
			this.GetSystem<ILevelSystem>().levelState.ChangeState(LevelSystem.LevelState.Exiting);
		}
		
		
		
		
		public IArchitecture GetArchitecture()
		{
			return PVZRouge.Interface;
		}
	}
}
