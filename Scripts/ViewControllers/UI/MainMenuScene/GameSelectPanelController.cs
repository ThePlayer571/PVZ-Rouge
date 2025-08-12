using System;
using DG.Tweening;
using QFramework;
using TMPro;
using TPL.PVZR.Classes.DataClasses.Game;
using TPL.PVZR.Classes.MazeMap;
using TPL.PVZR.CommandEvents._NotClassified_;
using TPL.PVZR.Helpers.New;
using TPL.PVZR.Services;
using UnityEngine;
using UnityEngine.UI;

namespace TPL.PVZR.ViewControllers.UI.MainMenuScene
{
    public class GameSelectPanelController : MonoBehaviour, IController, ICanPutInUIStack
    {
        // View
        [SerializeField] private RectTransform View;
        [SerializeField] private CanvasGroup MainCanvasGroup;
        [SerializeField] private RectTransform BgTransform;
        [SerializeField] private Button BlackMaskBtn;

        // Input
        [SerializeField] private Button StartGameBtn;
        [SerializeField] private TMP_InputField SeedInputField;

        [SerializeField] private Toggle GameModeToggle_N0;
        [SerializeField] private Toggle GameModeToggle_N1;
        [SerializeField] private Toggle GameModeToggle_Debug;

        // 引用
        private IUIStackService _UIStackService;

        // 变量
        private ulong? _inputSeed = null;

        private void Awake()
        {
            //
            _UIStackService = this.GetService<IUIStackService>();

            // 初始化View
            View.Show();
            HideUIInstantly();

            // 绑定按钮
            // 点击黑色背景关闭界面
            BlackMaskBtn.onClick.AddListener(HideUI);
            // 种子输入框
            SeedInputField.onValueChanged.AddListener(str =>
            {
                string filtered = "";
                foreach (char c in str)
                {
                    if (Uri.IsHexDigit(c)) filtered += c;
                    if (filtered.Length >= 8) break;
                }

                filtered = filtered.ToUpper();

                if (filtered != str)
                    SeedInputField.text = filtered;
            });

            SeedInputField.onEndEdit.AddListener(str =>
            {
                if (str.Length == 0) return;
                str = str.PadLeft(8, '0');
                if (SeedHelper.ValidateInput(str))
                {
                    _inputSeed = SeedHelper.ParseFrom(str);
                    SeedInputField.text = str;
                }
                else
                {
                    throw new Exception("怎么会触发呢？已经确保是正确的输入了");
                }
            });
            // 开始游戏
            StartGameBtn.onClick.AddListener(() =>
            {
                GameDef gameDef;
                if (GameModeToggle_N0.isOn)
                {
                    gameDef = new GameDef { GameDifficulty = GameDifficulty.N0 };
                }
                else if (GameModeToggle_N1.isOn)
                {
                    gameDef = new GameDef { GameDifficulty = GameDifficulty.N1 };
                }
                else if (GameModeToggle_Debug.isOn)
                {
                    gameDef = new GameDef { GameDifficulty = GameDifficulty.Test };
                }
                else
                {
                    $"尝试开始游戏，但没有选择游戏模式".LogError();
                    return;
                }

                // 开始游戏
                if (_inputSeed.HasValue) this.SendCommand(new StartNewGameCommand(gameDef, _inputSeed.Value));
                else this.SendCommand(new StartNewGameCommand(gameDef));
            });
        }

        private void OnDestroy()
        {
            // 清理事件
            BlackMaskBtn.onClick.RemoveAllListeners();
            StartGameBtn.onClick.RemoveAllListeners();
            SeedInputField.onValueChanged.RemoveAllListeners();
            SeedInputField.onEndEdit.RemoveAllListeners();
        }

        #region Display操作

        public void ShowUI()
        {
            _UIStackService.PushPanel(this);
            MainCanvasGroup.blocksRaycasts = true;
            MainCanvasGroup.DOFade(1, 0.2f).SetUpdate(true);
            BgTransform.DOAnchorPosY(0, 0.2f).SetUpdate(true);
        }

        private void HideUI()
        {
            _UIStackService.PopIfTop(this);
            MainCanvasGroup.blocksRaycasts = false;
            MainCanvasGroup.DOFade(0, 0.2f).SetUpdate(true);
            BgTransform.DOAnchorPosY(-80, 0.2f).SetUpdate(true);
        }

        private void HideUIInstantly()
        {
            _UIStackService.PopIfTop(this);
            MainCanvasGroup.blocksRaycasts = false;
            MainCanvasGroup.alpha = 0;
            BgTransform.anchoredPosition = new Vector2(BgTransform.anchoredPosition.x, -80);
        }

        #endregion

        public IArchitecture GetArchitecture()
        {
            return PVZRouge.Interface;
        }

        void ICanPutInUIStack.OnPopped()
        {
            HideUI();
        }
    }
}