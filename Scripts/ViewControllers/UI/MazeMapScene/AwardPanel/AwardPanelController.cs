using System.Collections.Generic;
using QFramework;
using TPL.PVZR.CommandEvents.MazeMap_AwardPanel;
using TPL.PVZR.Helpers.New.GameObjectFactory;
using TPL.PVZR.Systems.MazeMap;
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

            if (_AwardSystem.AwardCount == 0) return;
            // 初始化UI
            for (int index = 0; index < _AwardSystem.AwardCount; index++)
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
                });
            }

            // UI变化事件
            _AwardSystem.IsAwardAvailable.RegisterWithInitValue(val =>
            {
                foreach (var choice in choices)
                {
                    choice.interactable = val;
                }
            }).UnRegisterWhenGameObjectDestroyed(this);

            _AwardSystem.ChosenAwardIndex.RegisterWithInitValue(val =>
            {
                if (val != -1)
                {
                    var chosenChoice = choices[val];
                    // 高亮被选择的按钮
                    var newColor = chosenChoice.colors;
                    newColor.disabledColor = new Color(0, 0, 0, 0); // 设置高亮颜色
                    chosenChoice.colors = newColor;
                }
            }).UnRegisterWhenGameObjectDestroyed(this);
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