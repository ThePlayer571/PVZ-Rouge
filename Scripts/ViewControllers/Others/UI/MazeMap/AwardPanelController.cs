using System;
using System.Collections.Generic;
using QFramework;
using TPL.PVZR.CommandEvents.__NewlyAdded__;
using TPL.PVZR.Helpers;
using TPL.PVZR.Systems;
using UnityEngine;
using UnityEngine.UI;

namespace TPL.PVZR.ViewControllers.Others.UI.MazeMap
{
    public class AwardPanelController : MonoBehaviour, IController
    {
        [SerializeField] private Toggle toggle;
        [SerializeField] private RectTransform View;
        [SerializeField] private RectTransform AwardChoices;
        [SerializeField] private GameObject AwardChoicePrefab;


        private IAwardSystem _AwardSystem;
        private List<Button> choices;

        private void Awake()
        {
            _AwardSystem = this.GetSystem<IAwardSystem>();
            choices = new List<Button>();
        }

        private void Start()
        {
            toggle.onValueChanged.AddListener(Display);

            if (!_AwardSystem.HasAward) return;
            for (int index = 0; index < 3; index++)
            {
                // 创建Choice节点
                var Choice = AwardChoicePrefab.Instantiate().GetComponent<Button>();
                Choice.transform.SetParent(AwardChoices, false);
                var AwardView = Choice.transform.Find("AwardView");
                Choice.Show();

                choices.Add(Choice);

                // 初始化Choice的View
                var loots = _AwardSystem.GetLootGroupByIndex(index);
                foreach (var loot in loots)
                {
                    var go = ItemViewFactory.CreateItemView(loot);
                    go.transform.SetParent(AwardView, false);
                }

                // 订阅点击事件
                var capturedIndex = index;
                Choice.onClick.AddListener(() =>
                {
                    this.SendCommand<ChooseAwardCommand>(new ChooseAwardCommand(capturedIndex));

                    // 被选择对象保持高亮
                    var newColor = Choice.colors;
                    newColor.disabledColor = new Color(0, 0, 0, 0);
                    Choice.colors = newColor; // 设置禁用颜色

                    // 禁用所有按钮
                    foreach (var choice in choices)
                    {
                        choice.interactable = false;
                    }
                });
            }
        }

        private void Display(bool show)
        {
            if (show) View.Show();
            else View.Hide();
        }

        private void OnDestroy()
        {
            toggle.onValueChanged.RemoveListener(Display);

            foreach (var choice in choices)
            {
                choice.onClick.RemoveAllListeners();
            }
        }

        public IArchitecture GetArchitecture()
        {
            return PVZRouge.Interface;
        }
    }
}