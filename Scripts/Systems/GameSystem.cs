using System;
using System.Collections.Generic;
using QFramework;
using TPL.PVZR.Classes.DataClasses.Game;
using TPL.PVZR.Classes.DataClasses.Level;
using TPL.PVZR.Classes.MazeMap;
using TPL.PVZR.Classes.MazeMap.Public.DaveHouse;
using TPL.PVZR.CommandEvents.Phase;
using TPL.PVZR.Helpers;
using TPL.PVZR.Helpers.ClassCreator;
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
        private IPhaseModel _PhaseModel;
        private IGameModel _GameModel;

        protected override void OnInit()
        {
            _PhaseModel = this.GetModel<IPhaseModel>();
            _GameModel = this.GetModel<IGameModel>();

            this.RegisterEvent<OnPhaseChangeEvent>(e =>
            {
                switch (e.GamePhase)
                {
                    case GamePhase.GameInitialization:
                        switch (e.PhaseStage)
                        {
                            case PhaseStage.EnterEarly:
                                _GameModel.GameData = e.Parameters["GameData"] as GameData;
                                break;
                            case PhaseStage.EnterLate:
                                _PhaseModel.DelayChangePhase(GamePhase.MazeMapInitialization);
                                break;
                        }

                        break;
                    case GamePhase.MazeMapInitialization:
                        switch (e.PhaseStage)
                        {
                            case PhaseStage.EnterEarly:
                                SceneManager.LoadScene("MazeMapScene");
                                _GameModel.MazeMapController =
                                    new DaveHouseMazeMapController(_GameModel.GameData.MazeMapData);
                                break;
                            case PhaseStage.EnterNormal:
                                ActionKit.Sequence()
                                    .DelayFrame(1) // 等待场景加载
                                    .Callback(() => { _GameModel.MazeMapController.SetMazeMapTiles(); })
                                    .Start(GameManager.Instance);
                                break;
                            case PhaseStage.EnterLate:
                                ActionKit.Sequence()
                                    .DelayFrame(2)
                                    .Callback(() => { _PhaseModel.DelayChangePhase(GamePhase.MazeMap); })
                                    .Start(GameManager.Instance);
                                break;
                                break;
                        }

                        break;
                }
            });
        }
    }
}