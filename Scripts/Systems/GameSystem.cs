using System;
using QFramework;
using TPL.PVZR.Classes.Game;
using TPL.PVZR.Classes.MazeMap;
using TPL.PVZR.Classes.MazeMap.Instances.DaveHouse;
using TPL.PVZR.Events;
using TPL.PVZR.Models;
using TPL.PVZR.ViewControllers.Managers;
using TPL.PVZR.ViewControllers.UI;
using UnityEngine.SceneManagement;

namespace TPL.PVZR.Systems
{
    public interface IGameSystem : ISystem
    {
    }

    public class GameSystem : AbstractSystem, IGameSystem
    {
        #region Public

        #region Model

        public IMazeMapController MazeMapController { get; private set; }

        #endregion

        #endregion

        #region Private

        private IPhaseModel _PhaseModel;
        private GameData _GameData;

        #endregion

        protected override void OnInit()
        {
            _PhaseModel = this.GetModel<IPhaseModel>();
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

                        _GameData = e.parameters["GameData"] as GameData;
                        // StartGame(GameData);
                        break;

                    case GamePhase.MazeMapInitialization:
                        SceneManager.LoadScene("MazeMapScene");
                        SceneManager.GetActiveScene().name.LogInfo();
                        MazeMapController =
                            new DaveHouseMazeMapController(new MazeMapData(new DaveHouseMazeMapDefinition(),
                                _GameData.seed));
                        break;
                }
            });

            this.RegisterEvent<OnEnterPhaseEvent>(e =>
            {
                switch (e.changeToPhase)
                {
                    case GamePhase.MazeMapInitialization:
                        ActionKit.Sequence()
                            .Callback(() =>
                            {
                                "call this".LogInfo();
                            })
                            .DelayFrame(1) // 等待场景加载
                            .Callback(() =>
                            {
                                SceneManager.GetActiveScene().name.LogInfo();
                                MazeMapController.SetMazeMapTiles();
                            }).Start(GameManager.Instance);
                        break;
                }
            });
            this.RegisterEvent<OnEnterPhaseLateEvent>(e =>
            {
                if (e.changeToPhase == GamePhase.GameInitialization)
                {
                    _PhaseModel.DelayChangePhase(GamePhase.MazeMapInitialization);
                }
            });
        }
    }
}