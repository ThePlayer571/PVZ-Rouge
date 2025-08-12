using System;
using QFramework;
using TPL.PVZR.CommandEvents._NotClassified_;
using TPL.PVZR.Models;
using TPL.PVZR.Services;
using TPL.PVZR.Tools.Save;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.UI;

namespace TPL.PVZR.ViewControllers.UI.MainMenuScene
{
    public class MainTombstonePanelController : MonoBehaviour, IController
    {
        [SerializeField] private Button StartBtn;
        [SerializeField] private Button ContinueBtn;

        private ISaveService _SaveService;
        private IPhaseModel _PhaseModel;

        private void Awake()
        {
            _SaveService = this.GetService<ISaveService>();
            _PhaseModel = this.GetModel<IPhaseModel>();

            // 初始化按钮状态
            if (!_SaveService.SaveManager.Exists(SaveManager.GetFileName(SavePathId.GameData)))
            {
                ContinueBtn.interactable = false;
            }

            // 订阅按钮事件
            StartBtn.onClick.AddListener(() => { UIKit.GetPanel<UIMainMenuPanel>().GameSelectPanel.ShowUI(); });

            ContinueBtn.onClick.AddListener(() => { UIKit.GetPanel<UIMainMenuPanel>().GameContinuePanel.ShowUI(); });
        }

        private void OnDestroy()
        {
            // 取消订阅按钮事件
            StartBtn.onClick.RemoveAllListeners();
            ContinueBtn.onClick.RemoveAllListeners();
        }


        public IArchitecture GetArchitecture()
        {
            return PVZRouge.Interface;
        }
    }
}