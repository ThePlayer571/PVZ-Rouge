using System;
using QFramework;
using TPL.PVZR.Classes.DataClasses.Game;
using TPL.PVZR.Classes.DataClasses.Item.Card;
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

        protected override void OnInit()
        {
            _PhaseModel = this.GetModel<IPhaseModel>();
            _GameModel = this.GetModel<IGameModel>();

            var phaseService = this.GetService<IPhaseService>();

            phaseService.RegisterCallBack((GamePhase.GameInitialization, PhaseStage.EnterEarly), e =>
            {
                // 数据设置
                _GameModel.GameData = e.Paras["GameData"] as IGameData;
                // 本质史山：为了其他地方的代码优雅而设
                PlantDefHelper.SetInventory(_GameModel.GameData.InventoryData);
                RandomHelper.SetGame(_GameModel.GameData);
                // 常驻Panel
                UIKit.OpenPanel<UIGamePausePanel>(UILevel.PopUI);
                //
                phaseService.ChangePhase(GamePhase.MazeMapInitialization);
            });

            phaseService.RegisterCallBack((GamePhase.GameExiting, PhaseStage.EnterNormal), e =>
            {
                // 回到主菜单
                phaseService.ChangePhase(GamePhase.MainMenu);
            });

            phaseService.RegisterCallBack((GamePhase.GameExiting, PhaseStage.LeaveNormal), e =>
            {
                // 卸载数据
                _GameModel.Reset();
                // 卸载常驻UI
                UIKit.ClosePanel<UIGamePausePanel>();
                // 本质史山：为了其他地方的代码优雅而设
                PlantDefHelper.SetInventory(null);
                RandomHelper.SetGame(null);
            });

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
        }
    }
}