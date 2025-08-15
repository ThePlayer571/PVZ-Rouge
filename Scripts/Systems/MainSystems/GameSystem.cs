using System;
using QFramework;
using TPL.PVZR.Classes.DataClasses.Game;
using TPL.PVZR.Classes.DataClasses.Item.Card;
using TPL.PVZR.CommandEvents.Game_EventsForTip;
using TPL.PVZR.Helpers.New;
using TPL.PVZR.Models;
using TPL.PVZR.Services;
using TPL.PVZR.Tools.Random;
using TPL.PVZR.Tools.SoyoFramework;
using TPL.PVZR.ViewControllers.UI;
using UnityEngine;

namespace TPL.PVZR.Systems
{
    public interface IGameSystem : IMainSystem
    {
    }

    public class GameSystem : AbstractSystem, IGameSystem
    {
        private IPhaseModel _PhaseModel;
        private IGameModel _GameModel;

        private IPhaseService _PhaseService;
        private ISceneTransitionEffectService _SceneTransitionEffectService;
        private IUIStackService _UIStackService;
        private IGamePhaseChangeService _GamePhaseChangeService;

        protected override void OnInit()
        {
            _PhaseModel = this.GetModel<IPhaseModel>();
            _GameModel = this.GetModel<IGameModel>();

            _PhaseService = this.GetService<IPhaseService>();
            _SceneTransitionEffectService = this.GetService<ISceneTransitionEffectService>();
            _UIStackService = this.GetService<IUIStackService>();
            _GamePhaseChangeService = this.GetService<IGamePhaseChangeService>();

            _PhaseService.RegisterCallBack((GamePhase.GameInitialization, PhaseStage.EnterEarly), e =>
            {
                // 数据设置
                _GameModel.GameData = e.Paras["GameData"] as IGameData;
                // 本质史山：为了其他地方的代码优雅而设
                PlantDefHelper.SetInventory(_GameModel.GameData.InventoryData);
                RandomHelper.SetGame(_GameModel.GameData);
                // 常驻Panel
                UIKit.OpenPanel<UIGamePausePanel>(UILevel.PopUI);
                // 过场动画
                var task = _SceneTransitionEffectService.PlayTransitionAsync();
                _PhaseService.AddAwait(task);
                //
                _PhaseService.ChangePhase(GamePhase.MazeMapInitialization);
            });

            _PhaseService.RegisterCallBack((GamePhase.GameExiting, PhaseStage.EnterNormal), e =>
            {
                // 回到主菜单
                _PhaseService.ChangePhase(GamePhase.MainMenu);
            });

            _PhaseService.RegisterCallBack((GamePhase.GameExiting, PhaseStage.LeaveNormal), e =>
            {
                // 卸载数据
                _GameModel.Reset();
                // 卸载常驻UI
                UIKit.ClosePanel<UIGamePausePanel>();
                // 本质史山：为了其他地方的代码优雅而设
                PlantDefHelper.SetInventory(null);
                RandomHelper.SetGame(null);
            });

            #region Pause

            this.RegisterEvent<OnGamePaused>(_ =>
            {
                // 时间
                Time.timeScale = 0;
                // panel显示
                var panel = UIKit.GetPanel<UIGamePausePanel>();
                panel.ShowUI();
            });

            this.RegisterEvent<OnGameResumed>(_ =>
            {
                // 时间
                Time.timeScale = 1;
                // panel隐藏
                var panel = UIKit.GetPanel<UIGamePausePanel>();
                panel.HideUI();
            });

            this.RegisterEvent<OnTransitionEffectHoldingBegin>(e =>
            {
                _UIStackService.Clear();
                if (_GameModel.IsGamePaused)
                {
                    _GamePhaseChangeService.ResumeGame(true);
                }
            });

            #endregion
        }
    }
}