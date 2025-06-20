using System;
using System.Collections.Generic;
using QFramework;
using TPL.PVZR.Classes.Game;
using TPL.PVZR.Classes.Level;
using TPL.PVZR.Classes.MazeMap;
using TPL.PVZR.Classes.MazeMap.Instances.DaveHouse;
using TPL.PVZR.Events;
using TPL.PVZR.Helpers;
using TPL.PVZR.Models;
using TPL.PVZR.ViewControllers.Managers;
using TPL.PVZR.ViewControllers.UI;
using UnityEngine.SceneManagement;

namespace TPL.PVZR.Systems
{
    public interface IGamePhaseSystem : ISystem
    {
    }

    public class GamePhaseSystem : AbstractSystem, IGamePhaseSystem
    {
        private IPhaseModel _PhaseModel;
        private IGameModel _GameModel;

        protected override void OnInit()
        {
            _PhaseModel = this.GetModel<IPhaseModel>();
            _GameModel = this.GetModel<IGameModel>();

            this.RegisterEvent<OnLeavePhaseEvent>(e =>
            {
                switch (e.leaveFromPhase)
                {
                    case GamePhase.MainMenu:
                        UIKit.ClosePanel<UIGameStartPanel>();
                        break;
                }
            });
            this.RegisterEvent<OnEnterPhaseEarlyEvent>(e =>
            {
                switch (e.changeToPhase)
                {
                    case GamePhase.GameInitialization:

                        _GameModel.GameData = e.parameters["GameData"] as GameData;
                        break;

                    case GamePhase.MazeMapInitialization:
                        SceneManager.LoadScene("MazeMapScene");
                        SceneManager.GetActiveScene().name.LogInfo();
                        _GameModel.MazeMapController = new DaveHouseMazeMapController(_GameModel.GameData.MazeMapData);
                        break;
                }
            });

            this.RegisterEvent<OnEnterPhaseEvent>(e =>
            {
                switch (e.changeToPhase)
                {
                    case GamePhase.MazeMapInitialization:
                        ActionKit.Sequence()
                            .DelayFrame(1) // 等待场景加载
                            .Callback(() =>
                            {
                                SceneManager.GetActiveScene().name.LogInfo();
                                _GameModel.MazeMapController.SetMazeMapTiles();
                            }).Start(GameManager.Instance);
                        break;
                }
            });
            this.RegisterEvent<OnEnterPhaseLateEvent>(e =>
            {
                switch (e.changeToPhase)
                {
                    case GamePhase.GameInitialization:
                        _PhaseModel.DelayChangePhase(GamePhase.MazeMapInitialization);
                        break;
                    case GamePhase.MazeMapInitialization:
                        ActionKit.Sequence()
                            .DelayFrame(2)
                            .Callback(() => { _PhaseModel.DelayChangePhase(GamePhase.MazeMap); })
                            .Delay(1)
                            .Callback(() =>
                            {
                                _PhaseModel.DelayChangePhase(GamePhase.LevelPreInitialization,
                                    new Dictionary<string, object>
                                        { { "LevelData", new LevelData() } });
                            })
                            .Start(GameManager.Instance);
                        break;
                    case GamePhase.MazeMap:
                        break;
                }
            });
        }
    }
}