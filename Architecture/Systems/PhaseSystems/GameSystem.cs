using System.Collections.Generic;
using QFramework;
using TPL.PVZR.Architecture.Managers;
using TPL.PVZR.Architecture.Models;
using TPL.PVZR.Architecture.Systems.Interfaces;
using TPL.PVZR.Gameplay.Class.Games;
using TPL.PVZR.Gameplay.Class.MazeMap;
using UnityEngine;
using UnityEngine.SceneManagement;
using TPL.PVZR.Architecture.Events.GamePhase;
using TPL.PVZR.Core.Extensions;


namespace TPL.PVZR.Architecture.Systems.PhaseSystems
{
    public interface IGameSystem : ISystem, IPhaseCore
    {
    }

    public class GameSystem : AbstractSystem, IGameSystem
    {
        # region IGameSystem

        // 

        # endregion

        # region IPhaseManageSystem

        private void RegisterPhaseEvents()
        {
            RegisterGameInitialization();
            RegisterLevelExiting();
            RegisterMazeMap();
            RegisterGameExiting();
        }

        private void RegisterGameInitialization()
        {
            this.RegisterEvent<OnEnterPhaseEvent>(e =>
            {
                if (e.changeToPhase is GamePhaseSystem.GamePhase.GameInitialization)
                {
                    // 获取参数
                    var gameToEnter = e.parameters.GetPara<IGame>("gameToEnter");
                    //
                    ActionKit.Sequence()
                        .Callback(() => SceneTransitionManager.Instance.AddMaskReason("GameInitialization"))
                        .Condition(() => SceneTransitionManager.Instance.isMask)
                        .Callback(() =>
                        {
                            _GameModel.SetCurrentGame(gameToEnter);
                            _GamePhaseSystem.ChangePhase(GamePhaseSystem.GamePhase.MazeMap, new Dictionary<string, object> {{"enterByGameInitialization",true}});
                        })
                        .Start(GameManager.Instance);
                }
            });
        }

        private void RegisterLevelExiting()
        {
            this.RegisterEvent<OnEnterPhaseEvent>(e =>
            {
                if (e.changeToPhase is GamePhaseSystem.GamePhase.LevelExiting)
                {
                    _GameModel.currentGame.mazeMapSaveData.passSpotIds.Add(_GameModel.lastEnteredNode.id);
                    _GamePhaseSystem.ChangePhase(GamePhaseSystem.GamePhase.MazeMap);
                }
            });
        }

        private void RegisterMazeMap()
        {
            this.RegisterEvent<OnEnterPhaseEvent>(e =>
            {
                if (e.changeToPhase is GamePhaseSystem.GamePhase.MazeMap)
                {
                    // 获取参数
                    bool enterByGameInitialization = e.parameters.GetPara<bool>("enterByGameInitialization", false);
                    
                    //
                    AsyncOperation ao = null;
                    ActionKit.Sequence()
                        .Callback(() => SceneTransitionManager.Instance.AddMaskReason("MazeMap"))
                        .Condition(() => SceneTransitionManager.Instance.isMask)
                        .Callback(() => { ao = SceneManager.LoadSceneAsync("GameMapSceneTest"); })
                        .Condition(() => ao.isDone)
                        .Callback(() => // 生成构建地图
                        {
                            var mazeMap = MazeMapHelper.CreateMazeMap(_GameModel.currentGame.mazeMapSaveData);
                            _GameModel.SetCurrentMazeMap(mazeMap);
                            if (enterByGameInitialization)
                            {
                                _GameModel.SetLastEnteredNode(mazeMap.startNode);
                            }
                            mazeMap.GenerateMazeMapGO();
                        })
                        .Callback(() => { SceneTransitionManager.Instance.RemoveMaskReason("MazeMap"); })
                        .Start(GameManager.Instance);
                }
            });
        }

        private void RegisterGameExiting()
        {
            this.RegisterEvent<OnLeavePhaseEvent>(e =>
            {
                if (e.leaveFromPhase is GamePhaseSystem.GamePhase.GameExiting)
                {
                    // 重置GameModel
                    _GameModel.SetCurrentGame(null);
                    _GameModel.SetLastEnteredNode(null);
                    // _GameModel.currentMazeMap = null;
                }
            });
        }

        #endregion

        # region 私有

        // 引用
        private IGameModel _GameModel;
        private ILevelModel _LevelModel;
        private IGamePhaseSystem _GamePhaseSystem;

        // 初始化
        protected override void OnInit()
        {
            _GameModel = this.GetModel<IGameModel>();
            _LevelModel = this.GetModel<ILevelModel>();
            _GamePhaseSystem = this.GetSystem<IGamePhaseSystem>();
            RegisterPhaseEvents();
        }

        # endregion
    }
}