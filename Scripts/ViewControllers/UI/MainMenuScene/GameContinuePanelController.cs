using DG.Tweening;
using QFramework;
using TPL.PVZR.CommandEvents._NotClassified_;
using TPL.PVZR.Services;
using UnityEngine;
using UnityEngine.UI;

namespace TPL.PVZR.ViewControllers.UI.MainMenuScene
{
    public class GameContinuePanelController : MonoBehaviour, IController, ICanPutInUIStack
    {
        // View
        [SerializeField] private RectTransform View;
        [SerializeField] private CanvasGroup MainCanvasGroup;
        [SerializeField] private RectTransform BgTransform;
        [SerializeField] private Button BlackMaskBtn;

        // Input
        [SerializeField] private Button ContinueBtn;

        // 引用
        private IUIStackService _UIStackService;

        private void Awake()
        {
            //
            _UIStackService = this.GetService<IUIStackService>();
            
            // 初始化View
            View.Show();
            HideUIInstantly();
            
            // 绑定按钮
            BlackMaskBtn.onClick.AddListener(HideUI);
            ContinueBtn.onClick.AddListener(() => { this.SendCommand(new ContinueGameCommand()); });
        }

        private void OnDestroy()
        {
            // 清理事件
            BlackMaskBtn.onClick.RemoveAllListeners();
            ContinueBtn.onClick.RemoveAllListeners();
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